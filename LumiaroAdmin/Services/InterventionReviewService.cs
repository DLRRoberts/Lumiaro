using LumiaroAdmin.Data.Repositories;
using LumiaroAdmin.Models;
using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Services;

public class InterventionReviewService
{
    private readonly IInterventionReviewRepository _reviewRepo;
    private readonly MatchReportService _reportService;

    public InterventionReviewService(IInterventionReviewRepository reviewRepo, MatchReportService reportService)
    {
        _reviewRepo = reviewRepo;
        _reportService = reportService;
    }

    // ══════════════════════════════════════════════════
    // CRUD
    // ══════════════════════════════════════════════════

    public async Task<List<InterventionReview>> GetByReportAsync(int reportId)
    {
        var entities = await _reviewRepo.GetByReportAsync(reportId);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<InterventionReview>> GetByReportAndEventAsync(int reportId, int eventId)
    {
        var entities = await _reviewRepo.GetByReportAndEventAsync(reportId, eventId);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<InterventionReview>> GetByReviewerAsync(int reportId, string reviewerName)
    {
        var entities = await _reviewRepo.GetByReviewerAsync(reportId, reviewerName);
        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<bool> HasReviewedAsync(int reportId, string reviewerName) =>
        await _reviewRepo.HasReviewedAsync(reportId, reviewerName);

    public async Task<int> GetReviewerCountAsync(int reportId) =>
        await _reviewRepo.GetReviewerCountAsync(reportId);

    public async Task<int> GetReviewCountAsync(int reportId) =>
        await _reviewRepo.GetReviewCountAsync(reportId);

    /// <summary>Submit a batch of reviews from one reviewer for one match report.</summary>
    public async Task SubmitReviewsAsync(int reportId, string reviewerName, string reviewerRole,
        List<(int EventId, int Accuracy, int Consistency, int Communication, int MatchHandling, string? Comment)> scores)
    {
        // Remove any existing reviews from this reviewer for this report (allows re-submission)
        await _reviewRepo.RemoveByReviewerAsync(reportId, reviewerName);
        
        var review = new List<InterventionReviewEntity>();

        foreach (var s in scores.Where(s => s.Accuracy > 0 || s.Consistency > 0 ||
            s.Communication > 0 || s.MatchHandling > 0))
        {
            var entity = new InterventionReviewEntity
            {
                ReportId = reportId,
                EventId = s.EventId,
                ReviewerName = reviewerName,
                ReviewerRole = reviewerRole,
                AccuracyScore = Clamp(s.Accuracy),
                ConsistencyScore = Clamp(s.Consistency),
                CommunicationScore = Clamp(s.Communication),
                MatchHandlingScore = Clamp(s.MatchHandling),
                Comment = s.Comment,
                ReviewedAt = DateTime.UtcNow,
            };
            review.Add(entity);
        }
        await _reviewRepo.SaveReviewsAsync(review);
    }

    public async Task DeleteReviewerSubmissionAsync(int reportId, string reviewerName)
    {
        await _reviewRepo.RemoveByReviewerAsync(reportId, reviewerName);
    }

    // ══════════════════════════════════════════════════
    // AGGREGATION — Reviewer Match Summaries
    // ══════════════════════════════════════════════════

    public async Task<List<ReviewerMatchSummary>> GetReviewerSummariesAsync(int reportId)
    {
        var reviews = await GetByReportAsync(reportId);
        return reviews
            .GroupBy(r => r.ReviewerName, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ReviewerMatchSummary
            {
                ReviewerName = g.First().ReviewerName,
                ReviewerRole = g.First().ReviewerRole,
                EventsReviewed = g.Count(),
                LastReviewedAt = g.Max(r => r.ReviewedAt),
                AvgAccuracy = Math.Round(g.Average(r => r.AccuracyScore), 1),
                AvgConsistency = Math.Round(g.Average(r => r.ConsistencyScore), 1),
                AvgCommunication = Math.Round(g.Average(r => r.CommunicationScore), 1),
                AvgMatchHandling = Math.Round(g.Average(r => r.MatchHandlingScore), 1),
            })
            .OrderByDescending(s => s.OverallAverage)
            .ToList();
    }

    // ══════════════════════════════════════════════════
    // AGGREGATION — Event Consensus
    // ══════════════════════════════════════════════════

    public async Task<List<EventConsensus>> GetEventConsensusAsync(int reportId)
    {
        var report = await _reportService.GetByIdAsync(reportId);
        if (report == null) return new();

        var reviews = await GetByReportAsync(reportId);
        var consensus = new List<EventConsensus>();

        foreach (var evt in report.Events)
        {
            var evtReviews = reviews.Where(r => r.EventId == evt.Id).ToList();
            var entry = new EventConsensus
            {
                EventId = evt.Id, Minute = evt.Minute, AddedTime = evt.AddedTime,
                EventType = evt.EventType.ToData(), 
                Description = evt.Description,
                Impact = evt.Impact.ToData(), 
                ReviewCount = evtReviews.Count, 
                Reviews = evtReviews,
            };
            if (evtReviews.Count > 0)
            {
                entry.AvgAccuracy = Math.Round(evtReviews.Average(r => r.AccuracyScore), 1);
                entry.AvgConsistency = Math.Round(evtReviews.Average(r => r.ConsistencyScore), 1);
                entry.AvgCommunication = Math.Round(evtReviews.Average(r => r.CommunicationScore), 1);
                entry.AvgMatchHandling = Math.Round(evtReviews.Average(r => r.MatchHandlingScore), 1);
                if (evtReviews.Count >= 2)
                {
                    entry.AccuracySpread = StdDev(evtReviews.Select(r => (double)r.AccuracyScore));
                    entry.ConsistencySpread = StdDev(evtReviews.Select(r => (double)r.ConsistencyScore));
                    entry.CommunicationSpread = StdDev(evtReviews.Select(r => (double)r.CommunicationScore));
                    entry.MatchHandlingSpread = StdDev(evtReviews.Select(r => (double)r.MatchHandlingScore));
                }
            }
            consensus.Add(entry);
        }

        return consensus.OrderBy(c => c.Minute).ThenBy(c => c.AddedTime ?? 0).ToList();
    }

    // ══════════════════════════════════════════════════
    // AGGREGATION — Full Match Review Summary
    // ══════════════════════════════════════════════════

    public async Task<MatchReviewSummary?> GetMatchSummaryAsync(int reportId)
    {
        var report = await _reportService.GetByIdAsync(reportId);
        if (report == null) return null;

        var reviews = await GetByReportAsync(reportId);
        var reviewerSummaries = await GetReviewerSummariesAsync(reportId);
        var eventConsensus = await GetEventConsensusAsync(reportId);

        var summary = new MatchReviewSummary
        {
            ReportId = reportId,
            MatchTitle = report.MatchTitle,
            RefereeName = report.RefereeName,
            TotalReviewers = reviewerSummaries.Count,
            TotalReviews = reviews.Count,
            TotalEvents = report.Events.Count,
            ReviewerSummaries = reviewerSummaries,
            EventConsensus = eventConsensus,
        };

        if (reviews.Count > 0)
        {
            summary.GlobalAccuracy = Math.Round(reviews.Average(r => r.AccuracyScore), 1);
            summary.GlobalConsistency = Math.Round(reviews.Average(r => r.ConsistencyScore), 1);
            summary.GlobalCommunication = Math.Round(reviews.Average(r => r.CommunicationScore), 1);
            summary.GlobalMatchHandling = Math.Round(reviews.Average(r => r.MatchHandlingScore), 1);
        }

        return summary;
    }

    // ══════════════════════════════════════════════════
    // HELPERS
    // ══════════════════════════════════════════════════

    private static int Clamp(int value) => Math.Max(0, Math.Min(100, value));

    private static double StdDev(IEnumerable<double> values)
    {
        var list = values.ToList();
        if (list.Count < 2) return 0;
        var avg = list.Average();
        return Math.Round(Math.Sqrt(list.Sum(v => Math.Pow(v - avg, 2)) / (list.Count - 1)), 1);
    }
}
