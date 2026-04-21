# Lumiaro Match Validation — Blazor Component Library

This folder contains a Blazor Server component library decomposed from the
UEFA Match Validation Tool HTML prototype. It is designed to drop into
the existing Lumiaro referee admin application with minimal friction, while
following the DLR house style used in Sefe and FIFA Edge.

---

## 1. Folder layout

```
LumiaroMatchValidation/
├── Pages/
│   ├── MatchValidation.razor          ← page orchestrator (route: /match-validation/{MatchId})
│   └── MatchValidation.razor.cs
├── Components/
│   ├── MatchValidationHeader.razor
│   ├── MatchCard.razor
│   ├── PendingEventsSection.razor
│   ├── ValidatedEventsSection.razor
│   ├── EventRow.razor                  ← shared by both feeds, with RowMode enum
│   ├── EventRow.razor.cs
│   ├── MatchDurationPanel.razor
│   ├── PostMatchIncidentReport.razor
│   ├── VdcDemoToolbar.razor            ← dev-only, hide behind a flag in prod
│   ├── EditPanels/
│   │   ├── EventEditPanel.razor        ← polymorphic dispatcher
│   │   ├── GoalEditPanel.razor + .cs
│   │   ├── CardEditPanel.razor + .cs
│   │   ├── SubstitutionEditPanel.razor + .cs
│   │   └── TimingEditPanel.razor + .cs
│   └── Shared/
│       ├── TeamSelect.razor
│       ├── PlayerSelect.razor
│       ├── MinuteInput.razor
│       ├── CardTypeSelect.razor
│       ├── ChargeCodeSelect.razor
│       ├── OffenceSelect.razor
│       ├── OwnGoalToggle.razor
│       └── ChargeSummaryStrip.razor
├── Models/
│   ├── Team.cs, Player.cs, Squad.cs
│   ├── CardType.cs, ChargeCode.cs, EventSource.cs
│   ├── MatchEvent.cs                   ← abstract record + Goal/Card/Sub/Timing
│   ├── MatchPhase.cs, MatchRosterState.cs
│   ├── IncidentReportItem.cs
│   ├── ValidationSession.cs (+ MatchInfo)
│   └── Result.cs                       ← Result<T>.Ok / Result<T>.Err
├── Services/
│   ├── ISquadProvider.cs + StaticSquadProvider.cs
│   ├── IChargeCodeProvider.cs + StaticChargeCodeProvider.cs
│   ├── IMinuteFormatter.cs + MinuteFormatter.cs
│   ├── IValidationEventStream.cs       ← SignalR-swappable abstraction
│   ├── IMatchValidationSessionService.cs + MatchValidationSessionService.cs
│   └── DemoVdcPushFactory.cs           ← dev-only, matches VdcDemoToolbar
└── wwwroot/css/match-validation.css    ← full stylesheet with CSS variables
```

---

## 2. DI registration

Add to `Program.cs` (Blazor Server host):

```csharp
builder.Services.AddScoped<IMatchValidationSessionService, MatchValidationSessionService>();
builder.Services.AddSingleton<IChargeCodeProvider, StaticChargeCodeProvider>();
builder.Services.AddSingleton<IMinuteFormatter, MinuteFormatter>();
builder.Services.AddScoped<ISquadProvider, StaticSquadProvider>();
builder.Services.AddScoped<IDemoVdcPushFactory, DemoVdcPushFactory>();

// Swap StaticSquadProvider for an HTTP-backed impl once the squad API is ready:
// builder.Services.AddHttpClient<ISquadProvider, HttpSquadProvider>();
```

**Scope reasoning:**

- `IMatchValidationSessionService` is `Scoped` so each Blazor Server circuit
  (one per referee tab) has its own session. Same pattern as Sefe's
  `ThemeService`.
- `IChargeCodeProvider` and `IMinuteFormatter` are stateless → `Singleton`.
- `ISquadProvider` is `Scoped` to allow per-user request caching in the HTTP
  implementation.
- `IDemoVdcPushFactory` is `Scoped` because it holds rotation cursors that
  should track with the session.

---

## 3. CSS

Reference the stylesheet in `App.razor` (or `_Host.cshtml` depending on your
render mode):

```html
<link rel="stylesheet" href="css/match-validation.css" />
<link href="https://fonts.googleapis.com/css2?family=Barlow:wght@400;500;600&family=Barlow+Condensed:wght@500;600;700&display=swap" rel="stylesheet" />
```

All colour tokens are declared on `:root`. To reskin for Lumiaro's main
theme (rather than the UEFA gold/navy), override the variables on a higher
selector:

```css
.lumiaro-theme {
    --navy-deepest: #...;
    --gold:         #...;
    /* etc. */
}
```

and wrap the page in `<div class="lumiaro-theme">`.

---

## 4. Integration points you will need to wire up

### 4.1 The VDC feed

Right now pending events come from two places:

1. `MatchValidation.razor.cs → SeedPreloadedEvents()` — pushes the first-half
   kickoff + one demo goal on page load.
2. `VdcDemoToolbar` — the blue bar of buttons that dispatch sample events
   via `IDemoVdcPushFactory`.

For production, implement `IValidationEventStream` backed by a Submaster
SignalR hub subscription (reusing your existing Redis pub-sub backplane).
The implementation should:

- On `StartAsync`, open the hub connection and subscribe to match-scoped
  VDC events.
- Transform each incoming hub payload into a `MatchEvent` subtype.
- Invoke `OnEventReceived`, to which the page (or a host service) will
  subscribe and call `Session.PushPending(...)`.

Then hide the demo toolbar by passing `ShowDemoToolbar="false"` to the page,
or toggle it off via configuration / feature flag.

### 4.2 The squad provider

`StaticSquadProvider` returns the Bayern/Madrid rosters from the prototype.
Replace with an HTTP-backed implementation that calls the Lumiaro squad
API. Keep the signature — `Task<(Squad Home, Squad Away)> GetSquadsAsync(...)`.
`Team.Key` should be assigned based on which side of the fixture they are,
not a property of the team itself.

### 4.3 The charge code provider

`StaticChargeCodeProvider` carries the full UEFA C-codes and S-codes inline.
If the UEFA reference-data service exposes these, swap to an API-backed impl
that caches the list per match or per session. The `RequiresOffence` /
`Offences` shape maps directly to what the UI expects.

### 4.4 Finalization

`MatchValidation.razor.cs → HandleFinalize()` is a TODO. Post the
`ValidationSession` to the Lumiaro reporting API — you may want to project
to a DTO first to drop transient fields like `Source` and `SourceNote`.

---

## 5. Architectural notes

### 5.1 State ownership

All canonical state lives in `IMatchValidationSessionService` as an
immutable `ValidationSession` record. Mutations go through the service,
which raises `SessionChanged` so the page calls `StateHasChanged()`.
Children are presentational and emit intent via `EventCallback`.

This matches the Sefe `ThemeService` pattern and avoids prop-drilling
without sacrificing the single-source-of-truth invariant.

### 5.2 Polymorphic event model

`MatchEvent` is an abstract record with four concrete sub-records. The
session service, event row, and `EventEditPanel` all dispatch via `switch`
expressions on the concrete type. This keeps the union tight — if a new
event type is added later (e.g. VAR review), the compiler can't warn on
missing branches unless you add an exhaustive-check test, but the surface
area to update is obvious and local.

### 5.3 Result&lt;T&gt;

`QuickValidate` and `ConfirmEdited` return `Result<MatchEvent>`. The page
orchestrator currently ignores the return value; hook `.IsOk` into a
toast/snackbar system if you want referees to see validation failures.

### 5.4 Minute formatting

`IMinuteFormatter` centralises the "45+2'" / "90+3'" logic. It's a pure
function of `(minute, MatchPhase)`, which is easy to unit-test — good
candidate for your FsCheck harness once that's plumbed into Lumiaro.

### 5.5 The demo toolbar

Kept in the library deliberately so the component can be driven without a
live VDC feed during development, UI polish, and Storybook-style review.
Production builds should disable it via the `ShowDemoToolbar` parameter.

---

## 6. What's not yet here

- **Authentication integration** — the `UserLabel` parameter is a string.
  Wire it to `AuthenticationStateProvider` in Lumiaro's standard way.
- **Localisation** — strings are inline English. Lumiaro already has the
  multilingual report plumbing; move the user-facing strings into resx.
- **Persistence** — the session is in-memory per circuit. If a referee
  refreshes, state is lost. Consider persisting the `ValidationSession` to
  Redis (you already have it for Submaster) keyed by `(matchId, refereeId)`
  and rehydrating in `OnInitializedAsync`.
- **TracedValue&lt;T&gt; lineage** — a natural fit here: every event has an
  origin (VDC / manual / correction) and a validated-by timestamp. If you
  want to carry lineage through, wrap the event fields in `TracedValue<T>`
  in a follow-up pass.
- **Audit trail** — `ValidationSession` could hold an append-only
  `ImmutableList<AuditEntry>` of every mutation for replay and compliance.
