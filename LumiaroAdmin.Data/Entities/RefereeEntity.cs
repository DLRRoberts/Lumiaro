using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

[Table("Referees")]
public class RefereeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string RefCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [MaxLength(80)]
    public string Nationality { get; set; } = string.Empty;

    [MaxLength(150)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(80)]
    public string Region { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [MaxLength(256)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public RefereeTier Tier { get; set; }

    public RefereeStatus Status { get; set; }

    public DateTime RegistrationDate { get; set; }

    public DateTime? LastActiveDate { get; set; }

    // ── Aggregate stats (denormalised for performance) ──
    public int TotalMatches { get; set; }
    public double AverageScore { get; set; }
    public double AccuracyPercent { get; set; }
    public int TotalSeasons { get; set; }

    // ── Current season snapshot ──
    public int CurrentSeasonMatches { get; set; }
    public double CurrentSeasonAvgScore { get; set; }
    public int CurrentSeasonYellows { get; set; }
    public int CurrentSeasonReds { get; set; }
    public int CurrentSeasonPenalties { get; set; }
    public int CurrentSeasonIncidents { get; set; }

    // ── Audit ──
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // ── Navigation ──
    public ICollection<CareerEntryEntity> CareerHistory { get; set; } = new List<CareerEntryEntity>();
    public ICollection<SeasonStatsEntity> SeasonBreakdown { get; set; } = new List<SeasonStatsEntity>();
    public ICollection<OfficialAssignmentEntity> Assignments { get; set; } = new List<OfficialAssignmentEntity>();
    public ICollection<AssessmentEntity> Assessments { get; set; } = new List<AssessmentEntity>();
}
