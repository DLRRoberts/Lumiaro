namespace LumiaroAdmin.Data.Enums;


public enum RefereeStatus
{
    Active = 0,
    Inactive = 1,
    Probation = 2,
    Suspended = 3
}

public enum RefereeTier
{
    FifaInternational = 0,
    PremierLeague = 1,
    Championship = 2,
    LeagueOne = 3,
    LeagueTwo = 4,
    NationalLeague = 5,
    Grassroots = 6
}

public enum OfficialRole
{
    Referee = 0,
    AssistantReferee1 = 1,
    AssistantReferee2 = 2,
    MatchOfficial = 3
}

public enum AssessmentStatus
{
    Draft = 0,
    Submitted = 1,
    Reviewed = 2,
    Finalised = 3
}

public enum KmiDecisionType
{
    Penalty = 0,
    RedCard = 1,
    GoalAllowed = 2,
    GoalDisallowed = 3,
    Offside = 4,
    Foul = 5,
    Handball = 6,
    Other = 7
}


public enum ReportStatus
{
    Draft = 0,
    Submitted = 1,
    Published = 2
}

public enum EventType
{
    Goal = 0,
    Foul = 1,
    YellowCard = 2,
    RedCard = 3,
    Penalty = 4,
    Offside = 5,
    VarReview = 6,
    Substitution = 7,
    Injury = 8,
    Stoppage = 9,
    Other = 10
}

public enum EventImpact
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum DecisionAccuracy
{
    NotApplicable = 0,
    Correct = 1,
    Incorrect = 2,
    Debatable = 3
}
