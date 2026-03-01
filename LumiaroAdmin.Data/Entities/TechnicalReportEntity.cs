using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumiaroAdmin.Data.Entities;

/// <summary>
/// Technical Performance Report — reviewed by a former senior referee.
/// Competency scores are flattened into the table for optimal SQL query performance.
/// Each competency has Score (0-10), Notes, Strengths, and AreasForImprovement columns.
/// </summary>
[Table("TechnicalReports")]
public class TechnicalReportEntity
{
    [Key]
    public int AssessmentId { get; set; }

    [MaxLength(150)]
    public string ReviewerName { get; set; } = string.Empty;

    // ── Application of Law ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal LawScore { get; set; }

    [MaxLength(2000)]
    public string? LawNotes { get; set; }

    [MaxLength(1000)]
    public string? LawStrengths { get; set; }

    [MaxLength(1000)]
    public string? LawAreasForImprovement { get; set; }

    // ── Positioning & Movement ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal PositioningScore { get; set; }

    [MaxLength(2000)]
    public string? PositioningNotes { get; set; }

    [MaxLength(1000)]
    public string? PositioningStrengths { get; set; }

    [MaxLength(1000)]
    public string? PositioningAreasForImprovement { get; set; }

    // ── Fitness & Work Rate ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal FitnessScore { get; set; }

    [MaxLength(2000)]
    public string? FitnessNotes { get; set; }

    [MaxLength(1000)]
    public string? FitnessStrengths { get; set; }

    [MaxLength(1000)]
    public string? FitnessAreasForImprovement { get; set; }

    // ── Match Control ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal ControlScore { get; set; }

    [MaxLength(2000)]
    public string? ControlNotes { get; set; }

    [MaxLength(1000)]
    public string? ControlStrengths { get; set; }

    [MaxLength(1000)]
    public string? ControlAreasForImprovement { get; set; }

    // ── Overall ──
    [MaxLength(4000)]
    public string? OverallNotes { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(AssessmentId))]
    public AssessmentEntity Assessment { get; set; } = null!;
}
