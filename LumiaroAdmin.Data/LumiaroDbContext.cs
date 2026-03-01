using LumiaroAdmin.Data.Entities;
using Microsoft.EntityFrameworkCore;
using RedZone.LumiaroAdmin.Data.Entities;

namespace RedZone.LumiaroAdmin.Data;

public class LumiaroDbContext : DbContext
{
    public LumiaroDbContext(DbContextOptions<LumiaroDbContext> options) : base(options) { }

    // ── Core Entities ──
    public DbSet<RefereeEntity> Referees => Set<RefereeEntity>();
    public DbSet<CareerEntryEntity> CareerEntries => Set<CareerEntryEntity>();
    public DbSet<SeasonStatsEntity> SeasonStats => Set<SeasonStatsEntity>();
    public DbSet<DivisionEntity> Divisions => Set<DivisionEntity>();
    public DbSet<FixtureEntity> Fixtures => Set<FixtureEntity>();
    public DbSet<OfficialAssignmentEntity> OfficialAssignments => Set<OfficialAssignmentEntity>();
    public DbSet<MatchEntity> Matches => Set<MatchEntity>();
    public DbSet<MatchIncidentEntity> MatchIncidents => Set<MatchIncidentEntity>();

    // ── Assessment Entities ──
    public DbSet<AssessmentEntity> Assessments => Set<AssessmentEntity>();
    public DbSet<TechnicalReportEntity> TechnicalReports => Set<TechnicalReportEntity>();
    public DbSet<DelegateReportEntity> DelegateReports => Set<DelegateReportEntity>();
    public DbSet<MatchStatReportEntity> MatchStatReports => Set<MatchStatReportEntity>();
    public DbSet<KeyMatchIncidentEntity> KeyMatchIncidents => Set<KeyMatchIncidentEntity>();

    // ── Match Report Entities ──
    public DbSet<MatchOfficialReportEntity> MatchOfficialReports => Set<MatchOfficialReportEntity>();
    public DbSet<ReportSectionEntity> ReportSections => Set<ReportSectionEntity>();
    public DbSet<ReportEventEntity> ReportEvents => Set<ReportEventEntity>();
    public DbSet<VideoClipEntity> VideoClips => Set<VideoClipEntity>();

    // ── Review & Activity Entities ──
    public DbSet<InterventionReviewEntity> InterventionReviews => Set<InterventionReviewEntity>();
    public DbSet<ActivityLogEntity> ActivityLog => Set<ActivityLogEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> from the Configuration folder
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LumiaroDbContext).Assembly);
    }
}
