using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LumiaroAdmin.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HighlightName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HighlightValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HighlightColor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IconType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Referees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalMatches = table.Column<int>(type: "int", nullable: false),
                    AverageScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    AccuracyPercent = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    TotalSeasons = table.Column<int>(type: "int", nullable: false),
                    CurrentSeasonMatches = table.Column<int>(type: "int", nullable: false),
                    CurrentSeasonAvgScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    CurrentSeasonYellows = table.Column<int>(type: "int", nullable: false),
                    CurrentSeasonReds = table.Column<int>(type: "int", nullable: false),
                    CurrentSeasonPenalties = table.Column<int>(type: "int", nullable: false),
                    CurrentSeasonIncidents = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fixtures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivisionId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    KickOff = table.Column<TimeSpan>(type: "time", nullable: false),
                    HomeTeam = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AwayTeam = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Matchday = table.Column<int>(type: "int", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixtures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fixtures_Divisions_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Divisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefereeCareerEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefereeId = table.Column<int>(type: "int", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefereeCareerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefereeCareerEntries_Referees_RefereeId",
                        column: x => x.RefereeId,
                        principalTable: "Referees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefereeSeasonStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefereeId = table.Column<int>(type: "int", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Matches = table.Column<int>(type: "int", nullable: false),
                    Yellows = table.Column<int>(type: "int", nullable: false),
                    Reds = table.Column<int>(type: "int", nullable: false),
                    Penalties = table.Column<int>(type: "int", nullable: false),
                    AvgScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    TrendChange = table.Column<decimal>(type: "decimal(4,1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefereeSeasonStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefereeSeasonStats_Referees_RefereeId",
                        column: x => x.RefereeId,
                        principalTable: "Referees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FixtureId = table.Column<int>(type: "int", nullable: false),
                    RefereeId = table.Column<int>(type: "int", nullable: false),
                    OfficialRole = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OverallScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    SummaryNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    KmiPanelNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessments_Fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assessments_Referees_RefereeId",
                        column: x => x.RefereeId,
                        principalTable: "Referees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FixtureId = table.Column<int>(type: "int", nullable: true),
                    RefereeId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    HomeTeam = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AwayTeam = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Competition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompetitionTier = table.Column<int>(type: "int", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YellowCards = table.Column<int>(type: "int", nullable: false),
                    RedCards = table.Column<int>(type: "int", nullable: false),
                    AssessmentScore = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    Assistants = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FourthOfficial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixtures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matches_Referees_RefereeId",
                        column: x => x.RefereeId,
                        principalTable: "Referees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchOfficialReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FixtureId = table.Column<int>(type: "int", nullable: false),
                    RefereeId = table.Column<int>(type: "int", nullable: false),
                    OfficialRole = table.Column<int>(type: "int", nullable: false),
                    MatchTitle = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    MatchDate = table.Column<DateTime>(type: "date", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RefereeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RefereeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RefereeInitials = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RefereeTier = table.Column<int>(type: "int", nullable: false),
                    CompetitionTier = table.Column<int>(type: "int", nullable: false),
                    HomeScore = table.Column<int>(type: "int", nullable: true),
                    AwayScore = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OverallRating = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    ExecutiveSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DevelopmentPriorities = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuthorRole = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchOfficialReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchOfficialReports_Fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchOfficialReports_Referees_RefereeId",
                        column: x => x.RefereeId,
                        principalTable: "Referees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfficialAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FixtureId = table.Column<int>(type: "int", nullable: false),
                    RefereeId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficialAssignments_Fixtures_FixtureId",
                        column: x => x.FixtureId,
                        principalTable: "Fixtures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficialAssignments_Referees_RefereeId",
                        column: x => x.RefereeId,
                        principalTable: "Referees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DelegateReports",
                columns: table => new
                {
                    AssessmentId = table.Column<int>(type: "int", nullable: false),
                    DelegateName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ManagementScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    ManagementNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ManagementStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ManagementAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CommunicationScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    CommunicationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CommunicationStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CommunicationAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConsistencyScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    ConsistencyNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ConsistencyStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConsistencyAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OverallNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegateReports", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_DelegateReports_Assessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyMatchIncidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssessmentId = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DecisionType = table.Column<int>(type: "int", nullable: false),
                    OriginalDecisionCorrect = table.Column<bool>(type: "bit", nullable: false),
                    VarInterventionRequired = table.Column<bool>(type: "bit", nullable: false),
                    VarInterventionMade = table.Column<bool>(type: "bit", nullable: false),
                    PanelVotesCorrect = table.Column<int>(type: "int", nullable: false),
                    PanelVotesIncorrect = table.Column<int>(type: "int", nullable: false),
                    Rationale = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyMatchIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyMatchIncidents_Assessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchStatReports",
                columns: table => new
                {
                    AssessmentId = table.Column<int>(type: "int", nullable: false),
                    YellowCards = table.Column<int>(type: "int", nullable: false),
                    RedCards = table.Column<int>(type: "int", nullable: false),
                    PenaltiesAwarded = table.Column<int>(type: "int", nullable: false),
                    FoulsGiven = table.Column<int>(type: "int", nullable: false),
                    OffsidesCalled = table.Column<int>(type: "int", nullable: false),
                    DistanceCoveredKm = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    AvgSprintSpeedKmh = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    HighIntensityRuns = table.Column<int>(type: "int", nullable: false),
                    IncidentNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    VarInteractionNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchStatReports", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_MatchStatReports_Assessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechnicalReports",
                columns: table => new
                {
                    AssessmentId = table.Column<int>(type: "int", nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LawScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    LawNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LawStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LawAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PositioningScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    PositioningNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PositioningStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PositioningAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FitnessScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    FitnessNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FitnessStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FitnessAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ControlScore = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    ControlNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ControlStrengths = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ControlAreasForImprovement = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OverallNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnicalReports", x => x.AssessmentId);
                    table.ForeignKey(
                        name: "FK_TechnicalReports_Assessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "Assessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchIncidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchIncidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchIncidents_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Minute = table.Column<int>(type: "int", nullable: false),
                    AddedTime = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Impact = table.Column<int>(type: "int", nullable: false),
                    OfficialInvolved = table.Column<bool>(type: "bit", nullable: false),
                    OfficialResponse = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Teams = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Accuracy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportEvents_MatchOfficialReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "MatchOfficialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(4,1)", nullable: false),
                    Narrative = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Strengths = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AreasForImprovement = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportSections_MatchOfficialReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "MatchOfficialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoClips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Timestamp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoClips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoClips_MatchOfficialReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "MatchOfficialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterventionReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReviewerRole = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    AccuracyScore = table.Column<int>(type: "int", nullable: false),
                    ConsistencyScore = table.Column<int>(type: "int", nullable: false),
                    CommunicationScore = table.Column<int>(type: "int", nullable: false),
                    MatchHandlingScore = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterventionReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterventionReviews_MatchOfficialReports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "MatchOfficialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterventionReviews_ReportEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "ReportEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_Timestamp",
                table: "ActivityLog",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_Fixture_Referee_Role",
                table: "Assessments",
                columns: new[] { "FixtureId", "RefereeId", "OfficialRole" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_FixtureId",
                table: "Assessments",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_RefereeId",
                table: "Assessments",
                column: "RefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_Status",
                table: "Assessments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_ShortName",
                table: "Divisions",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_SortOrder",
                table: "Divisions",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_Date",
                table: "Fixtures",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_DivisionId_Date",
                table: "Fixtures",
                columns: new[] { "DivisionId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_Match_Unique",
                table: "Fixtures",
                columns: new[] { "HomeTeam", "AwayTeam", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fixtures_Matchday",
                table: "Fixtures",
                column: "Matchday");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReviews_EventId",
                table: "InterventionReviews",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReviews_ReportId",
                table: "InterventionReviews",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReviews_ReportId_EventId",
                table: "InterventionReviews",
                columns: new[] { "ReportId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_InterventionReviews_ReportId_ReviewerName",
                table: "InterventionReviews",
                columns: new[] { "ReportId", "ReviewerName" });

            migrationBuilder.CreateIndex(
                name: "IX_KeyMatchIncidents_AssessmentId_Minute",
                table: "KeyMatchIncidents",
                columns: new[] { "AssessmentId", "Minute" });

            migrationBuilder.CreateIndex(
                name: "IX_KeyMatchIncidents_AssessmentId_SortOrder",
                table: "KeyMatchIncidents",
                columns: new[] { "AssessmentId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Date",
                table: "Matches",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_FixtureId",
                table: "Matches",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RefereeId",
                table: "Matches",
                column: "RefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RefereeId_Date",
                table: "Matches",
                columns: new[] { "RefereeId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchIncidents_MatchId",
                table: "MatchIncidents",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchOfficialReports_FixtureId",
                table: "MatchOfficialReports",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchOfficialReports_MatchDate",
                table: "MatchOfficialReports",
                column: "MatchDate");

            migrationBuilder.CreateIndex(
                name: "IX_MatchOfficialReports_RefereeId",
                table: "MatchOfficialReports",
                column: "RefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchOfficialReports_Status",
                table: "MatchOfficialReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_Fixture_Referee_Role",
                table: "MatchOfficialReports",
                columns: new[] { "FixtureId", "RefereeId", "OfficialRole" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialAssignments_Fixture_Referee",
                table: "OfficialAssignments",
                columns: new[] { "FixtureId", "RefereeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialAssignments_Fixture_Role",
                table: "OfficialAssignments",
                columns: new[] { "FixtureId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialAssignments_RefereeId_FixtureId",
                table: "OfficialAssignments",
                columns: new[] { "RefereeId", "FixtureId" });

            migrationBuilder.CreateIndex(
                name: "IX_RefereeCareerEntries_RefereeId_SortOrder",
                table: "RefereeCareerEntries",
                columns: new[] { "RefereeId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Referees_LastName_FirstName",
                table: "Referees",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Referees_RefCode",
                table: "Referees",
                column: "RefCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referees_Region",
                table: "Referees",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_Referees_Status",
                table: "Referees",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Referees_Tier",
                table: "Referees",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_RefereeSeasonStats_RefereeId_Season",
                table: "RefereeSeasonStats",
                columns: new[] { "RefereeId", "Season" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportEvents_ReportId_Minute",
                table: "ReportEvents",
                columns: new[] { "ReportId", "Minute" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportSections_ReportId_SortOrder",
                table: "ReportSections",
                columns: new[] { "ReportId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoClips_ReportId_SortOrder",
                table: "VideoClips",
                columns: new[] { "ReportId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLog");

            migrationBuilder.DropTable(
                name: "DelegateReports");

            migrationBuilder.DropTable(
                name: "InterventionReviews");

            migrationBuilder.DropTable(
                name: "KeyMatchIncidents");

            migrationBuilder.DropTable(
                name: "MatchIncidents");

            migrationBuilder.DropTable(
                name: "MatchStatReports");

            migrationBuilder.DropTable(
                name: "OfficialAssignments");

            migrationBuilder.DropTable(
                name: "RefereeCareerEntries");

            migrationBuilder.DropTable(
                name: "RefereeSeasonStats");

            migrationBuilder.DropTable(
                name: "ReportSections");

            migrationBuilder.DropTable(
                name: "TechnicalReports");

            migrationBuilder.DropTable(
                name: "VideoClips");

            migrationBuilder.DropTable(
                name: "ReportEvents");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "MatchOfficialReports");

            migrationBuilder.DropTable(
                name: "Fixtures");

            migrationBuilder.DropTable(
                name: "Referees");

            migrationBuilder.DropTable(
                name: "Divisions");
        }
    }
}
