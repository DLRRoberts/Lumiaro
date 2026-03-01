using LumiaroAdmin.Data.Enums;

namespace LumiaroAdmin.Models;

// ══════════════════════════════════════════════════
// INTERVENTION REVIEW — Single reviewer's score for a single event
// ══════════════════════════════════════════════════

public class InterventionReview
{
    public int Id { get; set; }

    // ── Links ──
    public int ReportId { get; set; }       // Which match report
    public int EventId { get; set; }        // Which event within that report

    // ── Reviewer ──
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerRole { get; set; } = string.Empty;   // e.g. "Observer", "Referee", "Coach"
    public DateTime ReviewedAt { get; set; } = DateTime.Now;

    // ── Scores (0–100 each) ──
    public int AccuracyScore { get; set; }
    public int ConsistencyScore { get; set; }
    public int CommunicationScore { get; set; }
    public int MatchHandlingScore { get; set; }

    // ── Commentary ──
    public string? Comment { get; set; }

    // ── Computed ──
    public double OverallScore => Math.Round(
        (AccuracyScore + ConsistencyScore + CommunicationScore + MatchHandlingScore) / 4.0, 1);
}

// ══════════════════════════════════════════════════
// REVIEWER MATCH SUMMARY — Aggregated per reviewer across all events in a match
// ══════════════════════════════════════════════════

public class ReviewerMatchSummary
{
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerRole { get; set; } = string.Empty;
    public int EventsReviewed { get; set; }
    public DateTime LastReviewedAt { get; set; }

    // ── Category averages (across all events this reviewer scored) ──
    public double AvgAccuracy { get; set; }
    public double AvgConsistency { get; set; }
    public double AvgCommunication { get; set; }
    public double AvgMatchHandling { get; set; }

    public double OverallAverage => Math.Round(
        (AvgAccuracy + AvgConsistency + AvgCommunication + AvgMatchHandling) / 4.0, 1);
}

// ══════════════════════════════════════════════════
// EVENT CONSENSUS — All reviewers' averaged view of a single event
// ══════════════════════════════════════════════════

public class EventConsensus
{
    public int EventId { get; set; }
    public int Minute { get; set; }
    public int? AddedTime { get; set; }
    public EventType EventType { get; set; }
    public string Description { get; set; } = string.Empty;
    public EventImpact Impact { get; set; }
    public int ReviewCount { get; set; }

    // ── Consensus scores (averaged across all reviewers) ──
    public double AvgAccuracy { get; set; }
    public double AvgConsistency { get; set; }
    public double AvgCommunication { get; set; }
    public double AvgMatchHandling { get; set; }

    public double OverallConsensus => ReviewCount > 0
        ? Math.Round((AvgAccuracy + AvgConsistency + AvgCommunication + AvgMatchHandling) / 4.0, 1) : 0;

    // ── Agreement measure (low σ = strong agreement) ──
    public double AccuracySpread { get; set; }      // σ of accuracy scores
    public double ConsistencySpread { get; set; }
    public double CommunicationSpread { get; set; }
    public double MatchHandlingSpread { get; set; }

    public string AgreementLevel
    {
        get
        {
            var avgSpread = (AccuracySpread + ConsistencySpread + CommunicationSpread + MatchHandlingSpread) / 4.0;
            return avgSpread switch
            {
                <= 8 => "Strong",
                <= 15 => "Moderate",
                _ => "Divergent"
            };
        }
    }

    public string AgreementClass => AgreementLevel switch
    {
        "Strong" => "agreement-strong",
        "Moderate" => "agreement-moderate",
        _ => "agreement-divergent"
    };

    // ── Individual reviews for detail drill-down ──
    public List<InterventionReview> Reviews { get; set; } = new();

    public string MinuteDisplay => AddedTime.HasValue ? $"{Minute}+{AddedTime}'" : $"{Minute}'";
}

// ══════════════════════════════════════════════════
// MATCH REVIEW SUMMARY — Full overview of all reviews for a report
// ══════════════════════════════════════════════════

public class MatchReviewSummary
{
    public int ReportId { get; set; }
    public string MatchTitle { get; set; } = string.Empty;
    public string RefereeName { get; set; } = string.Empty;

    public int TotalReviewers { get; set; }
    public int TotalReviews { get; set; }
    public int TotalEvents { get; set; }

    // ── Global consensus across all events and reviewers ──
    public double GlobalAccuracy { get; set; }
    public double GlobalConsistency { get; set; }
    public double GlobalCommunication { get; set; }
    public double GlobalMatchHandling { get; set; }
    public double GlobalOverall => Math.Round(
        (GlobalAccuracy + GlobalConsistency + GlobalCommunication + GlobalMatchHandling) / 4.0, 1);

    public List<ReviewerMatchSummary> ReviewerSummaries { get; set; } = new();
    public List<EventConsensus> EventConsensus { get; set; } = new();
}

// ══════════════════════════════════════════════════
// FORM MODEL — For the review submission page
// ══════════════════════════════════════════════════

public class InterventionReviewFormModel
{
    public int ReportId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string ReviewerRole { get; set; } = "Observer";

    public List<EventReviewEntry> Entries { get; set; } = new();

    public bool IsValid => !string.IsNullOrWhiteSpace(ReviewerName) &&
        Entries.Any(e => e.HasScores);
}

public class EventReviewEntry
{
    // Event reference
    public int EventId { get; set; }
    public int Minute { get; set; }
    public int? AddedTime { get; set; }
    public EventType EventType { get; set; }
    public string Description { get; set; } = string.Empty;
    public EventImpact Impact { get; set; }
    public string? OfficialResponse { get; set; }

    // Reviewer scores
    public int AccuracyScore { get; set; }
    public int ConsistencyScore { get; set; }
    public int CommunicationScore { get; set; }
    public int MatchHandlingScore { get; set; }
    public string? Comment { get; set; }

    // UI state
    public bool IsExpanded { get; set; }

    public bool HasScores => AccuracyScore > 0 || ConsistencyScore > 0 ||
        CommunicationScore > 0 || MatchHandlingScore > 0;

    public double OverallScore => HasScores
        ? Math.Round((AccuracyScore + ConsistencyScore + CommunicationScore + MatchHandlingScore) / 4.0, 1) : 0;

    public string MinuteDisplay => AddedTime.HasValue ? $"{Minute}+{AddedTime}'" : $"{Minute}'";
}
