using AutoMapper;
using LumiaroAdmin.Data.Entities;
using LumiaroAdmin.Data.Models;
using LumiaroAdmin.Data.Repositories;
using LumiaroAdmin.Models;

namespace LumiaroAdmin.Services;

public class AssessmentService
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly IMapper _mapper;

    public AssessmentService(IAssessmentRepository assessmentRepository, IMapper mapper)
    {
        _assessmentRepository = assessmentRepository;
        _mapper = mapper;
    }

    // ─── Queries ───

    public async Task<List<Assessment>> GetAllAsync()
    {
        var results = await _assessmentRepository.GetAllAsync();
        var mapped = _mapper.Map<List<Assessment>>(results);

        return mapped.OrderByDescending(a => a.MatchDate).ThenBy(a => a.MatchTitle).ToList();;
    }
    

    public async Task<Assessment?> GetByIdAsync(int id)
    {
        var assessment = await _assessmentRepository.GetByIdAsync(id);
        
        var mapped = _mapper.Map<Assessment>(assessment);

        return mapped;
    }

    public async Task<List<Assessment>> GetByFixtureAsync(int fixtureId)
    {
        var assessments = await _assessmentRepository.GetByFixtureAsync(fixtureId);
        
        var mapped = _mapper.Map<List<Assessment>>(assessments);
        return mapped.OrderBy(a => a.OfficialRole).ToList();
    }


    public async Task<List<Assessment>> GetByRefereeAsync(int refereeId)
    {
        var assessments = await _assessmentRepository.GetByRefereeAsync(refereeId);
        var mapped = _mapper.Map<List<Assessment>>(assessments);
        
        return mapped.OrderByDescending(a => a.MatchDate).ToList();
    }

    public async Task<List<Assessment>> SearchAsync(string? query, int? refereeId, int? fixtureId,
        DateTime? dateFrom, DateTime? dateTo, AssessmentStatus? status)
    {
        var entityStatus = MapAssessmentStatus(status);
        var entityResults = await _assessmentRepository.SearchAsync(query,  refereeId, fixtureId, dateFrom, dateTo, entityStatus);
        var results = _mapper.Map<List<Assessment>>(entityResults);

        return results.OrderByDescending(a => a.MatchDate).ThenBy(a => a.MatchTitle).ToList();
    }
    
    // ─── CRUD ───

    public async Task<Assessment> CreateAsync(Assessment assessment, Fixture fixture, Referee referee)
    {
        var insertedEntity = await _assessmentRepository.CreateAssessment(assessment, fixture, referee);
        
        var inserted = _mapper.Map<Assessment>(insertedEntity);
        return inserted;
    }

    public async Task UpdateAsync(Assessment updated)
    {
        var existing = await GetByIdAsync(updated.Id);
        if (existing == null) return;

        existing.Status = updated.Status;
        existing.OverallScore = updated.OverallScore;
        existing.SummaryNotes = updated.SummaryNotes;
        existing.Recommendations = updated.Recommendations;
        existing.Technical = updated.Technical;
        existing.Delegate = updated.Delegate;
        existing.MatchReport = updated.MatchReport;
        existing.Kmi = updated.Kmi;
        existing.UpdatedAt = DateTime.Now;
        
        var asEntity = _mapper.Map<AssessmentEntity>(existing);
        
        _assessmentRepository.Update(asEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var a = await GetByIdAsync(id);
        if (a != null)
        {
            var asEntity = _mapper.Map<AssessmentEntity>(a);
            _assessmentRepository.Remove(asEntity);
        }
    }

    // ─── Stats ───

    public async Task<(int total, int draft, int submitted, int reviewed, int finalised)> GetStatusCountsAsync()
    {
        var statusCountsResponse = await _assessmentRepository.GetStatusCountsAsync();
        return (
            statusCountsResponse.Total,
            statusCountsResponse.Draft,
            statusCountsResponse.Submitted,
            statusCountsResponse.Reviewed,
            statusCountsResponse.Finalised
        );
    }

    /// <summary>Check if an assessment already exists for a given fixture + referee + role.</summary>
    
    public async Task<Assessment?> FindExistingAsync(int fixtureId, int refereeId, OfficialRole role)
    {
        var entity = await _assessmentRepository.FindExistingAsync(fixtureId, refereeId, role.ToData());
        return entity?.ToDomain();
    }

    public async Task<List<(int Id, string Name)>> GetAssessedRefereesAsync()
    {
        return await _assessmentRepository.GetAssessedRefereesAsync();
    }

    public async Task<List<(int Id, string Title, DateTime Date)>> GetAssessedFixturesAsync()
    {
        var results = await _assessmentRepository.GetAssessedFixturesAsync();
        return results.Select(f => (f.FixtureId, $"{f.HomeTeam} vs {f.AwayTeam}", f.Date)).ToList();
    }
    
    private Data.Enums.AssessmentStatus? MapAssessmentStatus(AssessmentStatus? status)
        => status switch
        {
            AssessmentStatus.Draft => Data.Enums.AssessmentStatus.Draft,
            AssessmentStatus.Finalised => Data.Enums.AssessmentStatus.Finalised,
            AssessmentStatus.Reviewed => Data.Enums.AssessmentStatus.Reviewed,
            AssessmentStatus.Submitted => Data.Enums.AssessmentStatus.Submitted,
            _ => null
        };


    // ─── Sample Data ───

    private List<Assessment> GenerateSampleAssessments()
    {
        var list = new List<Assessment>();

        // Assessment 1: Anthony Taylor — Arsenal vs Chelsea (fixture 1)
        list.Add(new Assessment
        {
            Id = 1, FixtureId = 1, RefereeId = 18, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Arsenal vs Chelsea", MatchDate = new DateTime(2026, 2, 15),
            Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague,
            Venue = "Emirates Stadium",
            RefereeName = "Anthony Taylor", RefereeInitials = "AT", RefereeCode = "REF-0018",
            RefereeTier = RefereeTier.FifaInternational,
            Status = AssessmentStatus.Finalised,
            CreatedAt = new DateTime(2026, 2, 16, 10, 30, 0),
            UpdatedAt = new DateTime(2026, 2, 17, 14, 0, 0),
            CreatedBy = "Paul Tierney",
            OverallScore = 7.6,
            SummaryNotes = "Competent performance in a high-intensity London derby. Managed the tempo well in the opening 30 minutes but appeared to lose control briefly around the 55th minute following a disputed penalty claim. Recovered composure and finished the match strongly.",
            Recommendations = "Review clip-based analysis of the 55th minute penalty area incident. Consider additional positioning drills for congested box situations.",
            Kmi = new KmiReport
            {
                PanelNotes = "Two key incidents reviewed. Panel broadly satisfied with outcomes though the 55' penalty claim split opinion.",
                Incidents = new List<KeyMatchIncident>
                {
                    new() { Minute = 23, Description = "Penalty awarded to Arsenal for handball in the box", DecisionType = KmiDecisionType.Penalty,
                            OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false,
                            PanelVotesCorrect = 5, PanelVotesIncorrect = 0, Rationale = "Clear and deliberate handball, arm in an unnatural position." },
                    new() { Minute = 55, Description = "Penalty claim waved away — Chelsea challenge on Saka", DecisionType = KmiDecisionType.Penalty,
                            OriginalDecisionCorrect = false, VarInterventionRequired = true, VarInterventionMade = false,
                            PanelVotesCorrect = 2, PanelVotesIncorrect = 3, Rationale = "Panel majority view: sufficient contact to warrant a penalty. VAR should have recommended on-field review." },
                    new() { Minute = 78, Description = "Red card shown for serious foul play", DecisionType = KmiDecisionType.RedCard,
                            OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false,
                            PanelVotesCorrect = 4, PanelVotesIncorrect = 1, Rationale = "Studs-up challenge with excessive force. Correct application of Law 12." }
                }
            },
            Technical = new TechnicalReport
            {
                ReviewerName = "Mike Dean",
                ApplicationOfLaw = new CompetencyScore { Score = 7.5, Notes = "Generally accurate interpretation. The 55' penalty incident was the main area of concern.", Strengths = "Confident in applying advantage", AreasForImprovement = "Needs sharper assessment of contact in the penalty area" },
                PositioningAndMovement = new CompetencyScore { Score = 7.8, Notes = "Good diagonal movement. Maintained sight lines for most key incidents.", Strengths = "Excellent movement in open play transitions", AreasForImprovement = "Slightly too far from the 55' incident" },
                FitnessAndWorkRate = new CompetencyScore { Score = 8.0, Notes = "Strong physical output. Kept pace with counterattacks throughout.", Strengths = "High-intensity sprint in final 15 minutes", AreasForImprovement = "None noted" },
                MatchControl = new CompetencyScore { Score = 7.2, Notes = "Lost the feel of the match briefly in the 50-60 minute period. Recovered well.", Strengths = "Early yellow card set the tone", AreasForImprovement = "Needs to assert control earlier when the temperature rises" },
                OverallNotes = "A solid 7+ performance. The penalty area incident remains the primary talking point."
            },
            Delegate = new DelegateReport
            {
                DelegateName = "Alan Shearer",
                ManagementOfGame = new CompetencyScore { Score = 7.4, Notes = "Managed the first half well. The frustration from Chelsea following the waved-away penalty was not handled as calmly as one would expect.", Strengths = "Good rapport with both captains", AreasForImprovement = "Could have spoken to Chelsea staff earlier to de-escalate" },
                Communication = new CompetencyScore { Score = 7.8, Notes = "Clear communication with assistants. Used visible hand signals effectively.", Strengths = "Proactive explanation to players after free kicks", AreasForImprovement = "None noted" },
                Consistency = new CompetencyScore { Score = 7.5, Notes = "Broadly consistent bar the penalty area decisions. Similar challenges were treated differently.", Strengths = "Consistent foul threshold for tackles", AreasForImprovement = "Apply the same standard in and out of the penalty area" },
                OverallNotes = "Taylor demonstrated strong communication and presence. The match mood shifted negatively around the 55' mark and it took 5-10 minutes to recover."
            },
            MatchReport = new MatchReport
            {
                YellowCards = 5, RedCards = 1, PenaltiesAwarded = 1,
                FoulsGiven = 22, OffsidesCalled = 4,
                DistanceCoveredKm = 11.2, AvgSprintSpeedKmh = 24.8, HighIntensityRuns = 67,
                IncidentNotes = "Red card at 78' for serious foul play. Penalty at 23' for handball.",
                VarInteractionNotes = "VAR checked the 55' penalty claim and confirmed the on-field decision. No on-field review was recommended."
            }
        });

        // Assessment 2: Michael Oliver — Liverpool vs Man Utd (fixture 2)
        list.Add(new Assessment
        {
            Id = 2, FixtureId = 2, RefereeId = 42, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Liverpool vs Manchester United", MatchDate = new DateTime(2026, 2, 15),
            Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague,
            Venue = "Anfield",
            RefereeName = "Michael Oliver", RefereeInitials = "MO", RefereeCode = "REF-0042",
            RefereeTier = RefereeTier.FifaInternational,
            Status = AssessmentStatus.Reviewed,
            CreatedAt = new DateTime(2026, 2, 16, 11, 0, 0),
            UpdatedAt = new DateTime(2026, 2, 17, 9, 30, 0),
            CreatedBy = "Howard Webb",
            OverallScore = 8.4,
            SummaryNotes = "Outstanding performance in a high-profile fixture. Oliver demonstrated excellent positioning throughout, particularly in congested areas. Decisive and accurate in all key moments.",
            Recommendations = "Consider for upcoming international appointments. Exemplary handling of a volatile atmosphere.",
            Kmi = new KmiReport
            {
                PanelNotes = "No contentious decisions. All key moments handled correctly.",
                Incidents = new List<KeyMatchIncident>
                {
                    new() { Minute = 34, Description = "Goal allowed — tight offside call", DecisionType = KmiDecisionType.GoalAllowed,
                            OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = true,
                            PanelVotesCorrect = 5, PanelVotesIncorrect = 0, Rationale = "VAR confirmed the attacker was onside by a narrow margin. Correct decision." },
                    new() { Minute = 61, Description = "Penalty awarded for trip in the box", DecisionType = KmiDecisionType.Penalty,
                            OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false,
                            PanelVotesCorrect = 5, PanelVotesIncorrect = 0, Rationale = "Clear and obvious foul. Excellent positioning to see the contact." }
                }
            },
            Technical = new TechnicalReport
            {
                ReviewerName = "Mark Clattenburg",
                ApplicationOfLaw = new CompetencyScore { Score = 8.5, Notes = "Impeccable application. Every major decision was correct.", Strengths = "Precise interpretation of handball law", AreasForImprovement = "None" },
                PositioningAndMovement = new CompetencyScore { Score = 8.6, Notes = "Textbook positioning throughout. Was perfectly placed for both key incidents.", Strengths = "Diagonal movement in transitions was outstanding", AreasForImprovement = "None" },
                FitnessAndWorkRate = new CompetencyScore { Score = 8.2, Notes = "Covered excellent ground. No sign of fatigue.", Strengths = "Consistent sprint output across both halves", AreasForImprovement = "None" },
                MatchControl = new CompetencyScore { Score = 8.3, Notes = "Complete control from start to finish. Early interventions set the standard.", Strengths = "Calm and authoritative presence", AreasForImprovement = "None" },
                OverallNotes = "One of the strongest performances this season. A benchmark for colleagues."
            },
            Delegate = new DelegateReport
            {
                DelegateName = "Gary Neville",
                ManagementOfGame = new CompetencyScore { Score = 8.5, Notes = "Managed an intense atmosphere superbly. Players clearly respected his authority.", Strengths = "De-escalated a confrontation between both sets of players at 40'", AreasForImprovement = "None" },
                Communication = new CompetencyScore { Score = 8.4, Notes = "Excellent verbal and non-verbal communication throughout.", Strengths = "Clear explanations to players, calm under pressure", AreasForImprovement = "None" },
                Consistency = new CompetencyScore { Score = 8.3, Notes = "Remarkably consistent. The same foul was penalised identically regardless of location or team.", Strengths = "Identical threshold applied to both teams", AreasForImprovement = "None" },
                OverallNotes = "Oliver read the game brilliantly. His presence and authority were felt from minute one."
            },
            MatchReport = new MatchReport
            {
                YellowCards = 3, RedCards = 0, PenaltiesAwarded = 1,
                FoulsGiven = 18, OffsidesCalled = 3,
                DistanceCoveredKm = 11.8, AvgSprintSpeedKmh = 25.3, HighIntensityRuns = 72,
                IncidentNotes = "One penalty at 61'. No red cards. Well-managed match.",
                VarInteractionNotes = "VAR confirmed goal at 34' for offside check. Clean interaction, minimal delay."
            }
        });

        // Assessment 3: John Brooks — Arsenal vs Chelsea as 4th Official (fixture 1)
        list.Add(new Assessment
        {
            Id = 3, FixtureId = 1, RefereeId = 91, OfficialRole = OfficialRole.MatchOfficial,
            MatchTitle = "Arsenal vs Chelsea", MatchDate = new DateTime(2026, 2, 15),
            Competition = "Premier League", CompetitionTier = RefereeTier.PremierLeague,
            Venue = "Emirates Stadium",
            RefereeName = "John Brooks", RefereeInitials = "JB", RefereeCode = "REF-0091",
            RefereeTier = RefereeTier.PremierLeague,
            Status = AssessmentStatus.Submitted,
            CreatedAt = new DateTime(2026, 2, 16, 12, 0, 0),
            CreatedBy = "Howard Webb",
            OverallScore = 7.8,
            SummaryNotes = "Solid 4th official performance. Managed the technical areas effectively throughout a heated derby. Communicated substitution procedures clearly.",
            Recommendations = "Continue development towards referee role in high-profile matches.",
            Kmi = new KmiReport { PanelNotes = "N/A — 4th official role.", Incidents = new() },
            Technical = new TechnicalReport
            {
                ReviewerName = "Mike Dean",
                ApplicationOfLaw = new CompetencyScore { Score = 7.8, Notes = "Good awareness of technical area regulations." },
                PositioningAndMovement = new CompetencyScore { Score = 7.5, Notes = "Maintained appropriate position." },
                FitnessAndWorkRate = new CompetencyScore { Score = 8.0, Notes = "Active and alert throughout." },
                MatchControl = new CompetencyScore { Score = 8.0, Notes = "Managed both technical areas calmly despite heated exchanges." },
                OverallNotes = "Strong showing in a challenging atmosphere."
            },
            Delegate = new DelegateReport
            {
                DelegateName = "Alan Shearer",
                ManagementOfGame = new CompetencyScore { Score = 7.8, Notes = "Kept both benches in check." },
                Communication = new CompetencyScore { Score = 8.0, Notes = "Clear communication with assessor and both management teams." },
                Consistency = new CompetencyScore { Score = 7.6, Notes = "Consistent approach to both benches." },
                OverallNotes = "Professional and composed."
            },
            MatchReport = new MatchReport
            {
                YellowCards = 0, RedCards = 0, PenaltiesAwarded = 0,
                FoulsGiven = 0, OffsidesCalled = 0,
                DistanceCoveredKm = 3.2, AvgSprintSpeedKmh = 0, HighIntensityRuns = 0,
                IncidentNotes = "Managed 6 substitutions across both teams. One warning issued to Chelsea assistant manager.",
                VarInteractionNotes = "N/A"
            }
        });

        // Assessment 4: Simon Roberts — Cardiff vs Swansea (fixture 6) — Draft
        list.Add(new Assessment
        {
            Id = 4, FixtureId = 6, RefereeId = 127, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Cardiff City vs Swansea City", MatchDate = new DateTime(2026, 2, 15),
            Competition = "Championship", CompetitionTier = RefereeTier.Championship,
            Venue = "Cardiff City Stadium",
            RefereeName = "Simon Roberts", RefereeInitials = "SR", RefereeCode = "REF-0127",
            RefereeTier = RefereeTier.Championship,
            Status = AssessmentStatus.Draft,
            CreatedAt = new DateTime(2026, 2, 16, 14, 30, 0),
            CreatedBy = "Performance Team",
            OverallScore = 6.8,
            SummaryNotes = "Below-par performance in a heated South Wales derby. Several inconsistencies in foul decisions and a missed offside that led to a goal. Currently on probation — this performance will require additional review.",
            Recommendations = "Mandatory video review session. Additional mentoring sessions recommended. Probation status to be reviewed.",
            Kmi = new KmiReport
            {
                PanelNotes = "Significant concern over the missed offside at 67'. Two of three key incidents were incorrectly handled.",
                Incidents = new List<KeyMatchIncident>
                {
                    new() { Minute = 32, Description = "Penalty awarded to Cardiff for foul in the box", DecisionType = KmiDecisionType.Penalty,
                            OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false,
                            PanelVotesCorrect = 4, PanelVotesIncorrect = 1, Rationale = "Correct — clear trip on the attacker." },
                    new() { Minute = 67, Description = "Goal allowed — attacker clearly offside", DecisionType = KmiDecisionType.GoalAllowed,
                            OriginalDecisionCorrect = false, VarInterventionRequired = true, VarInterventionMade = false,
                            PanelVotesCorrect = 0, PanelVotesIncorrect = 5, Rationale = "Unanimous — the attacker was visibly offside. Both referee and AR missed the call. No VAR at Championship level." },
                    new() { Minute = 85, Description = "Second yellow card — simulation", DecisionType = KmiDecisionType.Foul,
                            OriginalDecisionCorrect = false, VarInterventionRequired = false, VarInterventionMade = false,
                            PanelVotesCorrect = 1, PanelVotesIncorrect = 4, Rationale = "Panel majority view: there was genuine contact and the player did not simulate." }
                }
            },
            Technical = new TechnicalReport
            {
                ReviewerName = "Mark Halsey",
                ApplicationOfLaw = new CompetencyScore { Score = 6.5, Notes = "Several misapplications. The simulation call at 85' was particularly concerning.", Strengths = "Good penalty decision at 32'", AreasForImprovement = "Needs to better distinguish between simulation and genuine contact" },
                PositioningAndMovement = new CompetencyScore { Score = 6.8, Notes = "Positioning for the 67' offside was poor — too far from the line of play.", Strengths = "Reasonable in open play", AreasForImprovement = "Must improve positioning for offside decisions" },
                FitnessAndWorkRate = new CompetencyScore { Score = 7.0, Notes = "Adequate but showed signs of fatigue in the second half.", Strengths = "First half movement was good", AreasForImprovement = "Conditioning for 90-minute intensity at this level" },
                MatchControl = new CompetencyScore { Score = 6.8, Notes = "Lost control of the match after the disputed goal. Players became increasingly frustrated.", Strengths = "Started the match with authority", AreasForImprovement = "Must regain composure after controversial decisions" },
                OverallNotes = "A concerning performance that raises questions about readiness for Championship-level fixtures during probation."
            },
            Delegate = new DelegateReport
            {
                DelegateName = "John Hartson",
                ManagementOfGame = new CompetencyScore { Score = 6.5, Notes = "Struggled to manage the atmosphere after the 67' goal. Players surrounded the referee.", Strengths = "Started well", AreasForImprovement = "Needs stronger presence when under pressure" },
                Communication = new CompetencyScore { Score = 7.0, Notes = "Communication was acceptable but lacked clarity in explaining key decisions.", Strengths = "Adequate interaction with captains", AreasForImprovement = "Should explain disputed decisions more clearly and promptly" },
                Consistency = new CompetencyScore { Score = 6.4, Notes = "Inconsistent application of the foul threshold. Similar challenges treated differently.", Strengths = "None noted in this area", AreasForImprovement = "Critical area for development — consistency must improve" },
                OverallNotes = "Roberts appeared nervous and reactive rather than proactive. Additional support and mentoring is warranted."
            },
            MatchReport = new MatchReport
            {
                YellowCards = 7, RedCards = 0, PenaltiesAwarded = 1,
                FoulsGiven = 28, OffsidesCalled = 2,
                DistanceCoveredKm = 10.4, AvgSprintSpeedKmh = 23.1, HighIntensityRuns = 52,
                IncidentNotes = "High card count reflects loss of control. The 67' offside miss was the pivotal moment.",
                VarInteractionNotes = "N/A — no VAR at Championship level."
            }
        });

        // Assessment 5: David Phillips — Wrexham vs Bolton (fixture 9) — Submitted
        list.Add(new Assessment
        {
            Id = 5, FixtureId = 9, RefereeId = 134, OfficialRole = OfficialRole.Referee,
            MatchTitle = "Wrexham vs Bolton", MatchDate = new DateTime(2026, 2, 15),
            Competition = "League One", CompetitionTier = RefereeTier.LeagueOne,
            Venue = "Racecourse Ground",
            RefereeName = "David Phillips", RefereeInitials = "DP", RefereeCode = "REF-0134",
            RefereeTier = RefereeTier.LeagueOne,
            Status = AssessmentStatus.Submitted,
            CreatedAt = new DateTime(2026, 2, 16, 15, 0, 0),
            CreatedBy = "Performance Team",
            OverallScore = 7.4,
            SummaryNotes = "Solid debut at this venue. Phillips handled the atmosphere well for a relatively inexperienced official. Good fitness and work rate.",
            Recommendations = "Continue current development pathway. Positive trajectory.",
            Kmi = new KmiReport
            {
                PanelNotes = "One key incident. Handled correctly.",
                Incidents = new List<KeyMatchIncident>
                {
                    new() { Minute = 71, Description = "Penalty awarded for handball", DecisionType = KmiDecisionType.Handball,
                            OriginalDecisionCorrect = true, VarInterventionRequired = false, VarInterventionMade = false,
                            PanelVotesCorrect = 4, PanelVotesIncorrect = 1, Rationale = "Arm away from the body in an unnatural position. Correct decision." }
                }
            },
            Technical = new TechnicalReport
            {
                ReviewerName = "Chris Foy",
                ApplicationOfLaw = new CompetencyScore { Score = 7.5, Notes = "Good overall application." },
                PositioningAndMovement = new CompetencyScore { Score = 7.2, Notes = "Room for improvement in set-piece positioning." },
                FitnessAndWorkRate = new CompetencyScore { Score = 7.8, Notes = "Impressive physical output." },
                MatchControl = new CompetencyScore { Score = 7.0, Notes = "Managed the game adequately." },
                OverallNotes = "Encouraging performance from a developing official."
            },
            Delegate = new DelegateReport
            {
                DelegateName = "Dean Saunders",
                ManagementOfGame = new CompetencyScore { Score = 7.3, Notes = "Handled the Racecourse atmosphere well." },
                Communication = new CompetencyScore { Score = 7.5, Notes = "Good communication with both captains." },
                Consistency = new CompetencyScore { Score = 7.2, Notes = "Broadly consistent." },
                OverallNotes = "Positive performance. Phillips is developing well."
            },
            MatchReport = new MatchReport
            {
                YellowCards = 4, RedCards = 0, PenaltiesAwarded = 1,
                FoulsGiven = 20, OffsidesCalled = 3,
                DistanceCoveredKm = 11.0, AvgSprintSpeedKmh = 23.8, HighIntensityRuns = 58,
                IncidentNotes = "One penalty awarded. Clean match overall.",
                VarInteractionNotes = "N/A — no VAR at League One level."
            }
        });

        return list;
    }
}
