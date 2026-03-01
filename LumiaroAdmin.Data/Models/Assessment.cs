using LumiaroAdmin.Models;

namespace LumiaroAdmin.Data.Models;

public enum AssessmentStatus
{
    Draft,
    Submitted,
    Reviewed,
    Finalised
}

public class Assessment
{
    public int Id { get; set; }

    // ── Links ──
    public int FixtureId { get; set; }
    public int RefereeId { get; set; }
    public OfficialRole OfficialRole { get; set; }

    // Denormalised fixture context (for display without join)
    public string MatchTitle { get; set; } = string.Empty;
    public DateTime MatchDate { get; set; }
    public string Competition { get; set; } = string.Empty;
    public RefereeTier CompetitionTier { get; set; }
    public string? Venue { get; set; }

    // Denormalised referee context
    public string RefereeName { get; set; } = string.Empty;
    public string RefereeInitials { get; set; } = string.Empty;
    public string RefereeCode { get; set; } = string.Empty;
    public RefereeTier RefereeTier { get; set; }

    // ── Metadata ──
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // ── Sections ──
    public KmiReport Kmi { get; set; } = new();
    public TechnicalReport Technical { get; set; } = new();
    public DelegateReport Delegate { get; set; } = new();
    public MatchReport MatchReport { get; set; } = new();

    // ── Overall ──
    public double OverallScore { get; set; }
    public string? SummaryNotes { get; set; }
    public string? Recommendations { get; set; }

    // ── Computed ──
    public string StatusLabel => Status switch
    {
        AssessmentStatus.Draft => "Draft",
        AssessmentStatus.Submitted => "Submitted",
        AssessmentStatus.Reviewed => "Reviewed",
        AssessmentStatus.Finalised => "Finalised",
        _ => "Unknown"
    };

    public string DateDisplay => MatchDate.ToString("dd MMM yyyy");
}

// ══════════════════════════════════════════════════
// 1. KMI PANEL REPORT — Key Match Incidents
// ══════════════════════════════════════════════════

public class KmiReport
{
    public List<KeyMatchIncident> Incidents { get; set; } = new();
    public string? PanelNotes { get; set; }
    public double AccuracyPercent => Incidents.Count > 0
        ? Math.Round(Incidents.Count(i => i.OriginalDecisionCorrect) / (double)Incidents.Count * 100, 1) : 0;
    public int TotalIncidents => Incidents.Count;
    public int CorrectDecisions => Incidents.Count(i => i.OriginalDecisionCorrect);
}

public class KeyMatchIncident
{
    public int Minute { get; set; }
    public string Description { get; set; } = string.Empty;
    public KmiDecisionType DecisionType { get; set; }
    public bool OriginalDecisionCorrect { get; set; }
    public bool VarInterventionRequired { get; set; }
    public bool VarInterventionMade { get; set; }
    public int PanelVotesCorrect { get; set; }
    public int PanelVotesIncorrect { get; set; }
    public string? Rationale { get; set; }
}

public enum KmiDecisionType
{
    Penalty,
    RedCard,
    GoalAllowed,
    GoalDisallowed,
    Offside,
    Foul,
    Handball,
    Other
}

// ══════════════════════════════════════════════════
// 2. TECHNICAL PERFORMANCE REPORT
// ══════════════════════════════════════════════════

public class TechnicalReport
{
    public string ReviewerName { get; set; } = string.Empty;
    public CompetencyScore ApplicationOfLaw { get; set; } = new();
    public CompetencyScore PositioningAndMovement { get; set; } = new();
    public CompetencyScore FitnessAndWorkRate { get; set; } = new();
    public CompetencyScore MatchControl { get; set; } = new();
    public string? OverallNotes { get; set; }

    public double AverageScore
    {
        get
        {
            var scores = new[] { ApplicationOfLaw.Score, PositioningAndMovement.Score,
                                 FitnessAndWorkRate.Score, MatchControl.Score }
                .Where(s => s > 0).ToArray();
            return scores.Length > 0 ? Math.Round(scores.Average(), 1) : 0;
        }
    }
}

// ══════════════════════════════════════════════════
// 3. MATCH DELEGATE REPORT
// ══════════════════════════════════════════════════

public class DelegateReport
{
    public string DelegateName { get; set; } = string.Empty;
    public CompetencyScore ManagementOfGame { get; set; } = new();
    public CompetencyScore Communication { get; set; } = new();
    public CompetencyScore Consistency { get; set; } = new();
    public string? OverallNotes { get; set; }

    public double AverageScore
    {
        get
        {
            var scores = new[] { ManagementOfGame.Score, Communication.Score, Consistency.Score }
                .Where(s => s > 0).ToArray();
            return scores.Length > 0 ? Math.Round(scores.Average(), 1) : 0;
        }
    }
}

// ══════════════════════════════════════════════════
// 4. MATCH REPORT — from the official
// ══════════════════════════════════════════════════

public class MatchReport
{
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int PenaltiesAwarded { get; set; }
    public int FoulsGiven { get; set; }
    public int OffsidesCalled { get; set; }
    public double DistanceCoveredKm { get; set; }
    public double AvgSprintSpeedKmh { get; set; }
    public int HighIntensityRuns { get; set; }
    public string? IncidentNotes { get; set; }
    public string? VarInteractionNotes { get; set; }
}

// ══════════════════════════════════════════════════
// SHARED — Competency Score (used in Tech + Delegate)
// ══════════════════════════════════════════════════

public class CompetencyScore
{
    public double Score { get; set; }   // 0.0 – 10.0
    public string? Notes { get; set; }
    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
}

// ══════════════════════════════════════════════════
// FORM MODEL
// ══════════════════════════════════════════════════

public class AssessmentFormModel
{
    public int? Id { get; set; }
    public int FixtureId { get; set; }
    public int RefereeId { get; set; }
    public OfficialRole OfficialRole { get; set; }

    // Technical
    public string TechnicalReviewerName { get; set; } = string.Empty;
    public double TechLawScore { get; set; }
    public string? TechLawNotes { get; set; }
    public double TechPositioningScore { get; set; }
    public string? TechPositioningNotes { get; set; }
    public double TechFitnessScore { get; set; }
    public string? TechFitnessNotes { get; set; }
    public double TechControlScore { get; set; }
    public string? TechControlNotes { get; set; }
    public string? TechOverallNotes { get; set; }

    // Delegate
    public string DelegateName { get; set; } = string.Empty;
    public double DelManagementScore { get; set; }
    public string? DelManagementNotes { get; set; }
    public double DelCommunicationScore { get; set; }
    public string? DelCommunicationNotes { get; set; }
    public double DelConsistencyScore { get; set; }
    public string? DelConsistencyNotes { get; set; }
    public string? DelOverallNotes { get; set; }

    // Match stats
    public int Yellows { get; set; }
    public int Reds { get; set; }
    public int Penalties { get; set; }
    public int Fouls { get; set; }
    public int Offsides { get; set; }
    public double DistanceKm { get; set; }
    public double AvgSprintSpeed { get; set; }
    public int HighIntensityRuns { get; set; }
    public string? IncidentNotes { get; set; }
    public string? VarNotes { get; set; }

    // Overall
    public double OverallScore { get; set; }
    public string? SummaryNotes { get; set; }
    public string? Recommendations { get; set; }
    public AssessmentStatus Status { get; set; } = AssessmentStatus.Draft;

    public bool IsEdit => Id.HasValue;

    public Assessment ToAssessment() => new()
    {
        Id = Id ?? 0,
        FixtureId = FixtureId,
        RefereeId = RefereeId,
        OfficialRole = OfficialRole,
        Status = Status,
        OverallScore = OverallScore,
        SummaryNotes = SummaryNotes,
        Recommendations = Recommendations,
        Technical = new TechnicalReport
        {
            ReviewerName = TechnicalReviewerName,
            ApplicationOfLaw = new CompetencyScore { Score = TechLawScore, Notes = TechLawNotes },
            PositioningAndMovement = new CompetencyScore { Score = TechPositioningScore, Notes = TechPositioningNotes },
            FitnessAndWorkRate = new CompetencyScore { Score = TechFitnessScore, Notes = TechFitnessNotes },
            MatchControl = new CompetencyScore { Score = TechControlScore, Notes = TechControlNotes },
            OverallNotes = TechOverallNotes
        },
        Delegate = new DelegateReport
        {
            DelegateName = DelegateName,
            ManagementOfGame = new CompetencyScore { Score = DelManagementScore, Notes = DelManagementNotes },
            Communication = new CompetencyScore { Score = DelCommunicationScore, Notes = DelCommunicationNotes },
            Consistency = new CompetencyScore { Score = DelConsistencyScore, Notes = DelConsistencyNotes },
            OverallNotes = DelOverallNotes
        },
        MatchReport = new MatchReport
        {
            YellowCards = Yellows, RedCards = Reds, PenaltiesAwarded = Penalties,
            FoulsGiven = Fouls, OffsidesCalled = Offsides,
            DistanceCoveredKm = DistanceKm, AvgSprintSpeedKmh = AvgSprintSpeed,
            HighIntensityRuns = HighIntensityRuns,
            IncidentNotes = IncidentNotes, VarInteractionNotes = VarNotes
        }
    };

    public static AssessmentFormModel FromAssessment(Assessment a) => new()
    {
        Id = a.Id,
        FixtureId = a.FixtureId,
        RefereeId = a.RefereeId,
        OfficialRole = a.OfficialRole,
        Status = a.Status,
        OverallScore = a.OverallScore,
        SummaryNotes = a.SummaryNotes,
        Recommendations = a.Recommendations,
        TechnicalReviewerName = a.Technical.ReviewerName,
        TechLawScore = a.Technical.ApplicationOfLaw.Score,
        TechLawNotes = a.Technical.ApplicationOfLaw.Notes,
        TechPositioningScore = a.Technical.PositioningAndMovement.Score,
        TechPositioningNotes = a.Technical.PositioningAndMovement.Notes,
        TechFitnessScore = a.Technical.FitnessAndWorkRate.Score,
        TechFitnessNotes = a.Technical.FitnessAndWorkRate.Notes,
        TechControlScore = a.Technical.MatchControl.Score,
        TechControlNotes = a.Technical.MatchControl.Notes,
        TechOverallNotes = a.Technical.OverallNotes,
        DelegateName = a.Delegate.DelegateName,
        DelManagementScore = a.Delegate.ManagementOfGame.Score,
        DelManagementNotes = a.Delegate.ManagementOfGame.Notes,
        DelCommunicationScore = a.Delegate.Communication.Score,
        DelCommunicationNotes = a.Delegate.Communication.Notes,
        DelConsistencyScore = a.Delegate.Consistency.Score,
        DelConsistencyNotes = a.Delegate.Consistency.Notes,
        DelOverallNotes = a.Delegate.OverallNotes,
        Yellows = a.MatchReport.YellowCards,
        Reds = a.MatchReport.RedCards,
        Penalties = a.MatchReport.PenaltiesAwarded,
        Fouls = a.MatchReport.FoulsGiven,
        Offsides = a.MatchReport.OffsidesCalled,
        DistanceKm = a.MatchReport.DistanceCoveredKm,
        AvgSprintSpeed = a.MatchReport.AvgSprintSpeedKmh,
        HighIntensityRuns = a.MatchReport.HighIntensityRuns,
        IncidentNotes = a.MatchReport.IncidentNotes,
        VarNotes = a.MatchReport.VarInteractionNotes,
    };
}
