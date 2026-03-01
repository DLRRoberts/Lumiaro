namespace LumiaroAdmin.Models;

public enum RefereeStatus
{
    Active,
    Inactive,
    Probation,
    Suspended
}

public enum RefereeTier
{
    FifaInternational,
    PremierLeague,
    Championship,
    LeagueOne,
    LeagueTwo,
    NationalLeague,
    Grassroots
}

public class Referee
{
    public int Id { get; set; }
    public string RefCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Initials => $"{(FirstName.Length > 0 ? FirstName[0] : ' ')}{(LastName.Length > 0 ? LastName[0] : ' ')}";
    public DateTime DateOfBirth { get; set; }
    public int Age => (int)((DateTime.Today - DateOfBirth).TotalDays / 365.25);
    public string Nationality { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public RefereeTier Tier { get; set; }
    public RefereeStatus Status { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime? LastActiveDate { get; set; }

    // Computed stats (would come from match data in production)
    public int TotalMatches { get; set; }
    public double AverageScore { get; set; }
    public double AccuracyPercent { get; set; }
    public int TotalSeasons { get; set; }

    // Current season stats
    public int CurrentSeasonMatches { get; set; }
    public double CurrentSeasonAvgScore { get; set; }
    public int CurrentSeasonYellows { get; set; }
    public int CurrentSeasonReds { get; set; }
    public int CurrentSeasonPenalties { get; set; }
    public int CurrentSeasonIncidents { get; set; }

    public List<CareerEntry> CareerHistory { get; set; } = new();
    public List<SeasonStats> SeasonBreakdown { get; set; } = new();
}

public class CareerEntry
{
    public string Period { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}

public class SeasonStats
{
    public string Season { get; set; } = string.Empty;
    public RefereeTier Tier { get; set; }
    public int Matches { get; set; }
    public int Yellows { get; set; }
    public int Reds { get; set; }
    public int Penalties { get; set; }
    public double AvgScore { get; set; }
    public double TrendChange { get; set; }
}
