using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RedZone.LumiaroAdmin.Data.Entities;

namespace RedZone.LumiaroAdmin.Data;

/// <summary>
/// Seeds all demo data into the database. Called during application startup.
/// Replaces all GenerateSample*() methods that previously lived in in-memory services.
/// </summary>
public static class LumiaroDbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LumiaroDbContext>();

        // Only seed if the database is empty
        if (await db.Referees.AnyAsync()) return;

        // ════════════════════════════════════════════
        // 1. DIVISIONS
        // ════════════════════════════════════════════
        var divisions = new[]
        {
            new DivisionEntity { Name = "Premier League", ShortName = "PL", Tier = RefereeTier.PremierLeague, SortOrder = 1 },
            new DivisionEntity { Name = "Championship", ShortName = "CH", Tier = RefereeTier.Championship, SortOrder = 2 },
            new DivisionEntity { Name = "League One", ShortName = "L1", Tier = RefereeTier.LeagueOne, SortOrder = 3 },
            new DivisionEntity { Name = "League Two", ShortName = "L2", Tier = RefereeTier.LeagueTwo, SortOrder = 4 },
            new DivisionEntity { Name = "FA Cup", ShortName = "FAC", Tier = RefereeTier.PremierLeague, SortOrder = 5 },
        };
        db.Divisions.AddRange(divisions);
        await db.SaveChangesAsync();

        // ════════════════════════════════════════════
        // 2. REFEREES
        // ════════════════════════════════════════════
        var oliver = new RefereeEntity
        {
            RefCode = "REF-0042", FirstName = "Michael", LastName = "Oliver",
            DateOfBirth = new DateTime(1985, 2, 20), Nationality = "English",
            Location = "Ashington, England", Region = "Northumberland",
            Tier = RefereeTier.FifaInternational, Status = RefereeStatus.Active,
            RegistrationDate = new DateTime(2010, 7, 1), LastActiveDate = new DateTime(2026, 2, 12),
            TotalMatches = 387, AverageScore = 8.4, AccuracyPercent = 94.2, TotalSeasons = 16,
            CurrentSeasonMatches = 24, CurrentSeasonAvgScore = 8.4,
            CurrentSeasonYellows = 62, CurrentSeasonReds = 3,
            CurrentSeasonPenalties = 5, CurrentSeasonIncidents = 6,
            Email = "m.oliver@pgmol.com",
            CareerHistory = new List<CareerEntryEntity>
            {
                new() { Period = "2018 — Present", Title = "FIFA International Referee", Detail = "Champions League, World Cup 2022 & 2026 qualifier, Nations League", IsCurrent = true, SortOrder = 0 },
                new() { Period = "2010 — 2018", Title = "Premier League — Select Group 1", Detail = "Full-time PGMOL referee, Community Shield, FA Cup semi-finals", SortOrder = 1 },
            },
            SeasonBreakdown = new List<SeasonStatsEntity>
            {
                new() { Season = "2025/26", Tier = RefereeTier.FifaInternational, Matches = 24, Yellows = 62, Reds = 3, Penalties = 5, AvgScore = 8.4, TrendChange = 0.2 },
                new() { Season = "2024/25", Tier = RefereeTier.FifaInternational, Matches = 38, Yellows = 98, Reds = 4, Penalties = 8, AvgScore = 8.2, TrendChange = 0.1 },
            }
        };

        var taylor = new RefereeEntity
        {
            RefCode = "REF-0018", FirstName = "Anthony", LastName = "Taylor",
            DateOfBirth = new DateTime(1978, 10, 20), Nationality = "English",
            Location = "Wythenshawe, England", Region = "Greater Manchester",
            Tier = RefereeTier.FifaInternational, Status = RefereeStatus.Active,
            RegistrationDate = new DateTime(2016, 1, 3), LastActiveDate = new DateTime(2026, 2, 11),
            TotalMatches = 412, AverageScore = 7.9, AccuracyPercent = 92.3, TotalSeasons = 15,
            CurrentSeasonMatches = 22, CurrentSeasonAvgScore = 7.9,
            CurrentSeasonYellows = 58, CurrentSeasonReds = 2,
            CurrentSeasonPenalties = 4, CurrentSeasonIncidents = 5,
            Email = "a.taylor@pgmol.com",
            CareerHistory = new List<CareerEntryEntity>
            {
                new() { Period = "2020 — Present", Title = "FIFA International Referee", Detail = "UEFA Europa League, FIFA Club World Cup", IsCurrent = true, SortOrder = 0 },
                new() { Period = "2013 — 2020", Title = "Premier League — Select Group 1", Detail = "Full-time PGMOL referee, multiple cup finals", SortOrder = 1 },
            },
            SeasonBreakdown = new List<SeasonStatsEntity>
            {
                new() { Season = "2025/26", Tier = RefereeTier.FifaInternational, Matches = 22, Yellows = 58, Reds = 2, Penalties = 4, AvgScore = 7.9, TrendChange = 0.1 },
                new() { Season = "2024/25", Tier = RefereeTier.FifaInternational, Matches = 36, Yellows = 91, Reds = 5, Penalties = 7, AvgScore = 7.8, TrendChange = -0.2 },
            }
        };

        var brooks = new RefereeEntity
        {
            RefCode = "REF-0091", FirstName = "John", LastName = "Brooks",
            DateOfBirth = new DateTime(1990, 3, 15), Nationality = "English",
            Location = "Leicester, England", Region = "Leicestershire",
            Tier = RefereeTier.PremierLeague, Status = RefereeStatus.Active,
            RegistrationDate = new DateTime(2020, 6, 22), LastActiveDate = new DateTime(2026, 2, 10),
            TotalMatches = 186, AverageScore = 8.0, AccuracyPercent = 91.8, TotalSeasons = 8,
            CurrentSeasonMatches = 18, CurrentSeasonAvgScore = 8.0,
            CurrentSeasonYellows = 48, CurrentSeasonReds = 2,
            CurrentSeasonPenalties = 3, CurrentSeasonIncidents = 4,
            CareerHistory = new List<CareerEntryEntity>
            {
                new() { Period = "2020 — Present", Title = "Premier League — Select Group 1", Detail = "PGMOL full-time referee", IsCurrent = true, SortOrder = 0 },
                new() { Period = "2016 — 2020", Title = "Football League — Championship", Detail = "Championship appointments, League Cup", SortOrder = 1 },
            },
            SeasonBreakdown = new List<SeasonStatsEntity>
            {
                new() { Season = "2025/26", Tier = RefereeTier.PremierLeague, Matches = 18, Yellows = 48, Reds = 2, Penalties = 3, AvgScore = 8.0, TrendChange = 0.3 },
            }
        };

        var roberts = new RefereeEntity
        {
            RefCode = "REF-0127", FirstName = "Peter", LastName = "Bankes",
            DateOfBirth = new DateTime(1982, 11, 8), Nationality = "English",
            Location = "Blackburn", Region = "Lancashire",
            Tier = RefereeTier.Championship, Status = RefereeStatus.Probation,
            RegistrationDate = new DateTime(2022, 9, 10), LastActiveDate = new DateTime(2026, 2, 9),
            TotalMatches = 64, AverageScore = 7.2, AccuracyPercent = 88.5, TotalSeasons = 4,
            CurrentSeasonMatches = 12, CurrentSeasonAvgScore = 7.2,
            CurrentSeasonYellows = 34, CurrentSeasonReds = 1,
            CurrentSeasonPenalties = 2, CurrentSeasonIncidents = 3,
            CareerHistory = new List<CareerEntryEntity>
            {
                new() { Period = "2022 — Present", Title = "Football League — Championship", Detail = "Championship and League Cup appointments", IsCurrent = true, SortOrder = 0 },
                new() { Period = "2019 — 2022", Title = "League One & Two", Detail = "Lower league professional appointments", SortOrder = 1 },
            },
            SeasonBreakdown = new List<SeasonStatsEntity>
            {
                new() { Season = "2025/26", Tier = RefereeTier.Championship, Matches = 12, Yellows = 34, Reds = 1, Penalties = 2, AvgScore = 7.2, TrendChange = -0.3 },
            }
        };

        var phillips = new RefereeEntity
        {
            RefCode = "REF-0134", FirstName = "Paul", LastName = "Tierney",
            DateOfBirth = new DateTime(1980, 12, 25), Nationality = "England",
            Location = "Pembrokeshire, Wales", Region = "Lancashire",
            Tier = RefereeTier.LeagueOne, Status = RefereeStatus.Active,
            RegistrationDate = new DateTime(2026, 2, 11), LastActiveDate = new DateTime(2026, 2, 8),
            TotalMatches = 41, AverageScore = 7.6, AccuracyPercent = 90.2, TotalSeasons = 3,
            CurrentSeasonMatches = 10, CurrentSeasonAvgScore = 7.6,
            CurrentSeasonYellows = 26, CurrentSeasonReds = 1,
            CurrentSeasonPenalties = 1, CurrentSeasonIncidents = 2,
            CareerHistory = new List<CareerEntryEntity>
            {
                new() { Period = "2023 — Present", Title = "Football League — League One", Detail = "League One and League Cup early rounds", IsCurrent = true, SortOrder = 0 },
                new() { Period = "2021 — 2023", Title = "National League", Detail = "National League and FA Trophy appointments", SortOrder = 1 },
            },
            SeasonBreakdown = new List<SeasonStatsEntity>
            {
                new() { Season = "2025/26", Tier = RefereeTier.LeagueOne, Matches = 10, Yellows = 26, Reds = 1, Penalties = 1, AvgScore = 7.6, TrendChange = 0.4 },
            }
        };

        var mitchell = new RefereeEntity
        {
            RefCode = "REF-0098", FirstName = "Thomas", LastName = "Kirk",
            DateOfBirth = new DateTime(1988, 4, 3), Nationality = "English",
            Location = "Birmingham, England", Region = "West Midlands",
            Tier = RefereeTier.PremierLeague, Status = RefereeStatus.Inactive,
            RegistrationDate = new DateTime(2019, 3, 7), LastActiveDate = new DateTime(2025, 11, 20),
            TotalMatches = 203, AverageScore = 7.7, AccuracyPercent = 91.0, TotalSeasons = 9,
            CurrentSeasonMatches = 0, CurrentSeasonAvgScore = 0,
            CareerHistory = new List<CareerEntryEntity>
            {
                new() { Period = "2019 — 2025", Title = "Premier League — Select Group 2", Detail = "PGMOL referee, currently inactive", SortOrder = 0 },
                new() { Period = "2015 — 2019", Title = "Football League — Championship", Detail = "Championship appointments", SortOrder = 1 },
            },
            SeasonBreakdown = new List<SeasonStatsEntity>
            {
                new() { Season = "2024/25", Tier = RefereeTier.PremierLeague, Matches = 28, Yellows = 72, Reds = 3, Penalties = 6, AvgScore = 7.7, TrendChange = -0.1 },
            }
        };

        // Use explicit IDs via identity insert
        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Referees] ON");
        db.Referees.AddRange(oliver, taylor, brooks, roberts, phillips, mitchell);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Referees] OFF");

        // ════════════════════════════════════════════
        // 3. FIXTURES
        // ════════════════════════════════════════════S
        var fixtures = new List<FixtureEntity>
        {
            // Matchday 26: Sat 15 Feb 2026
            new() {  DivisionId = 1, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(12,30,0), Matchday = 26, HomeTeam = "Arsenal", AwayTeam = "Chelsea", Venue = "Emirates Stadium" },
            new() {  DivisionId = 1, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 26, HomeTeam = "Liverpool", AwayTeam = "Manchester United", Venue = "Anfield" },
            new() {  DivisionId = 1, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 26, HomeTeam = "Aston Villa", AwayTeam = "Newcastle", Venue = "Villa Park" },
            new() {  DivisionId = 1, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 26, HomeTeam = "Wolverhampton", AwayTeam = "Brighton", Venue = "Molineux Stadium" },
            new() {  DivisionId = 1, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(17,30,0), Matchday = 26, HomeTeam = "Manchester City", AwayTeam = "Tottenham", Venue = "Etihad Stadium" },
            new() {  DivisionId = 2, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 32, HomeTeam = "Cardiff City", AwayTeam = "Swansea City", Venue = "Cardiff City Stadium" },
            new() {  DivisionId = 2, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 32, HomeTeam = "Leeds United", AwayTeam = "Sunderland", Venue = "Elland Road" },
            new() {  DivisionId = 2, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 32, HomeTeam = "Sheffield United", AwayTeam = "West Brom", Venue = "Bramall Lane" },
            new() {  DivisionId = 3, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 32, HomeTeam = "Wrexham", AwayTeam = "Bolton", Venue = "Racecourse Ground" },
            new() {  DivisionId = 3, Date = new DateTime(2026,2,15), KickOff = new TimeSpan(15,0,0), Matchday = 32, HomeTeam = "Barnsley", AwayTeam = "Charlton", Venue = "Oakwell" },
            // Sun 16 Feb 2026
            new() { DivisionId = 1, Date = new DateTime(2026,2,16), KickOff = new TimeSpan(14,0,0), Matchday = 26, HomeTeam = "Everton", AwayTeam = "West Ham", Venue = "Goodison Park" },
            new() { DivisionId = 1, Date = new DateTime(2026,2,16), KickOff = new TimeSpan(16,30,0), Matchday = 26, HomeTeam = "Crystal Palace", AwayTeam = "Fulham", Venue = "Selhurst Park" },
            // Matchday 27: Sat 22 Feb 2026
            new() { DivisionId = 1, Date = new DateTime(2026,2,22), KickOff = new TimeSpan(12,30,0), Matchday = 27, HomeTeam = "Chelsea", AwayTeam = "Liverpool", Venue = "Stamford Bridge" },
            new() { DivisionId = 1, Date = new DateTime(2026,2,22), KickOff = new TimeSpan(15,0,0), Matchday = 27, HomeTeam = "Tottenham", AwayTeam = "Arsenal", Venue = "Tottenham Hotspur Stadium" },
            new() { DivisionId = 1, Date = new DateTime(2026,2,22), KickOff = new TimeSpan(15,0,0), Matchday = 27, HomeTeam = "Newcastle", AwayTeam = "Wolverhampton", Venue = "St James' Park" },
            new() { DivisionId = 2, Date = new DateTime(2026,2,22), KickOff = new TimeSpan(15,0,0), Matchday = 33, HomeTeam = "Swansea City", AwayTeam = "Leeds United", Venue = "Swansea.com Stadium" },
            new() { DivisionId = 2, Date = new DateTime(2026,2,22), KickOff = new TimeSpan(15,0,0), Matchday = 33, HomeTeam = "Sunderland", AwayTeam = "Cardiff City", Venue = "Stadium of Light" },
            new() { DivisionId = 3, Date = new DateTime(2026,2,22), KickOff = new TimeSpan(15,0,0), Matchday = 33, HomeTeam = "Bolton", AwayTeam = "Barnsley", Venue = "Toughsheet Community Stadium" },
            // FA Cup: Wed 25 Feb 2026
            new() { DivisionId = 5, Date = new DateTime(2026,2,25), KickOff = new TimeSpan(19,45,0), Matchday = 5, HomeTeam = "Manchester United", AwayTeam = "Nottingham Forest", Venue = "Old Trafford" },
            new() { DivisionId = 5, Date = new DateTime(2026,2,25), KickOff = new TimeSpan(20,0,0), Matchday = 5, HomeTeam = "Brighton", AwayTeam = "Stoke City", Venue = "Amex Stadium" },
        };

        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Fixtures] ON");
        db.Fixtures.AddRange(fixtures);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [Fixtures] OFF");

        // ════════════════════════════════════════════
        // 4. OFFICIAL ASSIGNMENTS
        // ════════════════════════════════════════════
        var assignments = new List<OfficialAssignmentEntity>
        {
            new() { FixtureId = 1, RefereeId = taylor.Id, Role = OfficialRole.Referee },
            new() { FixtureId = 1, RefereeId = brooks.Id, Role = OfficialRole.MatchOfficial },
            new() { FixtureId = 2, RefereeId = oliver.Id, Role = OfficialRole.Referee },
            new() { FixtureId = 6, RefereeId = roberts.Id, Role = OfficialRole.Referee },
            new() { FixtureId = 9, RefereeId = phillips.Id, Role = OfficialRole.Referee },
        };
        db.OfficialAssignments.AddRange(assignments);
        await db.SaveChangesAsync();

        // ════════════════════════════════════════════
        // 5. HISTORICAL MATCHES (referee profile)
        // ════════════════════════════════════════════
        var matches = new List<MatchEntity>
        {
            new() { Date = new DateTime(2026,2,12), HomeTeam = "Liverpool", AwayTeam = "Man City", Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague, HomeScore = 2, AwayScore = 1, Role = "Referee", YellowCards = 4, RedCards = 0, AssessmentScore = 8.6m, RefereeId = oliver.Id,
                Incidents = new List<MatchIncidentEntity>
                {
                    new() { Minute = 64, Description = "Penalty awarded", IncidentType = "Penalty", Notes = "Handball in the box" },
                }
            },
            new() { Date = new DateTime(2026,2,8), HomeTeam = "Arsenal", AwayTeam = "Tottenham", Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague, HomeScore = 3, AwayScore = 0, Role = "Referee", YellowCards = 3, RedCards = 1, AssessmentScore = 8.2m, RefereeId = oliver.Id,
                Incidents = new List<MatchIncidentEntity>
                {
                    new() { Minute = 38, Description = "Red card", IncidentType = "RedCard", Notes = "Serious foul play, studs up tackle" },
                    new() { Minute = 72, Description = "VAR review – goal disallowed", IncidentType = "VAR", Notes = "Offside in the build-up" },
                }
            },
            new() { Date = new DateTime(2026,2,1), HomeTeam = "Aston Villa", AwayTeam = "Chelsea", Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague, HomeScore = 1, AwayScore = 1, Role = "Referee", YellowCards = 5, RedCards = 0, AssessmentScore = 7.9m, RefereeId = oliver.Id },
            new() { Date = new DateTime(2026,1,22), HomeTeam = "Germany", AwayTeam = "France", Competition = "UEFA Nations League", CompetitionTier = RefereeTier.FifaInternational, HomeScore = 0, AwayScore = 2, Role = "Referee", YellowCards = 2, RedCards = 0, AssessmentScore = 9.1m, RefereeId = oliver.Id },
            new() { Date = new DateTime(2026,1,18), HomeTeam = "Newcastle", AwayTeam = "Man Utd", Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague, HomeScore = 2, AwayScore = 2, Role = "Referee", YellowCards = 6, RedCards = 0, AssessmentScore = 7.4m, RefereeId = oliver.Id,
                Incidents = new List<MatchIncidentEntity>
                {
                    new() { Minute = 23, Description = "Penalty awarded", IncidentType = "Penalty", Notes = "Foul on attacker in the box" },
                    new() { Minute = 81, Description = "Penalty awarded", IncidentType = "Penalty", Notes = "Shirt pull at corner" },
                }
            },
        };
        db.Matches.AddRange(matches);
        await db.SaveChangesAsync();

        // ════════════════════════════════════════════
        // 6. ASSESSMENTS
        // ════════════════════════════════════════════
        await SeedAssessments(db, oliver, taylor, brooks, roberts, phillips);

        // ════════════════════════════════════════════
        // 7. MATCH OFFICIAL REPORTS
        // ════════════════════════════════════════════
        await SeedMatchReports(db, oliver, taylor, brooks, roberts, phillips);

        // ════════════════════════════════════════════
        // 8. INTERVENTION REVIEWS
        // ════════════════════════════════════════════
        await SeedInterventionReviews(db);

        // ════════════════════════════════════════════
        // 9. ACTIVITY LOG
        // ════════════════════════════════════════════
        db.ActivityLog.AddRange(
            new ActivityLogEntity { Timestamp = DateTime.UtcNow.AddHours(-2), HighlightName = "David Phillips", Message = "registered as new referee (League 1)", IconType = "cyan" },
            new ActivityLogEntity { Timestamp = DateTime.UtcNow.AddHours(-5), HighlightName = "Michael Oliver", Message = "assessment completed — Score:", HighlightValue = "8.4", HighlightColor = "--accent-lime", IconType = "lime" },
            new ActivityLogEntity { Timestamp = DateTime.UtcNow.AddDays(-1), HighlightName = "Anthony Taylor", Message = "appointed to Arsenal vs Chelsea — 15 Feb", IconType = "orange" },
            new ActivityLogEntity { Timestamp = DateTime.UtcNow.AddDays(-1).AddHours(-3), HighlightName = "Simon Roberts", Message = "status changed to", HighlightValue = "Probation", HighlightColor = "--accent-orange", IconType = "cyan" },
            new ActivityLogEntity { Timestamp = DateTime.UtcNow.AddDays(-2), HighlightName = "", Message = "Matchday 25 post-match reports generated — 10 matches", IconType = "lime" },
            new ActivityLogEntity { Timestamp = DateTime.UtcNow.AddDays(-3), HighlightName = "John Brooks", Message = "profile updated — new photo uploaded", IconType = "orange" }
        );
        await db.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════
    // ASSESSMENT SEED DATA
    // ════════════════════════════════════════════════════

    private static async Task SeedAssessments(LumiaroDbContext db,
        RefereeEntity oliver, RefereeEntity taylor, RefereeEntity brooks,
        RefereeEntity roberts, RefereeEntity phillips)
    {
        // Assessment 1: Taylor — Arsenal vs Chelsea
        var a1 = new AssessmentEntity
        {
            FixtureId = 1, RefereeId = taylor.Id, OfficialRole = OfficialRole.Referee,
            Status = AssessmentStatus.Finalised, OverallScore = 8.1,
            SummaryNotes = "Strong performance in a high-profile London derby. Anthony Taylor managed the game well despite several contentious incidents. Decision-making was generally sound with one debatable penalty call that divided the panel.",
            Recommendations = "Continue developing positioning for penalty area incidents. Review sight-line management when play is transitioning quickly through midfield.",
            CreatedBy = "PGMOL Assessment Panel", KmiPanelNotes = "Panel split on the 55th-minute penalty appeal. Majority view was that the referee's position was suboptimal for the decision.",
            TechnicalReport = new TechnicalReportEntity
            {
                ReviewerName = "Mike Riley",
                LawScore = 8.0m, LawNotes = "Generally excellent application of the laws. The simulation booking was brave and correct.", LawStrengths = "Confident decision-making under pressure", LawAreasForImprovement = "Penalty area incident positioning",
                PositioningScore = 7.5m, PositioningNotes = "Good movement patterns in open play but sight-line was compromised for the key penalty area incident at 55'.", PositioningStrengths = "Dynamic movement, kept up with play", PositioningAreasForImprovement = "Wider position for box incidents",
                FitnessScore = 8.5m, FitnessNotes = "Excellent physical performance throughout. No drop-off in the second half.", FitnessStrengths = "Consistent work rate, good sprint recovery", FitnessAreasForImprovement = "N/A",
                ControlScore = 8.2m, ControlNotes = "Managed a heated derby well. Good verbal communication with players.", ControlStrengths = "Calm demeanour, effective communication", ControlAreasForImprovement = "Could be more proactive in managing bench areas"
            },
            DelegateReport = new DelegateReportEntity
            {
                DelegateName = "Dermot Gallagher",
                ManagementScore = 8.3m, ManagementNotes = "Managed the tempo well throughout.", ManagementStrengths = "Strong presence, players respected decisions", ManagementAreasForImprovement = "Touchline management",
                CommunicationScore = 8.0m, CommunicationNotes = "Clear and decisive signals. Good body language.", CommunicationStrengths = "Eye contact with players when communicating", CommunicationAreasForImprovement = "More proactive communication before set pieces",
                ConsistencyScore = 7.8m, ConsistencyNotes = "Generally consistent, though foul threshold changed slightly in the second half.", ConsistencyStrengths = "Foul recognition excellent", ConsistencyAreasForImprovement = "Maintain threshold across halves"
            },
            MatchStatReport = new MatchStatReportEntity
            {
                YellowCards = 3, RedCards = 0, PenaltiesAwarded = 0, FoulsGiven = 22, OffsidesCalled = 4,
                DistanceCoveredKm = 11.8m, AvgSprintSpeedKmh = 24.2m, HighIntensityRuns = 48,
                IncidentNotes = "Key incident at 55' — penalty appeal waved away.",
                VarInteractionNotes = "One check for the penalty incident — no overturn."
            },
            KeyMatchIncidents = new List<KeyMatchIncidentEntity>
            {
                new() { Minute = 23, Description = "Penalty awarded to Arsenal for handball in the box", DecisionType = KmiDecisionType.Penalty, OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false, PanelVotesCorrect = 4, PanelVotesIncorrect = 1, Rationale = "Clear handball, arm in unnatural position.", SortOrder = 0 },
                new() { Minute = 55, Description = "Penalty claim waved away — Chelsea challenge on Saka", DecisionType = KmiDecisionType.Penalty, OriginalDecisionCorrect = false, VarInterventionRequired = true, VarInterventionMade = false, PanelVotesCorrect = 2, PanelVotesIncorrect = 3, Rationale = "Contact visible on slow motion. Panel majority felt penalty should have been awarded.", SortOrder = 1 },
                new() { Minute = 78, Description = "Red card shown for serious foul play", DecisionType = KmiDecisionType.RedCard, OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false, PanelVotesCorrect = 5, PanelVotesIncorrect = 0, Rationale = "Excessive force, studs showing. Clear red.", SortOrder = 2 },
            }
        };

        // Assessment 2: Oliver — Liverpool vs Man Utd
        var a2 = new AssessmentEntity
        {
            FixtureId = 2, RefereeId = oliver.Id, OfficialRole = OfficialRole.Referee,
            Status = AssessmentStatus.Finalised, OverallScore = 8.6,
            SummaryNotes = "Excellent performance from Michael Oliver in a high-intensity fixture. Managed the rivalry atmosphere superbly.",
            Recommendations = "Maintain current form. Consider for upcoming Champions League knockout appointments.",
            CreatedBy = "PGMOL Assessment Panel",
            TechnicalReport = new TechnicalReportEntity
            {
                ReviewerName = "Mike Riley", LawScore = 8.8m, PositioningScore = 8.4m, FitnessScore = 9.0m, ControlScore = 8.5m
            },
            DelegateReport = new DelegateReportEntity
            {
                DelegateName = "Dermot Gallagher", ManagementScore = 8.7m, CommunicationScore = 8.5m, ConsistencyScore = 8.6m
            },
            MatchStatReport = new MatchStatReportEntity
            {
                YellowCards = 4, RedCards = 0, PenaltiesAwarded = 1, FoulsGiven = 26, OffsidesCalled = 3,
                DistanceCoveredKm = 12.1m, AvgSprintSpeedKmh = 25.1m, HighIntensityRuns = 52
            },
            KeyMatchIncidents = new List<KeyMatchIncidentEntity>
            {
                new() { Minute = 34, Description = "Goal allowed — tight offside call", DecisionType = KmiDecisionType.GoalAllowed, OriginalDecisionCorrect = true, PanelVotesCorrect = 5, PanelVotesIncorrect = 0, SortOrder = 0 },
                new() { Minute = 61, Description = "Penalty awarded for trip in the box", DecisionType = KmiDecisionType.Penalty, OriginalDecisionCorrect = true, PanelVotesCorrect = 4, PanelVotesIncorrect = 1, SortOrder = 1 },
            }
        };

        // Assessment 3: Brooks — 4th official (Arsenal vs Chelsea)
        var a3 = new AssessmentEntity
        {
            FixtureId = 1, RefereeId = brooks.Id, OfficialRole = OfficialRole.MatchOfficial,
            Status = AssessmentStatus.Reviewed, OverallScore = 7.8,
            SummaryNotes = "Solid performance as 4th official. Managed technical areas well.",
            CreatedBy = "PGMOL Assessment Panel",
            KeyMatchIncidents = new List<KeyMatchIncidentEntity>()
        };

        // Assessment 4: Roberts — Cardiff vs Swansea
        var a4 = new AssessmentEntity
        {
            FixtureId = 6, RefereeId = roberts.Id, OfficialRole = OfficialRole.Referee,
            Status = AssessmentStatus.Submitted, OverallScore = 6.8,
            SummaryNotes = "Mixed performance in the South Wales derby. Several decisions lacked consistency.",
            Recommendations = "Review foul threshold consistency. Additional mentoring recommended before next derby appointment.",
            CreatedBy = "FAW Assessment Panel",
            TechnicalReport = new TechnicalReportEntity
            {
                ReviewerName = "David Elleray", LawScore = 6.5m, PositioningScore = 7.0m, FitnessScore = 7.5m, ControlScore = 6.2m,
                LawNotes = "Several marginal calls went against the home side. The penalty decision at 32' was correct but the missed offside at 67' was a significant error.",
                ControlNotes = "Lost control of the game in the final 15 minutes. Too many stoppages broke the flow."
            },
            DelegateReport = new DelegateReportEntity
            {
                DelegateName = "Mark Halsey", ManagementScore = 6.5m, CommunicationScore = 7.0m, ConsistencyScore = 6.0m
            },
            MatchStatReport = new MatchStatReportEntity
            {
                YellowCards = 6, RedCards = 1, PenaltiesAwarded = 1, FoulsGiven = 32, OffsidesCalled = 5,
                DistanceCoveredKm = 10.9m, AvgSprintSpeedKmh = 22.8m, HighIntensityRuns = 38
            },
            KeyMatchIncidents = new List<KeyMatchIncidentEntity>
            {
                new() { Minute = 32, Description = "Penalty awarded to Cardiff for foul in the box", DecisionType = KmiDecisionType.Penalty, OriginalDecisionCorrect = true, PanelVotesCorrect = 4, PanelVotesIncorrect = 1, SortOrder = 0 },
                new() { Minute = 67, Description = "Goal allowed — attacker clearly offside", DecisionType = KmiDecisionType.GoalAllowed, OriginalDecisionCorrect = false, PanelVotesCorrect = 0, PanelVotesIncorrect = 5, Rationale = "Clear and obvious error.", SortOrder = 1 },
                new() { Minute = 85, Description = "Second yellow card — simulation", DecisionType = KmiDecisionType.Foul, OriginalDecisionCorrect = true, PanelVotesCorrect = 3, PanelVotesIncorrect = 2, SortOrder = 2 },
            }
        };

        // Assessment 5: Phillips — Wrexham vs Bolton
        var a5 = new AssessmentEntity
        {
            FixtureId = 9, RefereeId = phillips.Id, OfficialRole = OfficialRole.Referee,
            Status = AssessmentStatus.Draft, OverallScore = 7.6,
            SummaryNotes = "Good performance for League One level. Shows promise for progression.",
            CreatedBy = "EFL Assessment Panel",
            TechnicalReport = new TechnicalReportEntity
            {
                ReviewerName = "Graham Poll", LawScore = 7.5m, PositioningScore = 7.8m, FitnessScore = 8.0m, ControlScore = 7.2m
            },
            MatchStatReport = new MatchStatReportEntity
            {
                YellowCards = 3, RedCards = 0, PenaltiesAwarded = 1, FoulsGiven = 18, OffsidesCalled = 2,
                DistanceCoveredKm = 11.2m, AvgSprintSpeedKmh = 23.5m, HighIntensityRuns = 42
            },
            KeyMatchIncidents = new List<KeyMatchIncidentEntity>
            {
                new() { Minute = 71, Description = "Penalty awarded for handball", DecisionType = KmiDecisionType.Handball, OriginalDecisionCorrect = true, PanelVotesCorrect = 5, PanelVotesIncorrect = 0, SortOrder = 0 },
            }
        };

        db.Assessments.AddRange(a1, a2, a3, a4, a5);
        await db.SaveChangesAsync();
    }

    // ════════════════════════════════════════════════════
    // MATCH OFFICIAL REPORT SEED DATA
    // ════════════════════════════════════════════════════

    private static async Task SeedMatchReports(LumiaroDbContext db,
        RefereeEntity oliver, RefereeEntity taylor, RefereeEntity brooks,
        RefereeEntity roberts, RefereeEntity phillips)
    {
        var report1 = new MatchOfficialReportEntity
        {
            FixtureId = 1, RefereeId = taylor.Id, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Arsenal vs Chelsea", MatchDate = new DateTime(2026, 2, 15),
            Venue = "Emirates Stadium", RefereeName = "Anthony Taylor", RefereeCode = "REF-0018",
            RefereeInitials = "AT", RefereeTier = RefereeTier.FifaInternational, CompetitionTier = RefereeTier.PremierLeague,
            HomeScore = 2, AwayScore = 1, Status = ReportStatus.Published,
            OverallRating = 8.1m, AuthorName = "Mike Riley", AuthorRole = "PGMOL Chief",
            ExecutiveSummary = "Strong overall performance in a high-profile London derby.",
            DevelopmentPriorities = "Positioning for penalty area incidents. Sight-line management.",
            Sections = new List<ReportSectionEntity>
            {
                new() { Title = "Positioning & Movement", Rating = 7.5m, Narrative = "Good dynamic movement in open play. Sight-line was compromised for the 55th-minute penalty incident due to player screening.", SortOrder = 0 },
                new() { Title = "Decision Making", Rating = 8.0m, Narrative = "Generally excellent decision-making. The simulation call at 32' was brave and correct. Panel split on the 55' penalty appeal.", SortOrder = 1 },
                new() { Title = "Match Control", Rating = 8.2m, Narrative = "Managed the derby atmosphere effectively. Good verbal management of the touchline incident at 62'.", SortOrder = 2 },
                new() { Title = "Communication", Rating = 8.5m, Narrative = "Clear and decisive signalling throughout. Excellent body language and eye contact.", SortOrder = 3 },
                new() { Title = "Fitness & Work Rate", Rating = 8.5m, Narrative = "No drop-off in the second half. Covered 11.8km with excellent sprint recovery.", SortOrder = 4 },
                new() { Title = "Application of Laws", Rating = 8.0m, Narrative = "Correct application in all but the debatable penalty incident.", SortOrder = 5 },
            },
            Events = new List<ReportEventEntity>
            {
                new() { Minute = 12, EventType = EventType.Foul, Description = "Midfield challenge correctly penalised. Taylor played advantage initially before bringing play back.", Impact = EventImpact.Low, OfficialInvolved = true, OfficialResponse = "Good initial advantage, correctly brought back when advantage not materialised." },
                new() { Minute = 23, EventType = EventType.Goal, Description = "Arsenal open the scoring through Saka. Goal correctly allowed.", Impact = EventImpact.Medium, OfficialInvolved = false },
                new() { Minute = 32, EventType = EventType.YellowCard, Description = "Havertz booked for simulation in the penalty area. Brave and correct decision by Taylor.", Impact = EventImpact.High, OfficialInvolved = true, OfficialResponse = "Clear sight-line to the incident. Correct call — no contact from defender." },
                new() { Minute = 41, EventType = EventType.YellowCard, Description = "Rice cautioned for persistent infringement — fourth foul of the half.", Impact = EventImpact.Medium, OfficialInvolved = true, OfficialResponse = "Well-tracked foul count. Correctly cautioned at the right time." },
                new() { Minute = 55, EventType = EventType.Penalty, Description = "Strong penalty appeal from Chelsea after contact in the box. Taylor waves play on. Panel split 2-3 on the decision.", Impact = EventImpact.Critical, OfficialInvolved = true, OfficialResponse = "Sight-line partially blocked. Made the call with conviction but positioning could have been improved.", Teams = "Away", Accuracy = DecisionAccuracy.Debatable },
                new() { Minute = 62, EventType = EventType.Foul, Description = "Touchline incident as Chelsea bench protests. Taylor manages situation calmly.", Impact = EventImpact.Medium, OfficialInvolved = true, OfficialResponse = "Good verbal management. No cards necessary — talked the situation down." },
                new() { Minute = 67, EventType = EventType.Offside, Description = "Tight offside call against Chelsea. Correct on review. Brief hesitation before signalling.", Impact = EventImpact.High, OfficialInvolved = true, OfficialResponse = "Consulted AR1 before confirming. The slight delay was noted.", Teams = "Away" },
                new() { Minute = 74, EventType = EventType.Goal, Description = "Chelsea equaliser through Palmer. Goal correctly allowed after brief check.", Impact = EventImpact.Medium, OfficialInvolved = false },
                new() { Minute = 83, EventType = EventType.Goal, Description = "Arsenal winner from Saliba header. Correctly allowed — no foul in the build-up.", Impact = EventImpact.High, OfficialInvolved = false },
                new() {  Minute = 90, AddedTime = 2, EventType = EventType.YellowCard, Description = "Palmer cautioned for time-wasting at a free kick.", Impact = EventImpact.Low, OfficialInvolved = true, OfficialResponse = "Correct application. Clear delaying tactic.", Teams = "Away" },
            },
            VideoClips = new List<VideoClipEntity>
            {
                new() { Title = "55' Penalty Appeal", Timestamp = "54:38", Notes = "Key incident — panel divided on this call.", SortOrder = 0 },
                new() { Title = "32' Simulation Booking", Timestamp = "31:52", Notes = "Brave and correct call by the referee.", SortOrder = 1 },
            }
        };

        var report2 = new MatchOfficialReportEntity
        {
            FixtureId = 2, RefereeId = oliver.Id, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Liverpool vs Manchester United", MatchDate = new DateTime(2026, 2, 15),
            Venue = "Anfield", RefereeName = "Michael Oliver", RefereeCode = "REF-0042",
            RefereeInitials = "MO", RefereeTier = RefereeTier.FifaInternational, CompetitionTier = RefereeTier.PremierLeague,
            HomeScore = 3, AwayScore = 1, Status = ReportStatus.Published,
            OverallRating = 8.6m, AuthorName = "Howard Webb", AuthorRole = "PGMOL Referee Development",
            ExecutiveSummary = "Excellent performance from Oliver in a high-intensity fixture.",
            Sections = new List<ReportSectionEntity>
            {
                new() { Title = "Positioning & Movement", Rating = 8.4m, Narrative = "Excellent positioning throughout. Always found the right angle.", SortOrder = 0 },
                new() { Title = "Decision Making", Rating = 8.8m, Narrative = "Consistently correct decisions under pressure.", SortOrder = 1 },
                new() { Title = "Match Control", Rating = 8.5m, Narrative = "Managed the rivalry atmosphere superbly.", SortOrder = 2 },
                new() { Title = "Communication", Rating = 8.3m, Narrative = "Effective communication with captains and management.", SortOrder = 3 },
                new() { Title = "Fitness & Work Rate", Rating = 9.0m, Narrative = "Outstanding physical performance. 12.1km covered.", SortOrder = 4 },
                new() { Title = "Application of Laws", Rating = 8.8m, Narrative = "Flawless application of laws of the game.", SortOrder = 5 },
            },
            Events = new List<ReportEventEntity>
            {
                new() { Minute = 8, EventType = EventType.Foul, Description = "Early challenge sets the tone. Oliver correctly penalises and has a word.", Impact = EventImpact.Low, OfficialInvolved = true },
                new() { Minute = 22, EventType = EventType.Goal, Description = "Liverpool open scoring. Correctly allowed.", Impact = EventImpact.Medium, OfficialInvolved = false },
                new() { Minute = 34, EventType = EventType.Offside, Description = "Tight offside call. Correct on review.", Impact = EventImpact.High, OfficialInvolved = true },
                new() { Minute = 56, EventType = EventType.YellowCard, Description = "Cautioned for reckless challenge.", Impact = EventImpact.Medium, OfficialInvolved = true },
                new() { Minute = 71, EventType = EventType.Penalty, Description = "Clear penalty for trip in the box.", Impact = EventImpact.High, OfficialInvolved = true, Accuracy = DecisionAccuracy.Correct },
            }
        };

        var report3 = new MatchOfficialReportEntity
        {
            FixtureId = 6, RefereeId = roberts.Id, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Cardiff City vs Swansea City", MatchDate = new DateTime(2026, 2, 15),
            Venue = "Cardiff City Stadium", RefereeName = "Simon Roberts", RefereeCode = "REF-0127",
            RefereeInitials = "SR", RefereeTier = RefereeTier.Championship, CompetitionTier = RefereeTier.Championship,
            HomeScore = 2, AwayScore = 2, Status = ReportStatus.Submitted,
            OverallRating = 6.8m, AuthorName = "David Elleray", AuthorRole = "FAW Referee Observer",
            ExecutiveSummary = "Below par in a heated South Wales derby. Consistency was the main concern.",
            Sections = new List<ReportSectionEntity>
            {
                new() { Title = "Positioning & Movement", Rating = 7.0m, SortOrder = 0 },
                new() { Title = "Decision Making", Rating = 6.5m, SortOrder = 1 },
                new() { Title = "Match Control", Rating = 6.2m, SortOrder = 2 },
                new() { Title = "Communication", Rating = 7.0m, SortOrder = 3 },
                new() { Title = "Fitness & Work Rate", Rating = 7.5m, SortOrder = 4 },
                new() { Title = "Application of Laws", Rating = 6.5m, SortOrder = 5 },
            },
            Events = new List<ReportEventEntity>
            {
                new() {Minute = 15, EventType = EventType.Foul, Description = "Marginal foul call against Cardiff.", Impact = EventImpact.Low, OfficialInvolved = true },
                new() {Minute = 32, EventType = EventType.Penalty, Description = "Penalty awarded to Cardiff.", Impact = EventImpact.High, OfficialInvolved = true, Teams = "Home", Accuracy = DecisionAccuracy.Correct },
                new() {Minute = 55, EventType = EventType.YellowCard, Description = "Late challenge cautioned.", Impact = EventImpact.Medium, OfficialInvolved = true },
                new() {Minute = 67, EventType = EventType.Goal, Description = "Goal allowed — attacker was offside.", Impact = EventImpact.Critical, OfficialInvolved = true, Accuracy = DecisionAccuracy.Incorrect },
                new() {Minute = 78, EventType = EventType.YellowCard, Description = "Time-wasting caution.", Impact = EventImpact.Low, OfficialInvolved = true },
            }
        };

        var report4 = new MatchOfficialReportEntity
        {
            FixtureId = 9, RefereeId = phillips.Id, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Wrexham vs Bolton", MatchDate = new DateTime(2026, 2, 15),
            Venue = "Racecourse Ground", RefereeName = "David Phillips", RefereeCode = "REF-0134",
            RefereeInitials = "DP", RefereeTier = RefereeTier.LeagueOne, CompetitionTier = RefereeTier.LeagueOne,
            HomeScore = 1, AwayScore = 0, Status = ReportStatus.Draft,
            OverallRating = 7.6m, AuthorName = "Graham Poll", AuthorRole = "EFL Referee Observer",
            ExecutiveSummary = "Promising performance showing good potential for progression.",
            Sections = new List<ReportSectionEntity>
            {
                new() { Title = "Positioning & Movement", Rating = 7.8m, SortOrder = 0 },
                new() { Title = "Decision Making", Rating = 7.5m, SortOrder = 1 },
                new() { Title = "Match Control", Rating = 7.2m, SortOrder = 2 },
                new() { Title = "Communication", Rating = 7.5m, SortOrder = 3 },
                new() { Title = "Fitness & Work Rate", Rating = 8.0m, SortOrder = 4 },
                new() { Title = "Application of Laws", Rating = 7.5m, SortOrder = 5 },
            },
            Events = new List<ReportEventEntity>
            {
                new() {Minute = 28, EventType = EventType.Foul, Description = "Good advantage played.", Impact = EventImpact.Low, OfficialInvolved = true },
                new() {Minute = 44, EventType = EventType.YellowCard, Description = "Tactical foul correctly cautioned.", Impact = EventImpact.Medium, OfficialInvolved = true },
                new() {Minute = 66, EventType = EventType.Goal, Description = "Wrexham score. Correctly allowed.", Impact = EventImpact.Medium, OfficialInvolved = false },
                new() {Minute = 82, EventType = EventType.YellowCard, Description = "Dissent caution.", Impact = EventImpact.Medium, OfficialInvolved = true },
            }
        };

        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [MatchOfficialReports] ON");
        db.MatchOfficialReports.AddRange(report1, report2, report3, report4);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [MatchOfficialReports] OFF");

        // Also set explicit IDs on events for intervention review FK references
        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [ReportEvents] ON");
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [ReportEvents] OFF");
    }

    // ════════════════════════════════════════════════════
    // INTERVENTION REVIEW SEED DATA
    // ════════════════════════════════════════════════════

    private static async Task SeedInterventionReviews(LumiaroDbContext db)
    {
        var reviewers = new[] {
            ("Mike Dean", "Observer"),
            ("Howard Webb", "Referee Manager"),
            ("Mark Clattenburg", "Senior Referee"),
            ("Chris Foy", "Observer"),
        };

        var rng = new Random(42);
        var reviews = new List<InterventionReviewEntity>();

        // Reviews for Report 1 (events 1-10)
        foreach (var (name, role) in reviewers)
        {
            for (int eventId = 1; eventId <= 10; eventId++)
            {
                var (baseAcc, baseCon, baseCom, baseMH) = eventId switch
                {
                    5 => (55, 60, 70, 65),
                    7 => (70, 65, 75, 72),
                    3 => (88, 80, 78, 82),
                    _ => (82, 78, 80, 79),
                };

                var variation = name switch
                {
                    "Mike Dean" => 3,
                    "Howard Webb" => -2,
                    "Mark Clattenburg" => 0,
                    _ => 1,
                };

                reviews.Add(new InterventionReviewEntity
                {
                    ReportId = 1, EventId = eventId,
                    ReviewerName = name, ReviewerRole = role,
                    AccuracyScore = Clamp(baseAcc + variation + rng.Next(-6, 7)),
                    ConsistencyScore = Clamp(baseCon + variation + rng.Next(-5, 6)),
                    CommunicationScore = Clamp(baseCom + variation + rng.Next(-5, 6)),
                    MatchHandlingScore = Clamp(baseMH + variation + rng.Next(-5, 6)),
                    Comment = eventId == 5 ? GetPenaltyComment(name) : null,
                    ReviewedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 5)),
                });
            }
        }

        // Reviews for Report 2 (events 11-15)
        foreach (var (name, role) in reviewers.Take(2))
        {
            for (int eventId = 11; eventId <= 15; eventId++)
            {
                reviews.Add(new InterventionReviewEntity
                {
                    ReportId = 2, EventId = eventId,
                    ReviewerName = name, ReviewerRole = role,
                    AccuracyScore = Clamp(75 + rng.Next(-8, 9)),
                    ConsistencyScore = Clamp(72 + rng.Next(-7, 8)),
                    CommunicationScore = Clamp(78 + rng.Next(-6, 7)),
                    MatchHandlingScore = Clamp(74 + rng.Next(-7, 8)),
                    ReviewedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 3)),
                });
            }
        }

        db.InterventionReviews.AddRange(reviews);
        await db.SaveChangesAsync();
    }

    private static int Clamp(int v) => Math.Max(0, Math.Min(100, v));

    private static string? GetPenaltyComment(string reviewer) => reviewer switch
    {
        "Mike Dean" => "Difficult call in real time. The contact was marginal but I feel a penalty could have been given. Positioning was the key factor.",
        "Howard Webb" => "The referee's positioning was suboptimal for this incident. A wider angle would have provided a clearer view of the contact.",
        "Mark Clattenburg" => "Brave decision to wave play on. I can see both sides — the slow motion shows contact but in real time the attacker appeared to go down easily.",
        "Chris Foy" => "Borderline call. The referee backed his initial judgement which is important for match management, even if the decision itself is debatable.",
        _ => null,
    };
}
