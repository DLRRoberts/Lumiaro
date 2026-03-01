using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

[Table("RefereeCareerEntries")]
public class CareerEntryEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int RefereeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Period { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Detail { get; set; } = string.Empty;

    public bool IsCurrent { get; set; }

    public int SortOrder { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(RefereeId))]
    public RefereeEntity Referee { get; set; } = null!;
}

[Table("RefereeSeasonStats")]
public class SeasonStatsEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int RefereeId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Season { get; set; } = string.Empty;

    public RefereeTier Tier { get; set; }

    public int Matches { get; set; }

    public int Yellows { get; set; }

    public int Reds { get; set; }

    public int Penalties { get; set; }

    public double AvgScore { get; set; }

    public double TrendChange { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(RefereeId))]
    public RefereeEntity Referee { get; set; } = null!;
}
