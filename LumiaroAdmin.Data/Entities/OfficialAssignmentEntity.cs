using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

[Table("OfficialAssignments")]
public class OfficialAssignmentEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int FixtureId { get; set; }

    public int RefereeId { get; set; }

    public OfficialRole Role { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(150)]
    public string? AssignedBy { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // ── Navigation ──
    [ForeignKey(nameof(FixtureId))]
    public FixtureEntity Fixture { get; set; } = null!;

    [ForeignKey(nameof(RefereeId))]
    public RefereeEntity Referee { get; set; } = null!;
}
