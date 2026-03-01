using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

/// <summary>
/// Historical match record — used for referee profile match history and upcoming appointments.
/// Can reference a Fixture when one exists, or stand alone for imported historical data.
/// </summary>
[Table("Matches")]
public class MatchEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? FixtureId { get; set; }

    public int? RefereeId { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Required]
    [MaxLength(150)]
    public string HomeTeam { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string AwayTeam { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Competition { get; set; } = string.Empty;

    public RefereeTier CompetitionTier { get; set; }

    public int? HomeScore { get; set; }

    public int? AwayScore { get; set; }

    [MaxLength(50)]
    public string Role { get; set; } = "Referee";

    public int YellowCards { get; set; }

    public int RedCards { get; set; }

    [Column(TypeName = "decimal(4,1)")]
    public decimal? AssessmentScore { get; set; }

    // ── For future appointments display ──
    [MaxLength(200)]
    public string? Assistants { get; set; }

    [MaxLength(100)]
    public string? FourthOfficial { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(FixtureId))]
    public FixtureEntity? Fixture { get; set; }

    [ForeignKey(nameof(RefereeId))]
    public RefereeEntity? Referee { get; set; }

    public ICollection<MatchIncidentEntity> Incidents { get; set; } = new List<MatchIncidentEntity>();
}
