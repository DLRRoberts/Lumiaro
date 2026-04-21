using AutoMapper;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Models;
using LumiaroAdmin.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LumiaroAdmin.Data.Profiles;

public class DtoToModelProfile : Profile
{
    public DtoToModelProfile()
    {
        CreateMap<AssessmentEntity, Assessment>()
            .ConstructUsing(ConvertAssessmentEntityToAssessment);

        CreateMap<DelegateReportEntity, DelegateReport>()
            .ConstructUsing(ConvertDelegateReportEntityToDelegateReport);
        
        CreateMap<AssessmentEntity, KmiReport>()
            .ConstructUsing(ConvertAssessmentEntityToKmiReport);

        CreateMap<KeyMatchIncidentEntity, KeyMatchIncident>()
            .ConstructUsing(ConvertKeyMatchIncidentReportToKeyMatchIncident);
    }

    private KeyMatchIncident ConvertKeyMatchIncidentReportToKeyMatchIncident(KeyMatchIncidentEntity source, ResolutionContext context)
    {
        return new KeyMatchIncident
        {
            DecisionType = MapDecisionType(source.DecisionType),
            Description = source.Description,
            Minute = source.Minute,
            OriginalDecisionCorrect = source.OriginalDecisionCorrect,
            PanelVotesCorrect = source.PanelVotesCorrect,
            PanelVotesIncorrect = source.PanelVotesIncorrect,
            Rationale = source.Rationale,
            VarInterventionMade = source.VarInterventionMade,
            VarInterventionRequired = source.VarInterventionRequired
        };
    }

    private KmiReport ConvertAssessmentEntityToKmiReport(AssessmentEntity source, ResolutionContext context)
    {
        return new KmiReport
        {
            PanelNotes = source.KmiPanelNotes,
            Incidents = context.Mapper.Map<List<KeyMatchIncident>>(source.KeyMatchIncidents)
        };
    }

    private DelegateReport ConvertDelegateReportEntityToDelegateReport(DelegateReportEntity source, ResolutionContext context)
    {
        var communicationScore = new CompetencyScore
        {
            AreasForImprovement = source.CommunicationAreasForImprovement,
            Notes = source.CommunicationNotes,
            Strengths = source.CommunicationStrengths,
            Score = (double)source.CommunicationScore,
        };

        var consistencyScore = new CompetencyScore
        {
            AreasForImprovement = source.ConsistencyAreasForImprovement,
            Notes = source.ConsistencyNotes,
            Strengths = source.ConsistencyStrengths,
            Score = (double)source.ConsistencyScore,
        };

        var managementScore = new CompetencyScore
        {
            AreasForImprovement = source.ManagementAreasForImprovement,
            Notes = source.ManagementNotes,
            Strengths = source.ManagementStrengths,
            Score = (double)source.ManagementScore,
        };

        return new DelegateReport
        {
            DelegateName = source.DelegateName,
            OverallNotes = source.OverallNotes,
            Communication = communicationScore,
            Consistency = consistencyScore,
            ManagementOfGame =  managementScore, 
        };
    }

    private Assessment ConvertAssessmentEntityToAssessment(AssessmentEntity source, ResolutionContext context)
    {
        var delegateReport = context.Mapper.Map<DelegateReport>(source.DelegateReport);
        var kmiReport = context.Mapper.Map<KmiReport>(source);
        return new Assessment
        {
            Id = source.Id,
            Competition = source.Fixture?.Division?.Name,
            CompetitionTier = MapTier(source.Fixture?.Division?.Tier),
            Venue = source.Fixture?.Venue,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            CreatedBy = source.CreatedBy,
            FixtureId = source.FixtureId,
            MatchDate = source.Fixture?.Date ?? source.CreatedAt.Date,
            OverallScore = source.OverallScore,
            Recommendations = source.Recommendations,
            RefereeCode = source.Referee?.RefCode,
            MatchTitle = CreateMatchTitle(source.Fixture),
            RefereeId = source.RefereeId,
            RefereeName = CreateRefereeDisplayName(source.Referee),
            RefereeTier = MapTier(source.Fixture?.Division?.Tier),
            RefereeInitials = CreateRefereeInitials(source.Referee),
            SummaryNotes = source.SummaryNotes,
            Delegate = delegateReport,
            Kmi = kmiReport
        };
    }

    private static string CreateRefereeInitials(RefereeEntity sourceReferee) =>
        $"{sourceReferee.FirstName.First()}{sourceReferee.LastName?.FirstOrDefault().ToString() ?? ""}";

    private static string CreateRefereeDisplayName(RefereeEntity sourceReferee) =>
        $"{sourceReferee?.FirstName} {sourceReferee?.LastName}";

    private static string CreateMatchTitle(FixtureEntity sourceFixture) =>
        $"{sourceFixture?.HomeTeam} - {sourceFixture?.AwayTeam} ({sourceFixture?.Date.ToShortDateString()}";

    private RefereeTier MapTier(Enums.RefereeTier? divisionTier) =>
        divisionTier switch
        {
            Enums.RefereeTier.Championship => LumiaroAdmin.Models.RefereeTier.Championship,
            Enums.RefereeTier.FifaInternational => LumiaroAdmin.Models.RefereeTier.FifaInternational,
            Enums.RefereeTier.Grassroots => LumiaroAdmin.Models.RefereeTier.Grassroots,
            Enums.RefereeTier.LeagueOne => LumiaroAdmin.Models.RefereeTier.LeagueOne,
            Enums.RefereeTier.LeagueTwo => LumiaroAdmin.Models.RefereeTier.LeagueTwo,
            Enums.RefereeTier.NationalLeague => LumiaroAdmin.Models.RefereeTier.NationalLeague,
            Enums.RefereeTier.PremierLeague => LumiaroAdmin.Models.RefereeTier.PremierLeague,
            _ => RefereeTier.Championship
        };
    
    private static KmiDecisionType MapDecisionType(Enums.KmiDecisionType sourceDecisionType) =>
        sourceDecisionType switch
        {
            Enums.KmiDecisionType.Foul => KmiDecisionType.Foul,
            Enums.KmiDecisionType.GoalAllowed => KmiDecisionType.GoalAllowed,
            Enums.KmiDecisionType.GoalDisallowed => KmiDecisionType.GoalDisallowed,
            Enums.KmiDecisionType.Handball => KmiDecisionType.Handball,
            Enums.KmiDecisionType.Offside => KmiDecisionType.Offside,
            Enums.KmiDecisionType.Other => KmiDecisionType.Other,
            Enums.KmiDecisionType.Penalty => KmiDecisionType.Penalty,
            Enums.KmiDecisionType.RedCard => KmiDecisionType.RedCard,
            _ => throw new ArgumentOutOfRangeException(nameof(sourceDecisionType), sourceDecisionType, null)
        };

}