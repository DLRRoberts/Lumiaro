using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Configuration;

public class DivisionConfiguration : IEntityTypeConfiguration<DivisionEntity>
{
    public void Configure(EntityTypeBuilder<DivisionEntity> builder)
    {
        builder.HasIndex(d => d.ShortName).IsUnique();
        builder.HasIndex(d => d.SortOrder);

        builder.HasMany(d => d.Fixtures)
            .WithOne(f => f.Division)
            .HasForeignKey(f => f.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FixtureConfiguration : IEntityTypeConfiguration<FixtureEntity>
{
    public void Configure(EntityTypeBuilder<FixtureEntity> builder)
    {
        builder.HasIndex(f => f.Date);
        builder.HasIndex(f => new { f.DivisionId, f.Date });
        builder.HasIndex(f => f.Matchday);
        builder.HasIndex(f => new { f.HomeTeam, f.AwayTeam, f.Date })
            .IsUnique()
            .HasDatabaseName("IX_Fixtures_Match_Unique");

        builder.HasMany(f => f.OfficialAssignments)
            .WithOne(a => a.Fixture)
            .HasForeignKey(a => a.FixtureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Assessments)
            .WithOne(a => a.Fixture)
            .HasForeignKey(a => a.FixtureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OfficialAssignmentConfiguration : IEntityTypeConfiguration<OfficialAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<OfficialAssignmentEntity> builder)
    {
        // One referee can only fill one role per fixture
        builder.HasIndex(a => new { a.FixtureId, a.RefereeId })
            .IsUnique()
            .HasDatabaseName("IX_OfficialAssignments_Fixture_Referee");

        // Only one official per role per fixture
        builder.HasIndex(a => new { a.FixtureId, a.Role })
            .IsUnique()
            .HasDatabaseName("IX_OfficialAssignments_Fixture_Role");

        builder.HasIndex(a => new { a.RefereeId, a.FixtureId });
    }
}

public class MatchConfiguration : IEntityTypeConfiguration<MatchEntity>
{
    public void Configure(EntityTypeBuilder<MatchEntity> builder)
    {
        builder.HasIndex(m => m.Date);
        builder.HasIndex(m => m.RefereeId);
        builder.HasIndex(m => new { m.RefereeId, m.Date });

        builder.HasOne(m => m.Referee)
            .WithMany()
            .HasForeignKey(m => m.RefereeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Incidents)
            .WithOne(i => i.Match)
            .HasForeignKey(i => i.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class MatchIncidentConfiguration : IEntityTypeConfiguration<MatchIncidentEntity>
{
    public void Configure(EntityTypeBuilder<MatchIncidentEntity> builder)
    {
        builder.HasIndex(i => i.MatchId);
    }
}
