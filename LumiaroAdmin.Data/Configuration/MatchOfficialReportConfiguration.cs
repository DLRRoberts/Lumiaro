using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedZone.LumiaroAdmin.Data.Entities;

namespace RedZone.LumiaroAdmin.Data.Configuration;

public class MatchOfficialReportConfiguration : IEntityTypeConfiguration<MatchOfficialReportEntity>
{
    public void Configure(EntityTypeBuilder<MatchOfficialReportEntity> builder)
    {
        builder.Property(r => r.ConfidentialNotes)
            .HasMaxLength(4000);

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.RefereeId);
        builder.HasIndex(r => r.FixtureId);
        builder.HasIndex(r => r.MatchDate);
        builder.HasIndex(r => new { r.FixtureId, r.RefereeId, r.OfficialRole })
            .IsUnique()
            .HasDatabaseName("IX_Reports_Fixture_Referee_Role");

        builder.HasMany(r => r.Sections)
            .WithOne(s => s.Report)
            .HasForeignKey(s => s.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Events)
            .WithOne(e => e.Report)
            .HasForeignKey(e => e.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.VideoClips)
            .WithOne(v => v.Report)
            .HasForeignKey(v => v.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReportSectionConfiguration : IEntityTypeConfiguration<ReportSectionEntity>
{
    public void Configure(EntityTypeBuilder<ReportSectionEntity> builder)
    {
        builder.HasIndex(s => new { s.ReportId, s.SortOrder });
    }
}

public class ReportEventConfiguration : IEntityTypeConfiguration<ReportEventEntity>
{
    public void Configure(EntityTypeBuilder<ReportEventEntity> builder)
    {
        builder.HasIndex(e => new { e.ReportId, e.Minute });
    }
}

public class VideoClipConfiguration : IEntityTypeConfiguration<VideoClipEntity>
{
    public void Configure(EntityTypeBuilder<VideoClipEntity> builder)
    {
        builder.HasIndex(v => new { v.ReportId, v.SortOrder });
    }
}

public class InterventionReviewConfiguration : IEntityTypeConfiguration<InterventionReviewEntity>
{
    public void Configure(EntityTypeBuilder<InterventionReviewEntity> builder)
    {
        builder.HasIndex(r => new { r.ReportId, r.EventId });
        builder.HasIndex(r => new { r.ReportId, r.ReviewerName });
        builder.HasIndex(r => r.ReportId);
        
        builder.HasOne(r => r.Report)
            .WithMany()
            .HasForeignKey(r => r.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        // ReportEvent → InterventionReview: restrict (breaks the multiple cascade path)
        builder.HasOne(r => r.Event)
            .WithMany()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLogEntity>
{
    public void Configure(EntityTypeBuilder<ActivityLogEntity> builder)
    {
        builder.HasIndex(a => a.Timestamp);
    }
}
