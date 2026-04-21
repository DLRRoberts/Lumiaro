using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;

namespace RedZone.LumiaroAdmin.Data.Entities;

// ══════════════════════════════════════════════════
// MATCH OFFICIAL REPORT — Main report entity
// ══════════════════════════════════════════════════

[Table("MatchOfficialReports")]
public class MatchOfficialReportEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // ── Foreign Keys ──
    public int FixtureId { get; set; }
    public int RefereeId { get; set; }
    public OfficialRole OfficialRole { get; set; }

    // ── Match Info (denormalised for query perf) ──
    [Required] [MaxLength(300)]
    public string MatchTitle { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime MatchDate { get; set; }

    [MaxLength(200)]
    public string? Venue { get; set; }

    [MaxLength(200)]
    public string? RefereeName { get; set; }

    [MaxLength(10)]
    public string? RefereeCode { get; set; }

    [MaxLength(5)]
    public string? RefereeInitials { get; set; }
    
    [MaxLength(50)]
    public string? Language { get; set; }

    public RefereeTier RefereeTier { get; set; }
    public RefereeTier CompetitionTier { get; set; }

    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    // ── Report Content ──
    public ReportStatus Status { get; set; } = ReportStatus.Draft;

    [Column(TypeName = "decimal(4,1)")]
    public decimal OverallRating { get; set; }

    [MaxLength(4000)]
    public string? ExecutiveSummary { get; set; }

    [MaxLength(4000)]
    public string? DevelopmentPriorities { get; set; }

    [MaxLength(4000)]
    public string? ConfidentialNotes { get; set; }

    [MaxLength(200)]
    public string AuthorName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AuthorRole { get; set; }

    // ── Audit ──
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(FixtureId))]
    public FixtureEntity Fixture { get; set; } = null!;

    [ForeignKey(nameof(RefereeId))]
    public RefereeEntity Referee { get; set; } = null!;

    public ICollection<ReportSectionEntity> Sections { get; set; } = new List<ReportSectionEntity>();
    public ICollection<ReportEventEntity> Events { get; set; } = new List<ReportEventEntity>();
    public ICollection<VideoClipEntity> VideoClips { get; set; } = new List<VideoClipEntity>();
}

// ══════════════════════════════════════════════════
// REPORT SECTION — Performance dimension rating + narrative
// ══════════════════════════════════════════════════

[Table("ReportSections")]
public class ReportSectionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ReportId { get; set; }

    [Required] [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "decimal(4,1)")]
    public decimal Rating { get; set; }

    [MaxLength(4000)]
    public string? Narrative { get; set; }

    [MaxLength(2000)]
    public string? Strengths { get; set; }

    [MaxLength(2000)]
    public string? AreasForImprovement { get; set; }

    public int SortOrder { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(ReportId))]
    public MatchOfficialReportEntity Report { get; set; } = null!;
}

// ══════════════════════════════════════════════════
// REPORT EVENT — Match timeline event
// ══════════════════════════════════════════════════

[Table("ReportEvents")]
public class ReportEventEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ReportId { get; set; }

    public int Minute { get; set; }
    public int? AddedTime { get; set; }
    public EventType EventType { get; set; }

    [Required] [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public EventImpact Impact { get; set; } = EventImpact.Medium;
    public bool OfficialInvolved { get; set; }

    [MaxLength(2000)]
    public string? OfficialResponse { get; set; }

    [MaxLength(20)]
    public string? Teams { get; set; }

    public DecisionAccuracy Accuracy { get; set; } = DecisionAccuracy.NotApplicable;

    // ── Navigation ──
    [ForeignKey(nameof(ReportId))]
    public MatchOfficialReportEntity Report { get; set; } = null!;
}

// ══════════════════════════════════════════════════
// VIDEO CLIP — Attached to a report
// ══════════════════════════════════════════════════

[Table("VideoClips")]
public class VideoClipEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ReportId { get; set; }

    [Required] [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Url { get; set; }

    [MaxLength(100)]
    public string? Timestamp { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey(nameof(ReportId))]
    public MatchOfficialReportEntity Report { get; set; } = null!;
}

// ══════════════════════════════════════════════════
// INTERVENTION REVIEW — Reviewer score for a single event
// ══════════════════════════════════════════════════

[Table("InterventionReviews")]
public class InterventionReviewEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ReportId { get; set; }
    public int EventId { get; set; }

    [Required] [MaxLength(150)]
    public string ReviewerName { get; set; } = string.Empty;

    [Required] [MaxLength(80)]
    public string ReviewerRole { get; set; } = string.Empty;

    public int AccuracyScore { get; set; }
    public int ConsistencyScore { get; set; }
    public int CommunicationScore { get; set; }
    public int MatchHandlingScore { get; set; }

    [MaxLength(2000)]
    public string? Comment { get; set; }

    public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;

    // ── Navigation ──
    [ForeignKey(nameof(ReportId))]
    public MatchOfficialReportEntity Report { get; set; } = null!;

    [ForeignKey(nameof(EventId))]
    public ReportEventEntity Event { get; set; } = null!;
}

// ══════════════════════════════════════════════════
// ACTIVITY LOG — System activity feed
// ══════════════════════════════════════════════════

[Table("ActivityLog")]
public class ActivityLogEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(150)]
    public string? HighlightName { get; set; }

    [Required] [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? HighlightValue { get; set; }

    [MaxLength(30)]
    public string? HighlightColor { get; set; }

    [MaxLength(20)]
    public string IconType { get; set; } = "cyan";
}

// ══════════════════════════════════════════════════
// ENUMS needed in Data layer
// ══════════════════════════════════════════════════
// These are additions to DataEnums.cs in the Enums folder
