using System.Collections.Immutable;
using Lumiaro.MatchValidation.Components.Shared;
using Lumiaro.MatchValidation.Models;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Components.EditPanels;

public partial class GoalEditPanel : ComponentBase
{
    [Parameter, EditorRequired] public GoalEvent Event { get; set; } = default!;
    [Parameter, EditorRequired] public Team HomeTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Team AwayTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Squad HomeSquad { get; set; } = default!;
    [Parameter, EditorRequired] public Squad AwaySquad { get; set; } = default!;
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback<GoalEvent> OnConfirm { get; set; }

    private TeamKey? _team;
    private int?     _minute;
    private Player?  _player;
    private bool     _isOwnGoal;
    private IReadOnlyList<PlayerSelect.PlayerGroup> _playerGroups = Array.Empty<PlayerSelect.PlayerGroup>();
    private FieldErrors _errors = FieldErrors.None;

    protected override void OnParametersSet()
    {
        // Seed form state from the incoming event exactly once per edit session.
        _team       = Event.ScoringTeam;
        _minute     = Event.Minute;
        _player     = Event.Scorer;
        _isOwnGoal  = Event.IsOwnGoal;
        _playerGroups = BuildGroupsFor(_team);
    }

    private Task OnTeamChanged(TeamKey? team)
    {
        _team = team;
        _player = null; // new team means the old player is meaningless
        _playerGroups = BuildGroupsFor(team);
        return Task.CompletedTask;
    }

    private IReadOnlyList<PlayerSelect.PlayerGroup> BuildGroupsFor(TeamKey? team)
    {
        if (team is null) return Array.Empty<PlayerSelect.PlayerGroup>();
        var squad = team == TeamKey.Home ? HomeSquad : AwaySquad;
        return new[]
        {
            new PlayerSelect.PlayerGroup("Starting XI",  squad.StartingXi),
            new PlayerSelect.PlayerGroup("Substitutes",  squad.Substitutes)
        };
    }

    private async Task HandleConfirm()
    {
        _errors = ValidateInputs();
        if (_errors.HasAny) return;

        var updated = Event with
        {
            ScoringTeam = _team!.Value,
            Scorer      = _player!,
            Minute      = _minute!.Value,
            IsOwnGoal   = _isOwnGoal,
            Source      = EventSource.Correction
        };

        await OnConfirm.InvokeAsync(updated);
    }

    private FieldErrors ValidateInputs() => new(
        Team:   _team is null,
        Minute: _minute is null,
        Player: _player is null);

    private readonly record struct FieldErrors(bool Team, bool Minute, bool Player)
    {
        public static readonly FieldErrors None = new(false, false, false);
        public bool HasAny => Team || Minute || Player;
    }
}
