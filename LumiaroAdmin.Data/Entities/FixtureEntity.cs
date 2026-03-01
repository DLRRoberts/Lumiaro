using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LumiaroAdmin.Data.Entities;

[Table("Fixtures")]
public class FixtureEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int DivisionId { get; set; }

    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    public TimeSpan KickOff { get; set; }

    [Required]
    [MaxLength(150)]
    public string HomeTeam { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string AwayTeam { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Venue { get; set; }

    public int Matchday { get; set; }

    public int? HomeScore { get; set; }

    public int? AwayScore { get; set; }

    public bool IsCancelled { get; set; }

    // ── Audit ──
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(DivisionId))]
    public DivisionEntity Division { get; set; } = null!;

    public ICollection<OfficialAssignmentEntity> OfficialAssignments { get; set; } = new List<OfficialAssignmentEntity>();
    public ICollection<AssessmentEntity> Assessments { get; set; } = new List<AssessmentEntity>();
}
