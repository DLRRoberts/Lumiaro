using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumiaroAdmin.Data.Entities;

/// <summary>
/// Individual incident record for a match — penalties, red cards, VAR reviews, etc.
/// Multiple incidents can be recorded against a single match.
/// </summary>
[Table("MatchIncidents")]
public class MatchIncidentEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int MatchId { get; set; }

    public int? Minute { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? IncidentType { get; set; }  // Penalty, RedCard, VAR, Injury, etc.

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(MatchId))]
    public MatchEntity Match { get; set; } = null!;
}