using System.Collections.Immutable;
using Lumiaro.MatchValidation.Components.Shared;
using Lumiaro.MatchValidation.Models;
using Lumiaro.MatchValidation.Services;
using Microsoft.AspNetCore.Components;

namespace Lumiaro.MatchValidation.Components.EditPanels;

public partial class CardEditPanel : ComponentBase
{
    [Inject] public IChargeCodeProvider Charges { get; set; } = default!;

    [Parameter, EditorRequired] public CardEvent Event { get; set; } = default!;
    [Parameter, EditorRequired] public Team HomeTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Team AwayTeam { get; set; } = Team.Empty;
    [Parameter, EditorRequired] public Squad HomeSquad { get; set; } = default!;
    [Parameter, EditorRequired] public Squad AwaySquad { get; set; } = default!;
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback<CardEvent> OnConfirm { get; set; }

    private TeamKey?    _team;
    private int?        _minute;
    private Player?     _player;
    private CardType?   _cardType;
    private ChargeCode? _charge;
    private Offence?    _offence;

    private IReadOnlyList<PlayerSelect.PlayerGroup> _playerGroups = Array.Empty<PlayerSelect.PlayerGroup>();
    private ImmutableArray<ChargeCode> _availableCharges = ImmutableArray<ChargeCode>.Empty;
    private ImmutableArray<Offence>    _availableOffences = ImmutableArray<Offence>.Empty;
    private FieldErrors _errors = FieldErrors.None;

    protected override void OnParametersSet()
    {
        _team        = Event.Team;
        _minute      = Event.Minute;
        _player      = Event.Player;
        _cardType    = Event.CardType;
        _charge      = Event.Charge;
        _offence     = Event.Offence;
        _playerGroups      = BuildGroupsFor(_team);
        _availableCharges  = _cardType is { } ct ? Charges.GetChargesFor(ct) : ImmutableArray<ChargeCode>.Empty;
        _availableOffences = _charge?.Offences ?? ImmutableArray<Offence>.Empty;
    }

    private Task OnTeamChanged(TeamKey? team)
    {
        _team = team;
        _player = null;
        _playerGroups = BuildGroupsFor(team);
        return Task.CompletedTask;
    }

    private Task OnCardTypeChanged(CardType? cardType)
    {
        _cardType = cardType;
        _charge   = null;
        _offence  = null;
        _availableCharges  = cardType is { } ct ? Charges.GetChargesFor(ct) : ImmutableArray<ChargeCode>.Empty;
        _availableOffences = ImmutableArray<Offence>.Empty;
        return Task.CompletedTask;
    }

    private Task OnChargeChanged(ChargeCode? charge)
    {
        _charge = charge;
        _offence = null;
        _availableOffences = charge?.Offences ?? ImmutableArray<Offence>.Empty;
        return Task.CompletedTask;
    }

    private IReadOnlyList<PlayerSelect.PlayerGroup> BuildGroupsFor(TeamKey? team)
    {
        if (team is null) return Array.Empty<PlayerSelect.PlayerGroup>();
        var squad = team == TeamKey.Home ? HomeSquad : AwaySquad;
        return new[]
        {
            new PlayerSelect.PlayerGroup("Starting XI", squad.StartingXi),
            new PlayerSelect.PlayerGroup("Substitutes", squad.Substitutes)
        };
    }

    private async Task HandleConfirm()
    {
        _errors = ValidateInputs();
        if (_errors.HasAny) return;

        var updated = Event with
        {
            Team      = _team!.Value,
            Player    = _player!,
            Minute    = _minute!.Value,
            CardType  = _cardType!.Value,
            Charge    = _charge!,
            Offence   = _offence,
            Source    = EventSource.Correction
        };

        await OnConfirm.InvokeAsync(updated);
    }

    private FieldErrors ValidateInputs() => new(
        Team:     _team is null,
        Minute:   _minute is null,
        Player:   _player is null,
        CardType: _cardType is null,
        Charge:   _charge is null,
        Offence:  _charge?.RequiresOffence == true && _offence is null);

    private readonly record struct FieldErrors(bool Team, bool Minute, bool Player, bool CardType, bool Charge, bool Offence)
    {
        public static readonly FieldErrors None = new(false, false, false, false, false, false);
        public bool HasAny => Team || Minute || Player || CardType || Charge || Offence;
    }
}
