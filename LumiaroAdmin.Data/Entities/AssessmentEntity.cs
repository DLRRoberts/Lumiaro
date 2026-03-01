using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

[Table("Assessments")]
public class AssessmentEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // ── Foreign Keys ──
    public int FixtureId { get; set; }
    public int RefereeId { get; set; }

    public OfficialRole OfficialRole { get; set; }

    // ── Metadata ──
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Draft;

    public double OverallScore { get; set; }

    [MaxLength(4000)]
    public string? SummaryNotes { get; set; }

    [MaxLength(4000)]
    public string? Recommendations { get; set; }

    [MaxLength(150)]
    public string CreatedBy { get; set; } = string.Empty;

    // ── KMI Panel Notes (incidents stored in child table) ──
    [MaxLength(4000)]
    public string? KmiPanelNotes { get; set; }

    // ── Audit ──
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(FixtureId))]
    public FixtureEntity Fixture { get; set; } = null!;

    [ForeignKey(nameof(RefereeId))]
    public RefereeEntity Referee { get; set; } = null!;

    public TechnicalReportEntity? TechnicalReport { get; set; }
    public DelegateReportEntity? DelegateReport { get; set; }
    public MatchStatReportEntity? MatchStatReport { get; set; }
    public ICollection<KeyMatchIncidentEntity> KeyMatchIncidents { get; set; } = new List<KeyMatchIncidentEntity>();
}
