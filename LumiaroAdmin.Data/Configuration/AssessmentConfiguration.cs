using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Configuration;

public class AssessmentConfiguration : IEntityTypeConfiguration<AssessmentEntity>
{
    public void Configure(EntityTypeBuilder<AssessmentEntity> builder)
    {
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => new { a.FixtureId, a.RefereeId, a.OfficialRole })
            .IsUnique()
            .HasDatabaseName("IX_Assessments_Fixture_Referee_Role");
        builder.HasIndex(a => a.RefereeId);
        builder.HasIndex(a => a.FixtureId);

        builder.Property(a => a.OverallScore).HasColumnType("decimal(4,1)");

        // 1:1 — Technical Report (shared PK)
        builder.HasOne(a => a.TechnicalReport)
            .WithOne(t => t.Assessment)
            .HasForeignKey<TechnicalReportEntity>(t => t.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:1 — Delegate Report (shared PK)
        builder.HasOne(a => a.DelegateReport)
            .WithOne(d => d.Assessment)
            .HasForeignKey<DelegateReportEntity>(d => d.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:1 — Match Stat Report (shared PK)
        builder.HasOne(a => a.MatchStatReport)
            .WithOne(m => m.Assessment)
            .HasForeignKey<MatchStatReportEntity>(m => m.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:many — Key Match Incidents
        builder.HasMany(a => a.KeyMatchIncidents)
            .WithOne(k => k.Assessment)
            .HasForeignKey(k => k.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TechnicalReportConfiguration : IEntityTypeConfiguration<TechnicalReportEntity>
{
    public void Configure(EntityTypeBuilder<TechnicalReportEntity> builder)
    {
        // PK is AssessmentId (shared key 1:1) — already set via [Key] attribute
    }
}

public class DelegateReportConfiguration : IEntityTypeConfiguration<DelegateReportEntity>
{
    public void Configure(EntityTypeBuilder<DelegateReportEntity> builder)
    {
        // PK is AssessmentId (shared key 1:1) — already set via [Key] attribute
    }
}

public class MatchStatReportConfiguration : IEntityTypeConfiguration<MatchStatReportEntity>
{
    public void Configure(EntityTypeBuilder<MatchStatReportEntity> builder)
    {
        // PK is AssessmentId (shared key 1:1) — already set via [Key] attribute
    }
}

public class KeyMatchIncidentConfiguration : IEntityTypeConfiguration<KeyMatchIncidentEntity>
{
    public void Configure(EntityTypeBuilder<KeyMatchIncidentEntity> builder)
    {
        builder.HasIndex(k => new { k.AssessmentId, k.SortOrder });
        builder.HasIndex(k => new { k.AssessmentId, k.Minute });
    }
}
