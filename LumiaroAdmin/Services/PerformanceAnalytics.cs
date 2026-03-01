using LumiaroAdmin.Data.Models;

namespace LumiaroAdmin.Models;

// ══════════════════════════════════════════════════
// FLEET-WIDE ANALYTICS DASHBOARD
// ══════════════════════════════════════════════════

public class FleetAnalytics
{
    public int TotalOfficials { get; set; }
    public int TotalMatchesAssessed { get; set; }
    public int TotalReportsPublished { get; set; }
    public double AverageRating { get; set; }
    public double AverageAccuracy { get; set; }
    public int CriticalIncidents { get; set; }

    // Distribution
    public Dictionary<RefereeTier, int> OfficialsByTier { get; set; } = new();
    public Dictionary<RefereeTier, double> AvgRatingByTier { get; set; } = new();

    // Season trend (monthly averages)
    public List<MonthlyTrend> MonthlyTrends { get; set; } = new();

    // Leaderboard
    public List<RefereeRankEntry> TopPerformers { get; set; } = new();
    public List<RefereeRankEntry> MostImproved { get; set; } = new();
    public List<RefereeRankEntry> ConcerningPerformers { get; set; } = new();
    public List<RefereeRankEntry> AllRankings { get; set; } = new();
}

public class MonthlyTrend
{
    public string Month { get; set; } = string.Empty;       // "Sep", "Oct", etc.
    public int MonthIndex { get; set; }                      // 1-12
    public double AvgRating { get; set; }
    public double AvgAccuracy { get; set; }
    public int MatchCount { get; set; }
    public double CardsPerGame { get; set; }
}

public class RefereeRankEntry
{
    public int RefereeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public RefereeTier Tier { get; set; }
    public RefereeStatus Status { get; set; }
    public OfficialRole PrimaryRole { get; set; }
    public int MatchCount { get; set; }
    public double AvgRating { get; set; }
    public double ConsistencyIndex { get; set; }    // σ of match ratings (lower = better)
    public double AccuracyPercent { get; set; }
    public double CardsPerGame { get; set; }
    public double TrendChange { get; set; }         // Last 3 vs previous 3 match avg
    public int Rank { get; set; }
    public double Percentile { get; set; }          // Within tier
    public string TierReadiness { get; set; } = string.Empty; // "Ready", "Developing", "Not Ready"
}

// ══════════════════════════════════════════════════
// INDIVIDUAL REFEREE ANALYTICS PROFILE
// ══════════════════════════════════════════════════

public class RefereeAnalyticsProfile
{
    // ── Identity ──
    public int RefereeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public RefereeTier Tier { get; set; }
    public RefereeStatus Status { get; set; }

    // ── Season Summary ──
    public int MatchesThisSeason { get; set; }
    public double SeasonAvgRating { get; set; }
    public double ConsistencyIndex { get; set; }
    public double RecentFormAvg { get; set; }       // Last 5 matches
    public double TrendChange { get; set; }         // Last 3 vs previous 3
    public double AccuracyPercent { get; set; }     // From KMI data
    public int Rank { get; set; }
    public int TotalInTier { get; set; }
    public double Percentile { get; set; }

    // ── Competency Radar ──
    public CompetencyRadarData Competencies { get; set; } = new();

    // ── Match-by-Match Form ──
    public List<MatchFormEntry> MatchForm { get; set; } = new();

    // ── Event Statistics ──
    public EventStatistics EventStats { get; set; } = new();

    // ── Half-by-Half Analysis ──
    public HalfByHalfAnalysis HalfAnalysis { get; set; } = new();

    // ── Decision Accuracy Breakdown ──
    public DecisionAccuracyBreakdown DecisionAccuracy { get; set; } = new();

    // ── Bias Indicators ──
    public BiasIndicators Bias { get; set; } = new();

    // ── Physical Performance ──
    public PhysicalPerformance Physical { get; set; } = new();

    // ── Tier Readiness ──
    public TierReadinessAssessment TierReadiness { get; set; } = new();
}

// ── Competency Radar ──

public class CompetencyRadarData
{
    // From Reports (6 dimensions)
    public double PerformanceSummary { get; set; }
    public double DecisionMaking { get; set; }
    public double Positioning { get; set; }
    public double Communication { get; set; }
    public double ManagementStyle { get; set; }
    public double FitnessAndWorkRate { get; set; }

    // From Technical Assessments (4 dimensions)
    public double ApplicationOfLaw { get; set; }
    public double MatchControl { get; set; }

    // From Delegate Assessments (3 dimensions)
    public double Consistency { get; set; }

    public double OverallAverage
    {
        get
        {
            var scores = new[] { PerformanceSummary, DecisionMaking, Positioning,
                Communication, ManagementStyle, FitnessAndWorkRate }
                .Where(s => s > 0).ToArray();
            return scores.Length > 0 ? Math.Round(scores.Average(), 1) : 0;
        }
    }

    /// <summary>Returns the 6 primary radar dimensions as label/value pairs.</summary>
    public List<(string Label, double Value)> GetRadarPoints() => new()
    {
        ("Decision Making", DecisionMaking),
        ("Positioning", Positioning),
        ("Communication", Communication),
        ("Game Management", ManagementStyle),
        ("Fitness", FitnessAndWorkRate),
        ("Law Application", ApplicationOfLaw > 0 ? ApplicationOfLaw : PerformanceSummary)
    };
}

// ── Match Form ──

public class MatchFormEntry
{
    public int ReportId { get; set; }
    public int? AssessmentId { get; set; }
    public DateTime MatchDate { get; set; }
    public string MatchTitle { get; set; } = string.Empty;
    public string Competition { get; set; } = string.Empty;
    public RefereeTier CompetitionTier { get; set; }
    public double OverallRating { get; set; }
    public double? AccuracyPercent { get; set; }    // From KMI if available
    public int EventCount { get; set; }
    public int CriticalEventCount { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public string DateDisplay => MatchDate.ToString("dd MMM");
}

// ── Event Statistics ──

public class EventStatistics
{
    public int TotalMatches { get; set; }
    public int TotalYellowCards { get; set; }
    public int TotalRedCards { get; set; }
    public int TotalPenalties { get; set; }
    public int TotalFouls { get; set; }
    public int TotalOffsides { get; set; }
    public int TotalVarReviews { get; set; }
    public int TotalCriticalIncidents { get; set; }

    // Per-game averages
    public double YellowsPerGame => TotalMatches > 0 ? Math.Round((double)TotalYellowCards / TotalMatches, 2) : 0;
    public double RedsPerGame => TotalMatches > 0 ? Math.Round((double)TotalRedCards / TotalMatches, 2) : 0;
    public double PenaltiesPerGame => TotalMatches > 0 ? Math.Round((double)TotalPenalties / TotalMatches, 2) : 0;
    public double FoulsPerGame => TotalMatches > 0 ? Math.Round((double)TotalFouls / TotalMatches, 2) : 0;
    public double FoulsPerCard => (TotalYellowCards + TotalRedCards) > 0
        ? Math.Round((double)TotalFouls / (TotalYellowCards + TotalRedCards), 1) : 0;
    public double CriticalPerGame => TotalMatches > 0 ? Math.Round((double)TotalCriticalIncidents / TotalMatches, 2) : 0;

    // Event type distribution
    public Dictionary<EventType, int> EventDistribution { get; set; } = new();
}

// ── Half-by-Half ──

public class HalfByHalfAnalysis
{
    public int FirstHalfEvents { get; set; }
    public int SecondHalfEvents { get; set; }
    public int FirstHalfCards { get; set; }
    public int SecondHalfCards { get; set; }
    public int FirstHalfCritical { get; set; }
    public int SecondHalfCritical { get; set; }
    public double FirstHalfCardRate => FirstHalfEvents > 0
        ? Math.Round((double)FirstHalfCards / FirstHalfEvents * 100, 1) : 0;
    public double SecondHalfCardRate => SecondHalfEvents > 0
        ? Math.Round((double)SecondHalfCards / SecondHalfEvents * 100, 1) : 0;

    public string DriftIndicator
    {
        get
        {
            if (SecondHalfCritical > FirstHalfCritical * 1.5) return "Significant 2nd-half drift";
            if (SecondHalfCards > FirstHalfCards * 1.3) return "Moderate 2nd-half increase";
            return "Stable across halves";
        }
    }

    public string DriftSeverity
    {
        get
        {
            if (SecondHalfCritical > FirstHalfCritical * 1.5) return "warning";
            if (SecondHalfCards > FirstHalfCards * 1.3) return "caution";
            return "good";
        }
    }
}

// ── Decision Accuracy ──

public class DecisionAccuracyBreakdown
{
    public int TotalDecisionsReviewed { get; set; }
    public int CorrectDecisions { get; set; }
    public double OverallAccuracy => TotalDecisionsReviewed > 0
        ? Math.Round((double)CorrectDecisions / TotalDecisionsReviewed * 100, 1) : 0;

    public Dictionary<KmiDecisionType, (int Total, int Correct)> ByType { get; set; } = new();

    public double AccuracyForType(KmiDecisionType type) =>
        ByType.TryGetValue(type, out var data) && data.Total > 0
            ? Math.Round((double)data.Correct / data.Total * 100, 1) : 0;
}

// ── Bias Indicators ──

public class BiasIndicators
{
    public int HomeYellows { get; set; }
    public int AwayYellows { get; set; }
    public int HomeReds { get; set; }
    public int AwayReds { get; set; }
    public int HomePenalties { get; set; }
    public int AwayPenalties { get; set; }
    public int HomeFouls { get; set; }
    public int AwayFouls { get; set; }

    public double YellowBiasRatio => (HomeYellows + AwayYellows) > 0
        ? Math.Round((double)HomeYellows / (HomeYellows + AwayYellows) * 100, 1) : 50;
    public double PenaltyBiasRatio => (HomePenalties + AwayPenalties) > 0
        ? Math.Round((double)HomePenalties / (HomePenalties + AwayPenalties) * 100, 1) : 50;

    public string BiasAssessment
    {
        get
        {
            var deviation = Math.Abs(YellowBiasRatio - 50);
            if (deviation > 20) return "Significant imbalance detected";
            if (deviation > 10) return "Moderate imbalance";
            return "Within expected range";
        }
    }

    public string BiasSeverity
    {
        get
        {
            var deviation = Math.Abs(YellowBiasRatio - 50);
            if (deviation > 20) return "warning";
            if (deviation > 10) return "caution";
            return "good";
        }
    }
}

// ── Physical Performance ──

public class PhysicalPerformance
{
    public double AvgDistanceKm { get; set; }
    public double AvgSprintSpeed { get; set; }
    public int AvgHighIntensityRuns { get; set; }
    public double DistanceTrend { get; set; }   // Change vs previous period

    // Per-match breakdown
    public List<(string Match, double DistanceKm, double SprintSpeed, int HiRuns)> MatchBreakdown { get; set; } = new();
}

// ── Tier Readiness ──

public class TierReadinessAssessment
{
    public RefereeTier CurrentTier { get; set; }
    public RefereeTier TargetTier { get; set; }
    public double ReadinessScore { get; set; }   // 0-100%
    public string ReadinessLabel { get; set; } = string.Empty;   // Ready / Developing / Not Ready
    public string ReadinessColor { get; set; } = string.Empty;

    public List<ReadinessCriteria> Criteria { get; set; } = new();
}

public class ReadinessCriteria
{
    public string Name { get; set; } = string.Empty;
    public double RequiredScore { get; set; }
    public double ActualScore { get; set; }
    public bool Met => ActualScore >= RequiredScore;
    public string Status => Met ? "Met" : "Below threshold";
}

// ══════════════════════════════════════════════════
// DECISION ACCURACY ENUM (added to ReportEvent)
// ══════════════════════════════════════════════════

public enum DecisionAccuracy
{
    Correct,
    Incorrect,
    Debatable,
    NotApplicable
}
