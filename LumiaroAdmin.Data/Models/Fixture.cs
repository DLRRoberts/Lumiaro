namespace LumiaroAdmin.Models;

public class Division
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public RefereeTier Tier { get; set; }
    public int SortOrder { get; set; }
}

public class Fixture
{
    public int Id { get; set; }
    public int DivisionId { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public RefereeTier DivisionTier { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan KickOff { get; set; }
    public string KickOffDisplay => DateTime.Today.Add(KickOff).ToString("HH:mm");
    public string DateDisplay => Date.ToString("ddd dd MMM yyyy");
    public string DateShort => Date.ToString("dd MMM");
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public string MatchTitle => $"{HomeTeam} vs {AwayTeam}";
    public string? Venue { get; set; }
    public int Matchday { get; set; }

    // ── Official Assignment Slots ──
    public OfficialAssignment? Referee { get; set; }
    public OfficialAssignment? AssistantReferee1 { get; set; }
    public OfficialAssignment? AssistantReferee2 { get; set; }
    public OfficialAssignment? MatchOfficial { get; set; }

    public bool IsFullyAssigned =>
        Referee != null && AssistantReferee1 != null &&
        AssistantReferee2 != null && MatchOfficial != null;

    public int AssignedCount =>
        (Referee != null ? 1 : 0) + (AssistantReferee1 != null ? 1 : 0) +
        (AssistantReferee2 != null ? 1 : 0) + (MatchOfficial != null ? 1 : 0);

    public string AssignmentSummary => $"{AssignedCount}/4 assigned";

    public OfficialAssignment? GetSlot(OfficialRole role) => role switch
    {
        OfficialRole.Referee => Referee,
        OfficialRole.AssistantReferee1 => AssistantReferee1,
        OfficialRole.AssistantReferee2 => AssistantReferee2,
        OfficialRole.MatchOfficial => MatchOfficial,
        _ => null
    };

    public void SetSlot(OfficialRole role, OfficialAssignment? assignment)
    {
        switch (role)
        {
            case OfficialRole.Referee: Referee = assignment; break;
            case OfficialRole.AssistantReferee1: AssistantReferee1 = assignment; break;
            case OfficialRole.AssistantReferee2: AssistantReferee2 = assignment; break;
            case OfficialRole.MatchOfficial: MatchOfficial = assignment; break;
        }
    }

    /// <summary>Returns all referee IDs currently assigned to any slot in this fixture.</summary>
    public IEnumerable<int> GetAssignedRefereeIds()
    {
        if (Referee != null) yield return Referee.RefereeId;
        if (AssistantReferee1 != null) yield return AssistantReferee1.RefereeId;
        if (AssistantReferee2 != null) yield return AssistantReferee2.RefereeId;
        if (MatchOfficial != null) yield return MatchOfficial.RefereeId;
    }
}

public class OfficialAssignment
{
    public int RefereeId { get; set; }
    public string RefereeName { get; set; } = string.Empty;
    public string RefereeInitials { get; set; } = string.Empty;
    public string RefereeCode { get; set; } = string.Empty;
    public RefereeTier RefereeTier { get; set; }
    public OfficialRole Role { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.Now;

    public static OfficialAssignment FromReferee(Referee referee, OfficialRole role) => new()
    {
        RefereeId = referee.Id,
        RefereeName = referee.FullName,
        RefereeInitials = referee.Initials,
        RefereeCode = referee.RefCode,
        RefereeTier = referee.Tier,
        Role = role,
        AssignedAt = DateTime.Now
    };
}

public enum OfficialRole
{
    Referee,
    AssistantReferee1,
    AssistantReferee2,
    MatchOfficial
}

public static class OfficialRoleExtensions
{
    public static string Label(this OfficialRole role) => role switch
    {
        OfficialRole.Referee => "Referee",
        OfficialRole.AssistantReferee1 => "Assistant Referee 1",
        OfficialRole.AssistantReferee2 => "Assistant Referee 2",
        OfficialRole.MatchOfficial => "Match Official",
        _ => "Unknown"
    };

    public static string ShortLabel(this OfficialRole role) => role switch
    {
        OfficialRole.Referee => "REF",
        OfficialRole.AssistantReferee1 => "AR1",
        OfficialRole.AssistantReferee2 => "AR2",
        OfficialRole.MatchOfficial => "4TH",
        _ => "—"
    };
}

public class AssignmentConflict
{
    public int RefereeId { get; set; }
    public string RefereeName { get; set; } = string.Empty;
    public int ConflictFixtureId { get; set; }
    public string ConflictMatch { get; set; } = string.Empty;
    public OfficialRole ConflictRole { get; set; }
    public string Message => $"Already assigned as {ConflictRole.Label()} for {ConflictMatch} on this date";
}
