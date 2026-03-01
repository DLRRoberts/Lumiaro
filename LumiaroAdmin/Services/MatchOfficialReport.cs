namespace LumiaroAdmin.Models;

// ══════════════════════════════════════════════════
// MATCH REPORT STATUS
// ══════════════════════════════════════════════════

public enum ReportStatus
{
    Draft,
    Submitted,
    Published
}

public enum EventType
{
    Goal,
    Foul,
    YellowCard,
    RedCard,
    Penalty,
    Offside,
    VarReview,
    Substitution,
    Injury,
    Stoppage,
    Other
}

public enum EventImpact
{
    Low,
    Medium,
    High,
    Critical
}

// ══════════════════════════════════════════════════
// MATCH REPORT — Full narrative report on an official
// ══════════════════════════════════════════════════

public class MatchOfficialReport
{
    public int Id { get; set; }

    // ── Links ──
    public int FixtureId { get; set; }
    public int RefereeId { get; set; }
    public OfficialRole OfficialRole { get; set; }

    // ── Denormalised context ──
    public string MatchTitle { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public string Competition { get; set; } = string.Empty;
    public RefereeTier CompetitionTier { get; set; }
    public string? Venue { get; set; }
    public string RefereeName { get; set; } = string.Empty;
    public string RefereeInitials { get; set; } = string.Empty;
    public string RefereeCode { get; set; } = string.Empty;
    public RefereeTier RefereeTier { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    
    public string Language { get; set; }

    // ── Metadata ──
    public ReportStatus Status { get; set; } = ReportStatus.Draft;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }

    // ── Overall Rating ──
    public double OverallRating { get; set; }

    // ── Narrative Sections ──
    public ReportSection PerformanceSummary { get; set; } = new();
    public ReportSection DecisionMaking { get; set; } = new();
    public ReportSection Positioning { get; set; } = new();
    public ReportSection Communication { get; set; } = new();
    public ReportSection ManagementStyle { get; set; } = new();
    public ReportSection FitnessAndWorkRate { get; set; } = new();

    // ── Event Timeline ──
    public List<ReportEvent> Events { get; set; } = new();

    // ── Video Clips ──
    public List<VideoClip> VideoClips { get; set; } = new();

    // ── Conclusion ──
    public string? Conclusion { get; set; }
    public string? Recommendations { get; set; }
    public string? ConfidentialNotes { get; set; }

    // ── Computed ──
    public string StatusLabel => Status switch
    {
        ReportStatus.Draft => "Draft",
        ReportStatus.Submitted => "Submitted",
        ReportStatus.Published => "Published",
        _ => "Unknown"
    };

    public string DateDisplay => MatchDate.ToString("dd MMM yyyy");
    public string ScoreDisplay => HomeScore.HasValue && AwayScore.HasValue
        ? $"{HomeScore} — {AwayScore}" : "—";

    public int CriticalEventCount => Events.Count(e => e.Impact == EventImpact.Critical);
    public int HighEventCount => Events.Count(e => e.Impact == EventImpact.High);
    public int TotalEvents => Events.Count;

    public double SectionAverageRating
    {
        get
        {
            var scores = new[]
            {
                PerformanceSummary.Rating, DecisionMaking.Rating,
                Positioning.Rating, Communication.Rating,
                ManagementStyle.Rating, FitnessAndWorkRate.Rating
            }.Where(s => s > 0).ToArray();
            return scores.Length > 0 ? Math.Round(scores.Average(), 1) : 0;
        }
    }
}

// ══════════════════════════════════════════════════
// REPORT SECTION — Rated narrative block
// ══════════════════════════════════════════════════

public class ReportSection
{
    public double Rating { get; set; }         // 0.0 – 10.0
    public string? Narrative { get; set; }     // Main commentary text
    public string? KeyObservations { get; set; } // Bullet-point style observations
    public string? AreasOfConcern { get; set; }  // Issues noted
}

// ══════════════════════════════════════════════════
// REPORT EVENT — Timeline entry for a match incident
// ══════════════════════════════════════════════════

public class ReportEvent
{
    public int Id { get; set; }
    public int Minute { get; set; }
    public int? AddedTime { get; set; }   // e.g. 45+2
    public EventType EventType { get; set; }
    public string Description { get; set; } = string.Empty;
    public EventImpact Impact { get; set; } = EventImpact.Medium;
    public bool OfficialInvolved { get; set; }
    public string? OfficialResponse { get; set; }
    public string? Teams { get; set; }    // Which team involved e.g. "Home" / "Away"
    public DecisionAccuracy Accuracy { get; set; } = DecisionAccuracy.NotApplicable;

    public string MinuteDisplay => AddedTime.HasValue
        ? $"{Minute}+{AddedTime}'" : $"{Minute}'";

    public string EventTypeLabel => EventType switch
    {
        EventType.Goal => "Goal",
        EventType.Foul => "Foul",
        EventType.YellowCard => "Yellow Card",
        EventType.RedCard => "Red Card",
        EventType.Penalty => "Penalty",
        EventType.Offside => "Offside",
        EventType.VarReview => "VAR Review",
        EventType.Substitution => "Substitution",
        EventType.Injury => "Injury",
        EventType.Stoppage => "Stoppage",
        EventType.Other => "Other",
        _ => "—"
    };

    public string ImpactLabel => Impact switch
    {
        EventImpact.Low => "Low",
        EventImpact.Medium => "Medium",
        EventImpact.High => "High",
        EventImpact.Critical => "Critical",
        _ => "—"
    };
}

// ══════════════════════════════════════════════════
// VIDEO CLIP — Attached video reference
// ══════════════════════════════════════════════════

public class VideoClip
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = "video/mp4";
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? MatchMinute { get; set; }
    public string? LinkedEventDescription { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public string FileSizeDisplay
    {
        get
        {
            if (FileSize < 1024) return $"{FileSize} B";
            if (FileSize < 1048576) return $"{FileSize / 1024.0:F1} KB";
            return $"{FileSize / 1048576.0:F1} MB";
        }
    }

    public string MinuteDisplay => MatchMinute.HasValue ? $"{MatchMinute}'" : "—";
}

// ══════════════════════════════════════════════════
// FORM MODEL — Flattened for form binding
// ══════════════════════════════════════════════════

public class MatchReportFormModel
{
    public int? Id { get; set; }
    public int FixtureId { get; set; }
    public int RefereeId { get; set; }
    public OfficialRole OfficialRole { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Draft;
    public string AuthorName { get; set; } = string.Empty;

    // Overall
    public double OverallRating { get; set; }

    // Sections — flattened
    public double SummaryRating { get; set; }
    public string? SummaryNarrative { get; set; }
    public string? SummaryKeyObs { get; set; }
    public string? SummaryConcerns { get; set; }

    public double DecisionRating { get; set; }
    public string? DecisionNarrative { get; set; }
    public string? DecisionKeyObs { get; set; }
    public string? DecisionConcerns { get; set; }

    public double PositioningRating { get; set; }
    public string? PositioningNarrative { get; set; }
    public string? PositioningKeyObs { get; set; }
    public string? PositioningConcerns { get; set; }

    public double CommunicationRating { get; set; }
    public string? CommunicationNarrative { get; set; }
    public string? CommunicationKeyObs { get; set; }
    public string? CommunicationConcerns { get; set; }

    public double ManagementRating { get; set; }
    public string? ManagementNarrative { get; set; }
    public string? ManagementKeyObs { get; set; }
    public string? ManagementConcerns { get; set; }

    public double FitnessRating { get; set; }
    public string? FitnessNarrative { get; set; }
    public string? FitnessKeyObs { get; set; }
    public string? FitnessConcerns { get; set; }

    // Score context
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }

    // Conclusion
    public string? Conclusion { get; set; }
    public string? Recommendations { get; set; }
    public string? ConfidentialNotes { get; set; }

    public bool IsEdit => Id.HasValue;

    public MatchOfficialReport ToReport() => new()
    {
        Id = Id ?? 0,
        FixtureId = FixtureId,
        RefereeId = RefereeId,
        OfficialRole = OfficialRole,
        Status = Status,
        AuthorName = AuthorName,
        OverallRating = OverallRating,
        HomeScore = HomeScore,
        AwayScore = AwayScore,
        Conclusion = Conclusion,
        Recommendations = Recommendations,
        ConfidentialNotes = ConfidentialNotes,
        PerformanceSummary = new ReportSection { Rating = SummaryRating, Narrative = SummaryNarrative, KeyObservations = SummaryKeyObs, AreasOfConcern = SummaryConcerns },
        DecisionMaking = new ReportSection { Rating = DecisionRating, Narrative = DecisionNarrative, KeyObservations = DecisionKeyObs, AreasOfConcern = DecisionConcerns },
        Positioning = new ReportSection { Rating = PositioningRating, Narrative = PositioningNarrative, KeyObservations = PositioningKeyObs, AreasOfConcern = PositioningConcerns },
        Communication = new ReportSection { Rating = CommunicationRating, Narrative = CommunicationNarrative, KeyObservations = CommunicationKeyObs, AreasOfConcern = CommunicationConcerns },
        ManagementStyle = new ReportSection { Rating = ManagementRating, Narrative = ManagementNarrative, KeyObservations = ManagementKeyObs, AreasOfConcern = ManagementConcerns },
        FitnessAndWorkRate = new ReportSection { Rating = FitnessRating, Narrative = FitnessNarrative, KeyObservations = FitnessKeyObs, AreasOfConcern = FitnessConcerns },
    };

    public static MatchReportFormModel FromReport(MatchOfficialReport r) => new()
    {
        Id = r.Id,
        FixtureId = r.FixtureId,
        RefereeId = r.RefereeId,
        OfficialRole = r.OfficialRole,
        Status = r.Status,
        AuthorName = r.AuthorName,
        OverallRating = r.OverallRating,
        HomeScore = r.HomeScore,
        AwayScore = r.AwayScore,
        Conclusion = r.Conclusion,
        Recommendations = r.Recommendations,
        ConfidentialNotes = r.ConfidentialNotes,
        SummaryRating = r.PerformanceSummary.Rating, SummaryNarrative = r.PerformanceSummary.Narrative, SummaryKeyObs = r.PerformanceSummary.KeyObservations, SummaryConcerns = r.PerformanceSummary.AreasOfConcern,
        DecisionRating = r.DecisionMaking.Rating, DecisionNarrative = r.DecisionMaking.Narrative, DecisionKeyObs = r.DecisionMaking.KeyObservations, DecisionConcerns = r.DecisionMaking.AreasOfConcern,
        PositioningRating = r.Positioning.Rating, PositioningNarrative = r.Positioning.Narrative, PositioningKeyObs = r.Positioning.KeyObservations, PositioningConcerns = r.Positioning.AreasOfConcern,
        CommunicationRating = r.Communication.Rating, CommunicationNarrative = r.Communication.Narrative, CommunicationKeyObs = r.Communication.KeyObservations, CommunicationConcerns = r.Communication.AreasOfConcern,
        ManagementRating = r.ManagementStyle.Rating, ManagementNarrative = r.ManagementStyle.Narrative, ManagementKeyObs = r.ManagementStyle.KeyObservations, ManagementConcerns = r.ManagementStyle.AreasOfConcern,
        FitnessRating = r.FitnessAndWorkRate.Rating, FitnessNarrative = r.FitnessAndWorkRate.Narrative, FitnessKeyObs = r.FitnessAndWorkRate.KeyObservations, FitnessConcerns = r.FitnessAndWorkRate.AreasOfConcern,
    };
}
