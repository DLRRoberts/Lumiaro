namespace LumiaroAdmin.Models;

public class Match
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string MatchTitle => $"{HomeTeam} vs {AwayTeam}";
    public string Competition { get; set; } = string.Empty;
    public RefereeTier CompetitionTier { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public string? ScoreDisplay => HomeScore.HasValue && AwayScore.HasValue
        ? $"{HomeScore} — {AwayScore}" : null;
    public string Role { get; set; } = "Referee";
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public List<MatchIncident> Incidents { get; set; } = new();
    public string IncidentSummary => Incidents.Count > 0
        ? string.Join(", ", Incidents.Select(i => i.Description)) : "—";
    public double? AssessmentScore { get; set; }

    // Referee info resolved via join
    public int? RefereeId { get; set; }
    public string? RefereeName { get; set; }
    public string? RefereeInitials { get; set; }

    // For appointments (future matches)
    public string? Assistants { get; set; }
    public string? FourthOfficial { get; set; }
}

public class MatchIncident
{
    public int Id { get; set; }
    public int? Minute { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? IncidentType { get; set; }
    public string? Notes { get; set; }
    public string Display => Minute.HasValue ? $"{Minute}' {Description}" : Description;
}

public class ActivityItem
{
    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = string.Empty;
    public string HighlightName { get; set; } = string.Empty;
    public string? HighlightValue { get; set; }
    public string? HighlightColor { get; set; }
    public string IconType { get; set; } = "cyan"; // cyan, lime, orange
    public string IconSvg { get; set; } = string.Empty;

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.Now - Timestamp;
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hours ago";
            if (diff.TotalDays < 2) return "Yesterday";
            return $"{(int)diff.TotalDays} days ago";
        }
    }
}

public class DashboardStats
{
    public int ActiveReferees { get; set; }
    public int SeasonChange { get; set; }
    public int MatchesThisSeason { get; set; }
    public int MatchesThisWeek { get; set; }
    public double AvgAssessmentScore { get; set; }
    public double ScoreChangeVsLastSeason { get; set; }
    public int PendingAppointments { get; set; }
    public string NextMatchday { get; set; } = string.Empty;
}
