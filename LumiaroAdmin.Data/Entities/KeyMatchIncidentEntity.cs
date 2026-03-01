using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

/// <summary>
/// Key Match Incident — individual decision reviewed by the KMI panel.
/// Child of Assessment (1:many).
/// </summary>
[Table("KeyMatchIncidents")]
public class KeyMatchIncidentEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int AssessmentId { get; set; }

    public int Minute { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public KmiDecisionType DecisionType { get; set; }

    public bool OriginalDecisionCorrect { get; set; }

    public bool VarInterventionRequired { get; set; }

    public bool VarInterventionMade { get; set; }

    public int PanelVotesCorrect { get; set; }

    public int PanelVotesIncorrect { get; set; }

    [MaxLength(2000)]
    public string? Rationale { get; set; }

    public int SortOrder { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(AssessmentId))]
    public AssessmentEntity Assessment { get; set; } = null!;
}
