using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Data.Entities;

[Table("Divisions")]
public class DivisionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string ShortName { get; set; } = string.Empty;

    public RefereeTier Tier { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // ── Navigation ──
    public ICollection<FixtureEntity> Fixtures { get; set; } = new List<FixtureEntity>();
}
