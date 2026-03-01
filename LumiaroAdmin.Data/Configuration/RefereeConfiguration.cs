using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Configuration;

public class RefereeConfiguration : IEntityTypeConfiguration<RefereeEntity>
{
    public void Configure(EntityTypeBuilder<RefereeEntity> builder)
    {
        builder.HasIndex(r => r.RefCode).IsUnique();
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.Tier);
        builder.HasIndex(r => r.Region);
        builder.HasIndex(r => new { r.LastName, r.FirstName });

        builder.Property(r => r.AverageScore).HasColumnType("decimal(4,1)");
        builder.Property(r => r.AccuracyPercent).HasColumnType("decimal(5,1)");
        builder.Property(r => r.CurrentSeasonAvgScore).HasColumnType("decimal(4,1)");

        builder.HasMany(r => r.CareerHistory)
            .WithOne(c => c.Referee)
            .HasForeignKey(c => c.RefereeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.SeasonBreakdown)
            .WithOne(s => s.Referee)
            .HasForeignKey(s => s.RefereeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Assignments)
            .WithOne(a => a.Referee)
            .HasForeignKey(a => a.RefereeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Assessments)
            .WithOne(a => a.Referee)
            .HasForeignKey(a => a.RefereeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CareerEntryConfiguration : IEntityTypeConfiguration<CareerEntryEntity>
{
    public void Configure(EntityTypeBuilder<CareerEntryEntity> builder)
    {
        builder.HasIndex(c => new { c.RefereeId, c.SortOrder });
    }
}

public class SeasonStatsConfiguration : IEntityTypeConfiguration<SeasonStatsEntity>
{
    public void Configure(EntityTypeBuilder<SeasonStatsEntity> builder)
    {
        builder.HasIndex(s => new { s.RefereeId, s.Season });
        builder.Property(s => s.AvgScore).HasColumnType("decimal(4,1)");
        builder.Property(s => s.TrendChange).HasColumnType("decimal(4,1)");
    }
}
