using System.ComponentModel.DataAnnotations;

namespace LumiaroAdmin.Models;

public class RefereeFormModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Nationality is required")]
    public string Nationality { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required")]
    public string Location { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tier is required")]
    public RefereeTier? Tier { get; set; }

    public RefereeStatus Status { get; set; } = RefereeStatus.Active;

    public DateTime RegistrationDate { get; set; } = DateTime.Today;

    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Phone { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsEdit => Id.HasValue;

    public static RefereeFormModel FromReferee(Referee referee) => new()
    {
        Id = referee.Id,
        FirstName = referee.FirstName,
        LastName = referee.LastName,
        DateOfBirth = referee.DateOfBirth,
        Nationality = referee.Nationality,
        Location = referee.Location,
        Region = referee.Region,
        Tier = referee.Tier,
        Status = referee.Status,
        RegistrationDate = referee.RegistrationDate,
        Email = referee.Email,
        Phone = referee.Phone,
        Notes = referee.Notes
    };

    public Referee ToReferee() => new()
    {
        Id = Id ?? 0,
        FirstName = FirstName,
        LastName = LastName,
        DateOfBirth = DateOfBirth ?? DateTime.Today,
        Nationality = Nationality,
        Location = Location,
        Region = Region,
        Tier = Tier ?? RefereeTier.Grassroots,
        Status = Status,
        RegistrationDate = RegistrationDate,
        Email = Email,
        Phone = Phone,
        Notes = Notes
    };
}
