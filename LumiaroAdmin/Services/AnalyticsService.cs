using LumiaroAdmin.Data.Models;
using LumiaroAdmin.Models;

namespace LumiaroAdmin.Services;

public class AnalyticsService
{
    private readonly RefereeService _refereeService;
    private readonly AssessmentService _assessmentService;
    private readonly MatchReportService _reportService;

    public AnalyticsService(
        RefereeService refereeService,
        AssessmentService assessmentService,
        MatchReportService reportService)
    {
        _refereeService = refereeService;
        _assessmentService = assessmentService;
        _reportService = reportService;
    }

    // ══════════════════════════════════════════════════
    // FLEET ANALYTICS — Dashboard overview
    // ══════════════════════════════════════════════════

    public async Task<FleetAnalytics> GetFleetAnalyticsAsync()
    {
        var allReferees = await _refereeService.GetAllRefereesAsync();
        var allAssessments = await _assessmentService.GetAllAsync();
        var allReports = await _reportService.GetAllAsync();
        var publishedReports = allReports.Where(r => r.Status == ReportStatus.Published).ToList();

        var fleet = new FleetAnalytics
        {
            TotalOfficials = allReferees.Count(r => r.Status == RefereeStatus.Active),
            TotalMatchesAssessed = allAssessments.Count,
            TotalReportsPublished = publishedReports.Count,
            AverageRating = allReports.Count > 0
                ? Math.Round(allReports.Average(r => r.OverallRating), 1) : 0,
            AverageAccuracy = allAssessments.Count > 0 && allAssessments.Any(a => a.Kmi.TotalIncidents > 0)
                ? Math.Round(allAssessments.Where(a => a.Kmi.TotalIncidents > 0).Average(a => a.Kmi.AccuracyPercent), 1) : 0,
            CriticalIncidents = allReports.Sum(r => r.CriticalEventCount),
        };

        fleet.OfficialsByTier = allReferees
            .Where(r => r.Status == RefereeStatus.Active)
            .GroupBy(r => r.Tier)
            .ToDictionary(g => g.Key, g => g.Count());

        fleet.AvgRatingByTier = allReports
            .GroupBy(r => r.CompetitionTier)
            .ToDictionary(g => g.Key, g => Math.Round(g.Average(r => r.OverallRating), 1));

        fleet.MonthlyTrends = BuildMonthlyTrends(allReports);
        fleet.AllRankings = BuildAllRankings(allReferees, allReports, allAssessments);

        var ranked = fleet.AllRankings.Where(r => r.MatchCount >= 2).OrderByDescending(r => r.AvgRating).ToList();
        fleet.TopPerformers = ranked.Take(5).ToList();
        fleet.ConcerningPerformers = ranked.OrderBy(r => r.AvgRating).Take(5).ToList();
        fleet.MostImproved = ranked.OrderByDescending(r => r.TrendChange).Take(5).ToList();

        return fleet;
    }

    // ══════════════════════════════════════════════════
    // INDIVIDUAL REFEREE PROFILE
    // ══════════════════════════════════════════════════

    public async Task<RefereeAnalyticsProfile?> GetRefereeProfileAsync(int refereeId)
    {
        var referee = await _refereeService.GetRefereeByIdAsync(refereeId);
        if (referee == null) return null;

        var reports = (await _reportService.GetByRefereeAsync(refereeId))
            .OrderBy(r => r.MatchDate).ToList();
        var assessments = (await _assessmentService.GetByRefereeAsync(refereeId))
            .OrderBy(a => a.MatchDate).ToList();

        var allReports = await _reportService.GetAllAsync();
        var allReferees = await _refereeService.GetAllRefereesAsync();

        var profile = new RefereeAnalyticsProfile
        {
            RefereeId = referee.Id,
            Name = referee.FullName,
            Initials = referee.Initials,
            Code = referee.RefCode,
            Tier = referee.Tier,
            Status = referee.Status,
        };

        profile.MatchesThisSeason = reports.Count;
        profile.SeasonAvgRating = reports.Count > 0
            ? Math.Round(reports.Average(r => r.OverallRating), 1) : 0;
        profile.ConsistencyIndex = reports.Count >= 2
            ? Math.Round(CalculateStdDev(reports.Select(r => r.OverallRating)), 2) : 0;
        profile.RecentFormAvg = reports.Count > 0
            ? Math.Round(reports.TakeLast(Math.Min(5, reports.Count)).Average(r => r.OverallRating), 1) : 0;
        profile.TrendChange = CalculateTrendChange(reports.Select(r => r.OverallRating).ToList());
        profile.AccuracyPercent = assessments.Count > 0 && assessments.Any(a => a.Kmi.TotalIncidents > 0)
            ? Math.Round(assessments.Where(a => a.Kmi.TotalIncidents > 0).Average(a => a.Kmi.AccuracyPercent), 1) : 0;

        var tierPeers = BuildTierRankings(referee.Tier, allReferees, allReports);
        var myRank = tierPeers.FirstOrDefault(r => r.RefereeId == refereeId);
        profile.Rank = myRank?.Rank ?? 0;
        profile.TotalInTier = tierPeers.Count;
        profile.Percentile = myRank?.Percentile ?? 0;

        profile.Competencies = BuildCompetencyRadar(reports, assessments);
        profile.MatchForm = BuildMatchForm(reports, assessments);
        profile.EventStats = BuildEventStatistics(reports, assessments);
        profile.HalfAnalysis = BuildHalfAnalysis(reports);
        profile.DecisionAccuracy = BuildDecisionAccuracy(assessments);
        profile.Bias = BuildBiasIndicators(reports);
        profile.Physical = BuildPhysicalPerformance(assessments);
        profile.TierReadiness = BuildTierReadiness(referee, profile);

        return profile;
    }

    // ══════════════════════════════════════════════════
    // COMPARISON
    // ══════════════════════════════════════════════════

    public async Task<List<RefereeAnalyticsProfile>> GetComparisonAsync(List<int> refereeIds)
    {
        var profiles = new List<RefereeAnalyticsProfile>();
        foreach (var id in refereeIds)
        {
            var p = await GetRefereeProfileAsync(id);
            if (p != null) profiles.Add(p);
        }
        return profiles;
    }

    // ══════════════════════════════════════════════════
    // RANKINGS TABLE
    // ══════════════════════════════════════════════════

    public async Task<List<RefereeRankEntry>> GetRankingsAsync(RefereeTier? tierFilter = null, string? sortBy = null)
    {
        var allReferees = await _refereeService.GetAllRefereesAsync();
        var allReports = await _reportService.GetAllAsync();
        var allAssessments = await _assessmentService.GetAllAsync();

        var rankings = BuildAllRankings(allReferees, allReports, allAssessments);

        if (tierFilter.HasValue)
            rankings = rankings.Where(r => r.Tier == tierFilter.Value).ToList();

        rankings = (sortBy ?? "rating") switch
        {
            "consistency" => rankings.OrderBy(r => r.ConsistencyIndex).ToList(),
            "accuracy" => rankings.OrderByDescending(r => r.AccuracyPercent).ToList(),
            "trend" => rankings.OrderByDescending(r => r.TrendChange).ToList(),
            "cards" => rankings.OrderBy(r => r.CardsPerGame).ToList(),
            "matches" => rankings.OrderByDescending(r => r.MatchCount).ToList(),
            _ => rankings.OrderByDescending(r => r.AvgRating).ToList()
        };

        for (int i = 0; i < rankings.Count; i++)
            rankings[i].Rank = i + 1;

        return rankings;
    }

    // ══════════════════════════════════════════════════
    // PRIVATE BUILDERS — operate on pre-fetched data
    // ══════════════════════════════════════════════════

    private List<RefereeRankEntry> BuildAllRankings(
        List<Referee> referees, List<MatchOfficialReport> reports, List<Assessment> assessments)
    {
        var rankings = new List<RefereeRankEntry>();

        foreach (var referee in referees.Where(r => r.Status == RefereeStatus.Active))
        {
            var refReports = reports.Where(r => r.RefereeId == referee.Id).ToList();
            var refAssessments = assessments.Where(a => a.RefereeId == referee.Id).ToList();

            if (refReports.Count == 0 && refAssessments.Count == 0) continue;

            var ratings = refReports.Select(r => r.OverallRating).ToList();
            var matchCount = refReports.Count;

            var entry = new RefereeRankEntry
            {
                RefereeId = referee.Id, Name = referee.FullName, Initials = referee.Initials,
                Code = referee.RefCode, Tier = referee.Tier, Status = referee.Status,
                PrimaryRole = OfficialRole.Referee, MatchCount = matchCount,
                AvgRating = ratings.Count > 0 ? Math.Round(ratings.Average(), 1) : 0,
                ConsistencyIndex = ratings.Count >= 2 ? Math.Round(CalculateStdDev(ratings), 2) : 0,
                AccuracyPercent = refAssessments.Any(a => a.Kmi.TotalIncidents > 0)
                    ? Math.Round(refAssessments.Where(a => a.Kmi.TotalIncidents > 0)
                        .Average(a => a.Kmi.AccuracyPercent), 1) : 0,
                CardsPerGame = matchCount > 0
                    ? Math.Round((double)refReports.Sum(r =>
                        r.Events.Count(e => e.EventType == EventType.YellowCard || e.EventType == EventType.RedCard))
                        / matchCount, 2) : 0,
                TrendChange = CalculateTrendChange(ratings),
            };

            entry.TierReadiness = entry.AvgRating >= 8.0 ? "Ready"
                : entry.AvgRating >= 7.0 ? "Developing" : "Not Ready";

            rankings.Add(entry);
        }

        rankings = rankings.OrderByDescending(r => r.AvgRating).ToList();
        for (int i = 0; i < rankings.Count; i++)
        {
            rankings[i].Rank = i + 1;
            rankings[i].Percentile = rankings.Count > 1
                ? Math.Round((1 - (double)i / (rankings.Count - 1)) * 100, 0) : 100;
        }

        return rankings;
    }

    private List<RefereeRankEntry> BuildTierRankings(
        RefereeTier tier, List<Referee> referees, List<MatchOfficialReport> reports)
    {
        var tierReferees = referees.Where(r => r.Tier == tier && r.Status == RefereeStatus.Active).ToList();
        var rankings = new List<RefereeRankEntry>();

        foreach (var referee in tierReferees)
        {
            var refReports = reports.Where(r => r.RefereeId == referee.Id).ToList();
            if (refReports.Count == 0) continue;

            var ratings = refReports.Select(r => r.OverallRating).ToList();
            rankings.Add(new RefereeRankEntry
            {
                RefereeId = referee.Id, Name = referee.FullName,
                AvgRating = Math.Round(ratings.Average(), 1), MatchCount = refReports.Count,
            });
        }

        rankings = rankings.OrderByDescending(r => r.AvgRating).ToList();
        for (int i = 0; i < rankings.Count; i++)
        {
            rankings[i].Rank = i + 1;
            rankings[i].Percentile = rankings.Count > 1
                ? Math.Round((1 - (double)i / (rankings.Count - 1)) * 100, 0) : 100;
        }

        return rankings;
    }

    private CompetencyRadarData BuildCompetencyRadar(
        List<MatchOfficialReport> reports, List<Assessment> assessments)
    {
        var radar = new CompetencyRadarData();

        if (reports.Count > 0)
        {
            radar.PerformanceSummary = Math.Round(reports.Average(r => r.PerformanceSummary.Rating), 1);
            radar.DecisionMaking = Math.Round(reports.Average(r => r.DecisionMaking.Rating), 1);
            radar.Positioning = Math.Round(reports.Average(r => r.Positioning.Rating), 1);
            radar.Communication = Math.Round(reports.Average(r => r.Communication.Rating), 1);
            radar.ManagementStyle = Math.Round(reports.Average(r => r.ManagementStyle.Rating), 1);
            radar.FitnessAndWorkRate = Math.Round(reports.Average(r => r.FitnessAndWorkRate.Rating), 1);
        }

        if (assessments.Count > 0)
        {
            var techScores = assessments.Where(a => a.Technical.AverageScore > 0).ToList();
            if (techScores.Count > 0)
            {
                radar.ApplicationOfLaw = Math.Round(techScores.Average(a => a.Technical.ApplicationOfLaw.Score), 1);
                radar.MatchControl = Math.Round(techScores.Average(a => a.Technical.MatchControl.Score), 1);
            }

            var delScores = assessments.Where(a => a.Delegate.AverageScore > 0).ToList();
            if (delScores.Count > 0)
            {
                radar.Consistency = Math.Round(delScores.Average(a => a.Delegate.Consistency.Score), 1);
            }
        }

        return radar;
    }

    private List<MatchFormEntry> BuildMatchForm(
        List<MatchOfficialReport> reports, List<Assessment> assessments)
    {
        var form = new List<MatchFormEntry>();

        foreach (var report in reports.OrderByDescending(r => r.MatchDate))
        {
            var matchAssessment = assessments.FirstOrDefault(a =>
                a.FixtureId == report.FixtureId && a.RefereeId == report.RefereeId);

            form.Add(new MatchFormEntry
            {
                ReportId = report.Id, AssessmentId = matchAssessment?.Id,
                MatchDate = report.MatchDate, MatchTitle = report.MatchTitle,
                Competition = report.Competition, CompetitionTier = report.CompetitionTier,
                OverallRating = report.OverallRating,
                AccuracyPercent = matchAssessment?.Kmi.TotalIncidents > 0
                    ? matchAssessment.Kmi.AccuracyPercent : null,
                EventCount = report.TotalEvents, CriticalEventCount = report.CriticalEventCount,
                YellowCards = report.Events.Count(e => e.EventType == EventType.YellowCard),
                RedCards = report.Events.Count(e => e.EventType == EventType.RedCard),
            });
        }

        return form;
    }

    private EventStatistics BuildEventStatistics(
        List<MatchOfficialReport> reports, List<Assessment> assessments)
    {
        var stats = new EventStatistics { TotalMatches = reports.Count };
        var allEvents = reports.SelectMany(r => r.Events).ToList();
        stats.TotalYellowCards = allEvents.Count(e => e.EventType == EventType.YellowCard);
        stats.TotalRedCards = allEvents.Count(e => e.EventType == EventType.RedCard);
        stats.TotalPenalties = allEvents.Count(e => e.EventType == EventType.Penalty);
        stats.TotalFouls = allEvents.Count(e => e.EventType == EventType.Foul);
        stats.TotalOffsides = allEvents.Count(e => e.EventType == EventType.Offside);
        stats.TotalVarReviews = allEvents.Count(e => e.EventType == EventType.VarReview);
        stats.TotalCriticalIncidents = allEvents.Count(e => e.Impact == EventImpact.Critical);

        foreach (var assessment in assessments)
        {
            if (assessment.MatchReport.FoulsGiven > 0)
                stats.TotalFouls = Math.Max(stats.TotalFouls, stats.TotalFouls);
        }

        stats.EventDistribution = allEvents
            .GroupBy(e => e.EventType)
            .ToDictionary(g => g.Key, g => g.Count());

        return stats;
    }

    private HalfByHalfAnalysis BuildHalfAnalysis(List<MatchOfficialReport> reports)
    {
        var allEvents = reports.SelectMany(r => r.Events).ToList();
        var firstHalf = allEvents.Where(e => e.Minute <= 45).ToList();
        var secondHalf = allEvents.Where(e => e.Minute > 45).ToList();

        return new HalfByHalfAnalysis
        {
            FirstHalfEvents = firstHalf.Count, SecondHalfEvents = secondHalf.Count,
            FirstHalfCards = firstHalf.Count(e => e.EventType == EventType.YellowCard || e.EventType == EventType.RedCard),
            SecondHalfCards = secondHalf.Count(e => e.EventType == EventType.YellowCard || e.EventType == EventType.RedCard),
            FirstHalfCritical = firstHalf.Count(e => e.Impact == EventImpact.Critical),
            SecondHalfCritical = secondHalf.Count(e => e.Impact == EventImpact.Critical),
        };
    }

    private DecisionAccuracyBreakdown BuildDecisionAccuracy(List<Assessment> assessments)
    {
        var breakdown = new DecisionAccuracyBreakdown();
        var allIncidents = assessments.SelectMany(a => a.Kmi.Incidents).ToList();

        breakdown.TotalDecisionsReviewed = allIncidents.Count;
        breakdown.CorrectDecisions = allIncidents.Count(i => i.OriginalDecisionCorrect);
        breakdown.ByType = allIncidents.GroupBy(i => i.DecisionType)
            .ToDictionary(g => g.Key, g => (Total: g.Count(), Correct: g.Count(i => i.OriginalDecisionCorrect)));

        return breakdown;
    }

    private BiasIndicators BuildBiasIndicators(List<MatchOfficialReport> reports)
    {
        var bias = new BiasIndicators();
        var allEvents = reports.SelectMany(r => r.Events).ToList();
        var homeEvents = allEvents.Where(e => e.Teams == "Home").ToList();
        var awayEvents = allEvents.Where(e => e.Teams == "Away").ToList();

        bias.HomeYellows = homeEvents.Count(e => e.EventType == EventType.YellowCard);
        bias.AwayYellows = awayEvents.Count(e => e.EventType == EventType.YellowCard);
        bias.HomeReds = homeEvents.Count(e => e.EventType == EventType.RedCard);
        bias.AwayReds = awayEvents.Count(e => e.EventType == EventType.RedCard);
        bias.HomePenalties = homeEvents.Count(e => e.EventType == EventType.Penalty);
        bias.AwayPenalties = awayEvents.Count(e => e.EventType == EventType.Penalty);
        bias.HomeFouls = homeEvents.Count(e => e.EventType == EventType.Foul);
        bias.AwayFouls = awayEvents.Count(e => e.EventType == EventType.Foul);

        return bias;
    }

    private PhysicalPerformance BuildPhysicalPerformance(List<Assessment> assessments)
    {
        var physical = new PhysicalPerformance();
        var withData = assessments.Where(a => a.MatchReport.DistanceCoveredKm > 0).ToList();

        if (withData.Count > 0)
        {
            physical.AvgDistanceKm = Math.Round(withData.Average(a => a.MatchReport.DistanceCoveredKm), 1);
            physical.AvgSprintSpeed = Math.Round(withData.Average(a => a.MatchReport.AvgSprintSpeedKmh), 1);
            physical.AvgHighIntensityRuns = (int)Math.Round(withData.Average(a => a.MatchReport.HighIntensityRuns));

            physical.MatchBreakdown = withData.OrderByDescending(a => a.MatchDate).Take(10)
                .Select(a => (a.MatchTitle, a.MatchReport.DistanceCoveredKm,
                    a.MatchReport.AvgSprintSpeedKmh, a.MatchReport.HighIntensityRuns))
                .ToList();
        }

        return physical;
    }

    private TierReadinessAssessment BuildTierReadiness(Referee referee, RefereeAnalyticsProfile profile)
    {
        var nextTier = GetNextTier(referee.Tier);
        var thresholds = GetTierThresholds(nextTier);

        var criteria = new List<ReadinessCriteria>
        {
            new() { Name = "Season Average Rating", RequiredScore = thresholds.minRating,
                ActualScore = profile.SeasonAvgRating },
            new() { Name = "Consistency Index (σ)", RequiredScore = thresholds.maxConsistency,
                ActualScore = profile.ConsistencyIndex > 0
                    ? (profile.ConsistencyIndex <= thresholds.maxConsistency ? thresholds.maxConsistency : profile.ConsistencyIndex)
                    : 0 },
            new() { Name = "Decision Accuracy %", RequiredScore = thresholds.minAccuracy,
                ActualScore = profile.AccuracyPercent },
            new() { Name = "Minimum Matches", RequiredScore = thresholds.minMatches,
                ActualScore = profile.MatchesThisSeason },
        };

        var met = criteria.Count(c => c.Met);
        var total = criteria.Count;
        var readinessScore = total > 0 ? Math.Round((double)met / total * 100, 0) : 0;

        return new TierReadinessAssessment
        {
            CurrentTier = referee.Tier, TargetTier = nextTier,
            ReadinessScore = readinessScore,
            ReadinessLabel = readinessScore >= 75 ? "Ready" : readinessScore >= 50 ? "Developing" : "Not Ready",
            ReadinessColor = readinessScore >= 75 ? "var(--accent-lime)"
                : readinessScore >= 50 ? "var(--accent-orange)" : "var(--accent-red)",
            Criteria = criteria,
        };
    }

    private List<MonthlyTrend> BuildMonthlyTrends(List<MatchOfficialReport> reports)
    {
        var months = new[] { "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar", "Apr", "May" };
        var monthNums = new[] { 8, 9, 10, 11, 12, 1, 2, 3, 4, 5 };

        return months.Select((m, i) =>
        {
            var monthReports = reports.Where(r => r.MatchDate.Month == monthNums[i]).ToList();
            return new MonthlyTrend
            {
                Month = m, MonthIndex = monthNums[i],
                AvgRating = monthReports.Count > 0
                    ? Math.Round(monthReports.Average(r => r.OverallRating), 1) : 0,
                MatchCount = monthReports.Count,
                CardsPerGame = monthReports.Count > 0
                    ? Math.Round((double)monthReports.Sum(r =>
                        r.Events.Count(e => e.EventType == EventType.YellowCard || e.EventType == EventType.RedCard))
                        / monthReports.Count, 2) : 0,
            };
        }).Where(t => t.MatchCount > 0).ToList();
    }

    // ── Helpers ──

    private static double CalculateStdDev(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count < 2) return 0;
        var avg = list.Average();
        return Math.Sqrt(list.Sum(v => Math.Pow(v - avg, 2)) / (list.Count - 1));
    }

    private static double CalculateTrendChange(List<double> ratings)
    {
        if (ratings.Count < 4) return 0;
        var recent = ratings.TakeLast(3).Average();
        var previous = ratings.SkipLast(3).TakeLast(3).Average();
        return Math.Round(recent - previous, 1);
    }

    private static RefereeTier GetNextTier(RefereeTier current) => current switch
    {
        RefereeTier.Grassroots => RefereeTier.NationalLeague,
        RefereeTier.NationalLeague => RefereeTier.LeagueTwo,
        RefereeTier.LeagueTwo => RefereeTier.LeagueOne,
        RefereeTier.LeagueOne => RefereeTier.Championship,
        RefereeTier.Championship => RefereeTier.PremierLeague,
        RefereeTier.PremierLeague => RefereeTier.FifaInternational,
        _ => RefereeTier.FifaInternational
    };

    private static (double minRating, double maxConsistency, double minAccuracy, int minMatches) GetTierThresholds(
        RefereeTier tier) => tier switch
    {
        RefereeTier.FifaInternational => (8.5, 0.5, 90, 20),
        RefereeTier.PremierLeague => (8.0, 0.6, 85, 15),
        RefereeTier.Championship => (7.5, 0.7, 80, 12),
        RefereeTier.LeagueOne => (7.0, 0.8, 75, 10),
        RefereeTier.LeagueTwo => (6.5, 1.0, 70, 8),
        RefereeTier.NationalLeague => (6.0, 1.2, 65, 6),
        _ => (5.5, 1.5, 60, 4)
    };
}
