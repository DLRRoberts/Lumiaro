using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Models;
using LumiaroAdmin.Models;
using RedZone.LumiaroAdmin.Data.Entities;
using AssessmentStatus = LumiaroAdmin.Data.Enums.AssessmentStatus;
using DecisionAccuracy = LumiaroAdmin.Data.Enums.DecisionAccuracy;
using EventImpact = LumiaroAdmin.Data.Enums.EventImpact;
using EventType = LumiaroAdmin.Data.Enums.EventType;
using KmiDecisionType = LumiaroAdmin.Data.Enums.KmiDecisionType;
using OfficialRole = LumiaroAdmin.Data.Enums.OfficialRole;
using RefereeStatus = LumiaroAdmin.Data.Enums.RefereeStatus;
using RefereeTier = LumiaroAdmin.Data.Enums.RefereeTier;
using ReportStatus = LumiaroAdmin.Data.Enums.ReportStatus;

namespace LumiaroAdmin.Services;

public static class Mappings
{
    public static LumiaroAdmin.Models.RefereeTier ToDomain(this RefereeTier t) => (LumiaroAdmin.Models.RefereeTier)(int)t;
    public static LumiaroAdmin.Data.Enums.RefereeTier ToData(this LumiaroAdmin.Models.RefereeTier t) => (LumiaroAdmin.Data.Enums.RefereeTier)(int)t;

    
    public static LumiaroAdmin.Models.OfficialRole ToDomain(this OfficialRole t) => (LumiaroAdmin.Models.OfficialRole)(int)t;
    public static LumiaroAdmin.Data.Enums.OfficialRole ToData(this LumiaroAdmin.Models.OfficialRole t) => (LumiaroAdmin.Data.Enums.OfficialRole)(int)t;

    
    public static LumiaroAdmin.Models.DecisionAccuracy ToDomain(this DecisionAccuracy t) => (LumiaroAdmin.Models.DecisionAccuracy)(int)t;
    public static LumiaroAdmin.Data.Enums.DecisionAccuracy ToData(this LumiaroAdmin.Models.DecisionAccuracy t) => (LumiaroAdmin.Data.Enums.DecisionAccuracy)(int)t;
    
    
    // ══════════════════════════════════════════════════
    // ENUM CONVERSIONS — Data ↔ Domain (same int values)
    // ══════════════════════════════════════════════════

    // RefereeStatus
    public static LumiaroAdmin.Models.RefereeStatus ToDomain(this Data.Enums.RefereeStatus s) => (LumiaroAdmin.Models.RefereeStatus)(int)s;
    public static Data.Enums.RefereeStatus ToData(this LumiaroAdmin.Models.RefereeStatus s) => (Data.Enums.RefereeStatus)(int)s;

    // AssessmentStatus
    public static LumiaroAdmin.Data.Models.AssessmentStatus ToDomain(this Data.Enums.AssessmentStatus s) => (LumiaroAdmin.Data.Models.AssessmentStatus)(int)s;
    public static Data.Enums.AssessmentStatus ToData(this LumiaroAdmin.Data.Models.AssessmentStatus s) => (Data.Enums.AssessmentStatus)(int)s;

    // KmiDecisionType
    public static LumiaroAdmin.Data.Models.KmiDecisionType ToDomain(this Data.Enums.KmiDecisionType d) => (LumiaroAdmin.Data.Models.KmiDecisionType)(int)d;
    public static Data.Enums.KmiDecisionType ToData(this LumiaroAdmin.Data.Models.KmiDecisionType d) => (Data.Enums.KmiDecisionType)(int)d;

    // ReportStatus
    public static LumiaroAdmin.Models.ReportStatus ToDomain(this Data.Enums.ReportStatus s) => (LumiaroAdmin.Models.ReportStatus)(int)s;
    public static Data.Enums.ReportStatus ToData(this LumiaroAdmin.Models.ReportStatus s) => (Data.Enums.ReportStatus)(int)s;

    // EventType
    public static LumiaroAdmin.Models.EventType ToDomain(this Data.Enums.EventType t) => (LumiaroAdmin.Models.EventType)(int)t;
    public static Data.Enums.EventType ToData(this LumiaroAdmin.Models.EventType t) => (Data.Enums.EventType)(int)t;

    // EventImpact
    public static LumiaroAdmin.Models.EventImpact ToDomain(this Data.Enums.EventImpact i) => (LumiaroAdmin.Models.EventImpact)(int)i;
    public static Data.Enums.EventImpact ToData(this LumiaroAdmin.Models.EventImpact i) => (Data.Enums.EventImpact)(int)i;

    
    // ══════════════════════════════════════════════════
    // REFEREE → Domain
    // ══════════════════════════════════════════════════

    public static Referee ToDomain(this RefereeEntity e) => new()
    {
        Id = e.Id,
        RefCode = e.RefCode,
        FirstName = e.FirstName,
        LastName = e.LastName,
        DateOfBirth = e.DateOfBirth,
        Nationality = e.Nationality,
        Location = e.Location,
        Region = e.Region,
        PhotoUrl = e.PhotoUrl,
        Email = e.Email,
        Phone = e.Phone,
        Notes = e.Notes,
        Tier = e.Tier.ToDomain(),
        Status = e.Status.ToDomain(),
        RegistrationDate = e.RegistrationDate,
        LastActiveDate = e.LastActiveDate,
        TotalMatches = e.TotalMatches,
        AverageScore = e.AverageScore,
        AccuracyPercent = e.AccuracyPercent,
        TotalSeasons = e.TotalSeasons,
        CurrentSeasonMatches = e.CurrentSeasonMatches,
        CurrentSeasonAvgScore = e.CurrentSeasonAvgScore,
        CurrentSeasonYellows = e.CurrentSeasonYellows,
        CurrentSeasonReds = e.CurrentSeasonReds,
        CurrentSeasonPenalties = e.CurrentSeasonPenalties,
        CurrentSeasonIncidents = e.CurrentSeasonIncidents,
        CareerHistory = e.CareerHistory?.OrderBy(c => c.SortOrder).Select(c => new CareerEntry
        {
            Period = c.Period,
            Title = c.Title,
            Detail = c.Detail,
            IsCurrent = c.IsCurrent,
        }).ToList() ?? new(),
        SeasonBreakdown = e.SeasonBreakdown?.Select(s => new SeasonStats
        {
            Season = s.Season,
            Tier = s.Tier.ToDomain(),
            Matches = s.Matches,
            Yellows = s.Yellows,
            Reds = s.Reds,
            Penalties = s.Penalties,
            AvgScore = s.AvgScore,
            TrendChange = s.TrendChange,
        }).ToList() ?? new(),
    };

    // ══════════════════════════════════════════════════
    // MATCH (historical) → Domain
    // ══════════════════════════════════════════════════

    public static Match ToDomain(this MatchEntity e) => new()
    {
        Id = e.Id,
        Date = e.Date,
        HomeTeam = e.HomeTeam,
        AwayTeam = e.AwayTeam,
        Competition = e.Competition,
        CompetitionTier = e.CompetitionTier.ToDomain(),
        HomeScore = e.HomeScore,
        AwayScore = e.AwayScore,
        Role = e.Role,
        YellowCards = e.YellowCards,
        RedCards = e.RedCards,
        Incidents = MapIncidents(e.Incidents),
        AssessmentScore = e.AssessmentScore.HasValue ? (double)e.AssessmentScore.Value : null,
        RefereeId = e.RefereeId ?? 0,
        Assistants = e.Assistants,
        FourthOfficial = e.FourthOfficial,
    };

    private static List<MatchIncident> MapIncidents(ICollection<MatchIncidentEntity> incidents)
    {
        return incidents.Select(MapIncident).ToList();
    }

    private static MatchIncident MapIncident(MatchIncidentEntity inc)
    {
        return new MatchIncident
        {
            Id = inc.Id,
            Description = inc.Description,
            IncidentType = inc.IncidentType,
            Minute = inc.Minute,
            Notes = inc.Notes
        };
    }

    // ══════════════════════════════════════════════════
    // DIVISION → Domain
    // ══════════════════════════════════════════════════

    public static Division ToDomain(this DivisionEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        ShortName = e.ShortName,
        Tier = e.Tier.ToDomain(),
        SortOrder = e.SortOrder,
    };

    // ══════════════════════════════════════════════════
    // FIXTURE → Domain (with official assignment slots)
    // ══════════════════════════════════════════════════

    public static Fixture ToDomain(this FixtureEntity e) => new()
    {
        Id = e.Id,
        DivisionId = e.DivisionId,
        DivisionName = e.Division?.Name ?? "",
        DivisionTier = (e.Division?.Tier ?? Data.Enums.RefereeTier.Grassroots).ToDomain(),
        Date = e.Date,
        KickOff = e.KickOff,
        Matchday = e.Matchday,
        HomeTeam = e.HomeTeam,
        AwayTeam = e.AwayTeam,
        Venue = e.Venue,
        Referee = MapSlot(e, Data.Enums.OfficialRole.Referee),
        AssistantReferee1 = MapSlot(e, Data.Enums.OfficialRole.AssistantReferee1),
        AssistantReferee2 = MapSlot(e, Data.Enums.OfficialRole.AssistantReferee2),
        MatchOfficial = MapSlot(e, Data.Enums.OfficialRole.MatchOfficial),
    };

    private static OfficialAssignment? MapSlot(FixtureEntity e, Data.Enums.OfficialRole role)
    {
        var a = e.OfficialAssignments?.FirstOrDefault(x => x.Role == role);
        if (a == null) return null;
        return new OfficialAssignment
        {
            RefereeId = a.RefereeId,
            RefereeName = a.Referee != null ? $"{a.Referee.FirstName} {a.Referee.LastName}" : "",
            RefereeCode = a.Referee?.RefCode ?? "",
            RefereeInitials = a.Referee != null
                ? $"{(a.Referee.FirstName.Length > 0 ? a.Referee.FirstName[0] : ' ')}{(a.Referee.LastName.Length > 0 ? a.Referee.LastName[0] : ' ')}"
                : "",
            RefereeTier = (a.Referee?.Tier ?? Data.Enums.RefereeTier.Grassroots).ToDomain(),
            Role = a.Role.ToDomain(),
        };
    }

    // ══════════════════════════════════════════════════
    // ASSESSMENT → Domain (with child reports + KMIs)
    // ══════════════════════════════════════════════════

    public static Assessment ToDomain(this AssessmentEntity e) => new()
    {
        Id = e.Id,
        FixtureId = e.FixtureId,
        RefereeId = e.RefereeId,
        OfficialRole = e.OfficialRole.ToDomain(),
        RefereeName = e.Referee != null ? $"{e.Referee.FirstName} {e.Referee.LastName}" : "",
        RefereeCode = e.Referee?.RefCode ?? "",
        RefereeInitials = e.Referee != null ? $"{e.Referee.FirstName[0]}{e.Referee.LastName[0]}" : "",
        RefereeTier = (e.Referee?.Tier ?? Data.Enums.RefereeTier.Grassroots).ToDomain(),
        MatchTitle = e.Fixture != null ? $"{e.Fixture.HomeTeam} vs {e.Fixture.AwayTeam}" : "",
        MatchDate = e.Fixture?.Date ?? DateTime.MinValue,
        Competition = e.Fixture?.Division?.Name ?? "",
        CompetitionTier = (e.Fixture?.Division?.Tier ?? Data.Enums.RefereeTier.Grassroots).ToDomain(),
        Venue = e.Fixture?.Venue,
        Status = e.Status.ToDomain(),
        OverallScore = e.OverallScore,
        SummaryNotes = e.SummaryNotes,
        Recommendations = e.Recommendations,
        CreatedBy = e.CreatedBy,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
        Technical = e.TechnicalReport != null ? MapTechnical(e.TechnicalReport) : new(),
        Delegate = e.DelegateReport != null ? MapDelegate(e.DelegateReport) : new(),
        MatchReport = e.MatchStatReport != null ? MapMatchReport(e.MatchStatReport) : new(),
        Kmi = new KmiReport
        {
            PanelNotes = e.KmiPanelNotes,
            Incidents = e.KeyMatchIncidents?.OrderBy(k => k.SortOrder).Select(k => new KeyMatchIncident
            {
                Minute = k.Minute,
                Description = k.Description,
                DecisionType = k.DecisionType.ToDomain(),
                OriginalDecisionCorrect = k.OriginalDecisionCorrect,
                VarInterventionRequired = k.VarInterventionRequired,
                VarInterventionMade = k.VarInterventionMade,
                PanelVotesCorrect = k.PanelVotesCorrect,
                PanelVotesIncorrect = k.PanelVotesIncorrect,
                Rationale = k.Rationale,
            }).ToList() ?? new(),
        },
    };

    private static TechnicalReport MapTechnical(TechnicalReportEntity t) => new()
    {
        ReviewerName = t.ReviewerName,
        ApplicationOfLaw = new CompetencyScore { Score = (double)t.LawScore, Notes = t.LawNotes, Strengths = t.LawStrengths, AreasForImprovement = t.LawAreasForImprovement },
        PositioningAndMovement = new CompetencyScore { Score = (double)t.PositioningScore, Notes = t.PositioningNotes, Strengths = t.PositioningStrengths, AreasForImprovement = t.PositioningAreasForImprovement },
        FitnessAndWorkRate = new CompetencyScore { Score = (double)t.FitnessScore, Notes = t.FitnessNotes, Strengths = t.FitnessStrengths, AreasForImprovement = t.FitnessAreasForImprovement },
        MatchControl = new CompetencyScore { Score = (double)t.ControlScore, Notes = t.ControlNotes, Strengths = t.ControlStrengths, AreasForImprovement = t.ControlAreasForImprovement },
        OverallNotes = t.OverallNotes,
    };

    private static DelegateReport MapDelegate(DelegateReportEntity d) => new()
    {
        DelegateName = d.DelegateName,
        ManagementOfGame = new CompetencyScore { Score = (double)d.ManagementScore, Notes = d.ManagementNotes, Strengths = d.ManagementStrengths, AreasForImprovement = d.ManagementAreasForImprovement },
        Communication = new CompetencyScore { Score = (double)d.CommunicationScore, Notes = d.CommunicationNotes, Strengths = d.CommunicationStrengths, AreasForImprovement = d.CommunicationAreasForImprovement },
        Consistency = new CompetencyScore { Score = (double)d.ConsistencyScore, Notes = d.ConsistencyNotes, Strengths = d.ConsistencyStrengths, AreasForImprovement = d.ConsistencyAreasForImprovement },
        OverallNotes = d.OverallNotes,
    };

    private static Data.Models.MatchReport MapMatchReport(MatchStatReportEntity m) => new()
    {
        YellowCards = m.YellowCards,
        RedCards = m.RedCards,
        PenaltiesAwarded = m.PenaltiesAwarded,
        FoulsGiven = m.FoulsGiven,
        OffsidesCalled = m.OffsidesCalled,
        DistanceCoveredKm = (double)m.DistanceCoveredKm,
        AvgSprintSpeedKmh = (double)m.AvgSprintSpeedKmh,
        HighIntensityRuns = m.HighIntensityRuns,
        IncidentNotes = m.IncidentNotes,
        VarInteractionNotes = m.VarInteractionNotes,
    };

    // ══════════════════════════════════════════════════
    // MATCH OFFICIAL REPORT → Domain (sections list → named properties)
    // ══════════════════════════════════════════════════

    public static MatchOfficialReport ToDomain(this MatchOfficialReportEntity e)
    {
        var sections = e.Sections?.OrderBy(s => s.SortOrder).ToList() ?? new();
        ReportSection MapSection(int idx) => idx < sections.Count
            ? new ReportSection
            {
                Rating = (double)sections[idx].Rating,
                Narrative = sections[idx].Narrative,
                KeyObservations = sections[idx].Strengths,
                AreasOfConcern = sections[idx].AreasForImprovement,
            }
            : new ReportSection();

        return new MatchOfficialReport
        {
            Id = e.Id,
            FixtureId = e.FixtureId,
            RefereeId = e.RefereeId,
            OfficialRole = e.OfficialRole.ToDomain(),
            MatchTitle = e.MatchTitle,
            MatchDate = e.MatchDate,
            Venue = e.Venue,
            Competition = "",  // Hydrated from fixture division at query time
            CompetitionTier = e.CompetitionTier.ToDomain(),
            RefereeName = e.RefereeName ?? "",
            RefereeCode = e.RefereeCode ?? "",
            RefereeInitials = e.RefereeInitials ?? "",
            RefereeTier = e.RefereeTier.ToDomain(),
            HomeScore = e.HomeScore,
            AwayScore = e.AwayScore,
            Status = e.Status.ToDomain(),
            OverallRating = (double)e.OverallRating,
            AuthorName = e.AuthorName,
            Language = e.Language,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            Conclusion = e.ExecutiveSummary,
            Recommendations = e.DevelopmentPriorities,
            PerformanceSummary = MapSection(0),
            DecisionMaking = MapSection(1),
            Positioning = MapSection(2),
            Communication = MapSection(3),
            ManagementStyle = MapSection(4),
            FitnessAndWorkRate = MapSection(5),
            Events = e.Events?.OrderBy(ev => ev.Minute).ThenBy(ev => ev.AddedTime ?? 0).Select(ev => new ReportEvent
            {
                Id = ev.Id,
                Minute = ev.Minute,
                AddedTime = ev.AddedTime,
                EventType = ev.EventType.ToDomain(),
                Description = ev.Description,
                Impact = ev.Impact.ToDomain(),
                OfficialInvolved = ev.OfficialInvolved,
                OfficialResponse = ev.OfficialResponse,
                Teams = ev.Teams,
                Accuracy = ev.Accuracy.ToDomain(),
            }).ToList() ?? new(),
            VideoClips = e.VideoClips?.OrderBy(v => v.SortOrder).Select(v => new VideoClip
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Notes,
            }).ToList() ?? new(),
        };
    }

    // ══════════════════════════════════════════════════
    // INTERVENTION REVIEW → Domain
    // ══════════════════════════════════════════════════

    public static InterventionReview ToDomain(this InterventionReviewEntity e) => new()
    {
        Id = e.Id,
        ReportId = e.ReportId,
        EventId = e.EventId,
        ReviewerName = e.ReviewerName,
        ReviewerRole = e.ReviewerRole,
        AccuracyScore = e.AccuracyScore,
        ConsistencyScore = e.ConsistencyScore,
        CommunicationScore = e.CommunicationScore,
        MatchHandlingScore = e.MatchHandlingScore,
        Comment = e.Comment,
        ReviewedAt = e.ReviewedAt,
    };

    // ══════════════════════════════════════════════════
    // ACTIVITY LOG → Domain
    // ══════════════════════════════════════════════════

    public static ActivityItem ToDomain(this ActivityLogEntity e) => new()
    {
        Timestamp = e.Timestamp,
        HighlightName = e.HighlightName ?? "",
        Message = e.Message,
        HighlightValue = e.HighlightValue,
        HighlightColor = e.HighlightColor,
        IconType = e.IconType,
    };

}