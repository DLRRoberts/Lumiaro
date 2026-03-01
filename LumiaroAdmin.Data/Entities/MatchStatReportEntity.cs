using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumiaroAdmin.Data.Entities;

/// <summary>
/// Match Statistics Report — discipline data, physical performance metrics, and incident notes.
/// 1:1 relationship with Assessment via shared primary key.
/// </summary>
[Table("MatchStatReports")]
public class MatchStatReportEntity
{
    [Key]
    public int AssessmentId { get; set; }

    // ── Discipline ──
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int PenaltiesAwarded { get; set; }
    public int FoulsGiven { get; set; }
    public int OffsidesCalled { get; set; }

    // ── Physical Performance ──
    [Column(TypeName = "decimal(5,1)")]
    public decimal DistanceCoveredKm { get; set; }

    [Column(TypeName = "decimal(5,1)")]
    public decimal AvgSprintSpeedKmh { get; set; }

    public int HighIntensityRuns { get; set; }

    // ── Notes ──
    [MaxLength(4000)]
    public string? IncidentNotes { get; set; }

    [MaxLength(4000)]
    public string? VarInteractionNotes { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(AssessmentId))]
    public AssessmentEntity Assessment { get; set; } = null!;
}
