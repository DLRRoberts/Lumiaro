using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumiaroAdmin.Data.Entities;

/// <summary>
/// Match Delegate Report — assessed by former players/managers focusing on soft skills.
/// Competency scores flattened into columns for SQL query performance.
/// </summary>
[Table("DelegateReports")]
public class DelegateReportEntity
{
    [Key]
    public int AssessmentId { get; set; }

    [MaxLength(150)]
    public string DelegateName { get; set; } = string.Empty;

    // ── Management of Game ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal ManagementScore { get; set; }

    [MaxLength(2000)]
    public string? ManagementNotes { get; set; }

    [MaxLength(1000)]
    public string? ManagementStrengths { get; set; }

    [MaxLength(1000)]
    public string? ManagementAreasForImprovement { get; set; }

    // ── Communication ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal CommunicationScore { get; set; }

    [MaxLength(2000)]
    public string? CommunicationNotes { get; set; }

    [MaxLength(1000)]
    public string? CommunicationStrengths { get; set; }

    [MaxLength(1000)]
    public string? CommunicationAreasForImprovement { get; set; }

    // ── Consistency ──
    [Column(TypeName = "decimal(4,1)")]
    public decimal ConsistencyScore { get; set; }

    [MaxLength(2000)]
    public string? ConsistencyNotes { get; set; }

    [MaxLength(1000)]
    public string? ConsistencyStrengths { get; set; }

    [MaxLength(1000)]
    public string? ConsistencyAreasForImprovement { get; set; }

    // ── Overall ──
    [MaxLength(4000)]
    public string? OverallNotes { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(AssessmentId))]
    public AssessmentEntity Assessment { get; set; } = null!;
}
