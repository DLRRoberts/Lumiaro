using Lumiaro.MatchValidation.Components.Shared;
using Lumiaro.MatchValidation.Models;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Components.EditPanels;

public partial class SubstitutionEditPanel : ComponentBase
{
    [Parameter, EditorRequired] public SubstitutionEvent Event { get; set; } = default!;
    [Parameter, EditorRequired] public Team HomeTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Team AwayTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public MatchRosterState Rosters { get; set; } = default!;
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback<SubstitutionEvent> OnConfirm { get; set; }

    private TeamKey? _team;
    private int?     _minute;
    private Player?  _playerOff;
    private Player?  _playerOn;

    private IReadOnlyList<PlayerSelect.PlayerGroup> _offGroups = Array.Empty<PlayerSelect.PlayerGroup>();
    private IReadOnlyList<PlayerSelect.PlayerGroup> _onGroups  = Array.Empty<PlayerSelect.PlayerGroup>();
    private FieldErrors _errors = FieldErrors.None;

    protected override void OnParametersSet()
    {
        _team      = Event.Team;
        _minute    = Event.Minute;
        _playerOff = Event.PlayerOff;
        _playerOn  = Event.PlayerOn;
        RebuildGroups(_team);
    }

    private Task OnTeamChanged(TeamKey? team)
    {
        _team      = team;
        _playerOff = null;
        _playerOn  = null;
        RebuildGroups(team);
        return Task.CompletedTask;
    }

    private void RebuildGroups(TeamKey? team)
    {
        if (team is null)
        {
            _offGroups = Array.Empty<PlayerSelect.PlayerGroup>();
            _onGroups  = Array.Empty<PlayerSelect.PlayerGroup>();
            return;
        }

        var roster = Rosters.For(team.Value);
        _offGroups = new[] { new PlayerSelect.PlayerGroup("On pitch",  roster.OnPitch) };
        _onGroups  = new[] { new PlayerSelect.PlayerGroup("Available", roster.Available) };
    }

    private async Task HandleConfirm()
    {
        _errors = ValidateInputs();
        if (_errors.HasAny) return;

        var updated = Event with
        {
            Team      = _team!.Value,
            Minute    = _minute!.Value,
            PlayerOff = _playerOff!,
            PlayerOn  = _playerOn!,
            Source    = EventSource.Correction
        };

        await OnConfirm.InvokeAsync(updated);
    }

    private FieldErrors ValidateInputs() => new(
        Team:      _team is null,
        Minute:    _minute is null,
        PlayerOff: _playerOff is null,
        PlayerOn:  _playerOn is null);

    private readonly record struct FieldErrors(bool Team, bool Minute, bool PlayerOff, bool PlayerOn)
    {
        public static readonly FieldErrors None = new(false, false, false, false);
        public bool HasAny => Team || Minute || PlayerOff || PlayerOn;
    }
}
