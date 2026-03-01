using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;

namespace RedZone.LumiaroAdmin.Data.Mapping;

public static class AssessmentMapper
{
    /// <summary>
    /// Creates a new AssessmentEntity with all child report entities from form input.
    /// </summary>
    public static AssessmentEntity ToEntity(
        int fixtureId, int refereeId, OfficialRole officialRole,
        AssessmentStatus status, decimal overallScore,
        string? summaryNotes, string? recommendations, string createdBy,
        string? kmiPanelNotes = null)
    {
        return new AssessmentEntity
        {
            FixtureId = fixtureId,
            RefereeId = refereeId,
            OfficialRole = officialRole,
            Status = status,
            OverallScore = (double)overallScore,
            SummaryNotes = summaryNotes,
            Recommendations = recommendations,
            CreatedBy = createdBy,
            KmiPanelNotes = kmiPanelNotes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(AssessmentEntity entity,
        AssessmentStatus status, decimal overallScore,
        string? summaryNotes, string? recommendations, string? kmiPanelNotes)
    {
        entity.Status = status;
        entity.OverallScore = (double)overallScore;
        entity.SummaryNotes = summaryNotes;
        entity.Recommendations = recommendations;
        entity.KmiPanelNotes = kmiPanelNotes;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    // ── Technical Report ──

    public static TechnicalReportEntity ToTechnicalReportEntity(int assessmentId,
        string reviewerName,
        decimal lawScore, string? lawNotes, string? lawStrengths, string? lawAreas,
        decimal positioningScore, string? positioningNotes, string? positioningStrengths, string? positioningAreas,
        decimal fitnessScore, string? fitnessNotes, string? fitnessStrengths, string? fitnessAreas,
        decimal controlScore, string? controlNotes, string? controlStrengths, string? controlAreas,
        string? overallNotes)
    {
        return new TechnicalReportEntity
        {
            AssessmentId = assessmentId,
            ReviewerName = reviewerName,
            LawScore = lawScore,
            LawNotes = lawNotes,
            LawStrengths = lawStrengths,
            LawAreasForImprovement = lawAreas,
            PositioningScore = positioningScore,
            PositioningNotes = positioningNotes,
            PositioningStrengths = positioningStrengths,
            PositioningAreasForImprovement = positioningAreas,
            FitnessScore = fitnessScore,
            FitnessNotes = fitnessNotes,
            FitnessStrengths = fitnessStrengths,
            FitnessAreasForImprovement = fitnessAreas,
            ControlScore = controlScore,
            ControlNotes = controlNotes,
            ControlStrengths = controlStrengths,
            ControlAreasForImprovement = controlAreas,
            OverallNotes = overallNotes
        };
    }

    public static void UpdateTechnicalReport(TechnicalReportEntity entity,
        string reviewerName,
        decimal lawScore, string? lawNotes, string? lawStrengths, string? lawAreas,
        decimal positioningScore, string? positioningNotes, string? positioningStrengths, string? positioningAreas,
        decimal fitnessScore, string? fitnessNotes, string? fitnessStrengths, string? fitnessAreas,
        decimal controlScore, string? controlNotes, string? controlStrengths, string? controlAreas,
        string? overallNotes)
    {
        entity.ReviewerName = reviewerName;
        entity.LawScore = lawScore;
        entity.LawNotes = lawNotes;
        entity.LawStrengths = lawStrengths;
        entity.LawAreasForImprovement = lawAreas;
        entity.PositioningScore = positioningScore;
        entity.PositioningNotes = positioningNotes;
        entity.PositioningStrengths = positioningStrengths;
        entity.PositioningAreasForImprovement = positioningAreas;
        entity.FitnessScore = fitnessScore;
        entity.FitnessNotes = fitnessNotes;
        entity.FitnessStrengths = fitnessStrengths;
        entity.FitnessAreasForImprovement = fitnessAreas;
        entity.ControlScore = controlScore;
        entity.ControlNotes = controlNotes;
        entity.ControlStrengths = controlStrengths;
        entity.ControlAreasForImprovement = controlAreas;
        entity.OverallNotes = overallNotes;
    }

    // ── Delegate Report ──

    public static DelegateReportEntity ToDelegateReportEntity(int assessmentId,
        string delegateName,
        decimal managementScore, string? managementNotes, string? managementStrengths, string? managementAreas,
        decimal communicationScore, string? communicationNotes, string? communicationStrengths, string? communicationAreas,
        decimal consistencyScore, string? consistencyNotes, string? consistencyStrengths, string? consistencyAreas,
        string? overallNotes)
    {
        return new DelegateReportEntity
        {
            AssessmentId = assessmentId,
            DelegateName = delegateName,
            ManagementScore = managementScore,
            ManagementNotes = managementNotes,
            ManagementStrengths = managementStrengths,
            ManagementAreasForImprovement = managementAreas,
            CommunicationScore = communicationScore,
            CommunicationNotes = communicationNotes,
            CommunicationStrengths = communicationStrengths,
            CommunicationAreasForImprovement = communicationAreas,
            ConsistencyScore = consistencyScore,
            ConsistencyNotes = consistencyNotes,
            ConsistencyStrengths = consistencyStrengths,
            ConsistencyAreasForImprovement = consistencyAreas,
            OverallNotes = overallNotes
        };
    }

    public static void UpdateDelegateReport(DelegateReportEntity entity,
        string delegateName,
        decimal managementScore, string? managementNotes, string? managementStrengths, string? managementAreas,
        decimal communicationScore, string? communicationNotes, string? communicationStrengths, string? communicationAreas,
        decimal consistencyScore, string? consistencyNotes, string? consistencyStrengths, string? consistencyAreas,
        string? overallNotes)
    {
        entity.DelegateName = delegateName;
        entity.ManagementScore = managementScore;
        entity.ManagementNotes = managementNotes;
        entity.ManagementStrengths = managementStrengths;
        entity.ManagementAreasForImprovement = managementAreas;
        entity.CommunicationScore = communicationScore;
        entity.CommunicationNotes = communicationNotes;
        entity.CommunicationStrengths = communicationStrengths;
        entity.CommunicationAreasForImprovement = communicationAreas;
        entity.ConsistencyScore = consistencyScore;
        entity.ConsistencyNotes = consistencyNotes;
        entity.ConsistencyStrengths = consistencyStrengths;
        entity.ConsistencyAreasForImprovement = consistencyAreas;
        entity.OverallNotes = overallNotes;
    }

    // ── Match Stat Report ──

    public static MatchStatReportEntity ToMatchStatReportEntity(int assessmentId,
        int yellowCards, int redCards, int penaltiesAwarded, int foulsGiven, int offsidesCalled,
        decimal distanceCoveredKm, decimal avgSprintSpeedKmh, int highIntensityRuns,
        string? incidentNotes, string? varInteractionNotes)
    {
        return new MatchStatReportEntity
        {
            AssessmentId = assessmentId,
            YellowCards = yellowCards,
            RedCards = redCards,
            PenaltiesAwarded = penaltiesAwarded,
            FoulsGiven = foulsGiven,
            OffsidesCalled = offsidesCalled,
            DistanceCoveredKm = distanceCoveredKm,
            AvgSprintSpeedKmh = avgSprintSpeedKmh,
            HighIntensityRuns = highIntensityRuns,
            IncidentNotes = incidentNotes,
            VarInteractionNotes = varInteractionNotes
        };
    }

    public static void UpdateMatchStatReport(MatchStatReportEntity entity,
        int yellowCards, int redCards, int penaltiesAwarded, int foulsGiven, int offsidesCalled,
        decimal distanceCoveredKm, decimal avgSprintSpeedKmh, int highIntensityRuns,
        string? incidentNotes, string? varInteractionNotes)
    {
        entity.YellowCards = yellowCards;
        entity.RedCards = redCards;
        entity.PenaltiesAwarded = penaltiesAwarded;
        entity.FoulsGiven = foulsGiven;
        entity.OffsidesCalled = offsidesCalled;
        entity.DistanceCoveredKm = distanceCoveredKm;
        entity.AvgSprintSpeedKmh = avgSprintSpeedKmh;
        entity.HighIntensityRuns = highIntensityRuns;
        entity.IncidentNotes = incidentNotes;
        entity.VarInteractionNotes = varInteractionNotes;
    }

    // ── Key Match Incidents ──

    public static KeyMatchIncidentEntity ToKeyMatchIncidentEntity(int assessmentId,
        int minute, string description, KmiDecisionType decisionType,
        bool originalCorrect, bool varRequired, bool varMade,
        int votesCorrect, int votesIncorrect, string? rationale, int sortOrder)
    {
        return new KeyMatchIncidentEntity
        {
            AssessmentId = assessmentId,
            Minute = minute,
            Description = description,
            DecisionType = decisionType,
            OriginalDecisionCorrect = originalCorrect,
            VarInterventionRequired = varRequired,
            VarInterventionMade = varMade,
            PanelVotesCorrect = votesCorrect,
            PanelVotesIncorrect = votesIncorrect,
            Rationale = rationale,
            SortOrder = sortOrder
        };
    }
}
