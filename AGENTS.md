# AGENTS.md

## Scope
- Applies to the whole repo (`LumiaroAdmin.sln`) with 2 projects: `LumiaroAdmin` (Blazor Server UI) and `LumiaroAdmin.Data` (EF Core + repositories).

## Big Picture Architecture
- Startup composition is in `LumiaroAdmin/Program.cs`: Razor Components + Interactive Server + antiforgery + AutoMapper + `AddLumiaroData(connectionString)`.
- UI pages (`LumiaroAdmin/Components/Pages/*.razor`) call app services in `LumiaroAdmin/Services/*Service.cs`; services call repository interfaces from `LumiaroAdmin.Data/Repositories`.
- Data boundary is explicit: EF entities in `LumiaroAdmin.Data/Entities` are mapped to UI/domain models in `LumiaroAdmin/Models`.
- Mapping is currently mixed: AutoMapper profile (`LumiaroAdmin.Data/Profiles/DtoToModelProfile.cs`) and manual extension mappers (`LumiaroAdmin/Services/Mappings.cs`) both exist.
- `LumiaroAdmin.Data/LumiaroDbContext.cs` applies all `IEntityTypeConfiguration` from the assembly, so new entity rules belong in `LumiaroAdmin.Data/Configuration/*`.

## Data Flow Patterns You Should Reuse
- Typical page flow: `Assessments.razor` -> `AssessmentService` -> `IAssessmentRepository` -> EF query with `Include(...)` for needed graph.
- Repository methods are read-heavy and mostly `AsNoTracking()`; shape data in repository, not in Razor pages.
- Enum conversions are centralized in `LumiaroAdmin/Services/Mappings.cs` (`ToData()` / `ToDomain()`); do not duplicate ad-hoc casts.
- Fixture assignment conflict checks live in service layer (`FixtureService.AssignOfficialAsync`), not repository layer.
- Detail pages expect sorted child collections from repositories (e.g., report sections/events sorted in `MatchOfficialReportRepository`).

## Runtime and Environment Behavior
- In Development, app startup runs `EnsureCreatedAsync` and then `LumiaroDbSeeder.SeedAsync(...)` (`Program.cs`); seed runs only when no referees exist.
- Seeder (`LumiaroAdmin.Data/LumiaroDbSeeder.cs`) is canonical demo dataset and uses explicit identity insert for several tables.
- Localization is JSON-file based via `ILocalizationService` and `@inject ILocalizationService L` in `_Imports.razor`.
- Client-side language persistence depends on JS interop calls (`lumiaro.getLanguage` / `lumiaro.setLanguage`); check `wwwroot/js/app.js` when extending localization behavior.

## Developer Workflows
- Build solution: `dotnet build LumiaroAdmin.sln`
- Run web app: `dotnet run --project LumiaroAdmin/LumiaroAdmin.csproj`
- Local URLs are defined in `LumiaroAdmin/Properties/launchSettings.json` (`https://localhost:7200`, `http://localhost:5200`).
- There are no test projects in this repo currently; validate changes via targeted manual flows in affected Razor pages.
- No CI workflow is present (`.github/workflows/` is empty), so do not assume pipeline checks.

## Change Conventions (Project-Specific)
- Keep business/query logic in services and repositories; pages should orchestrate UI state and navigation only.
- When adding a new persisted feature, update in this order: `Entities` -> `Configuration` -> `DbContext` -> repository interface/impl -> service -> Razor page.
- Preserve namespace split already in use (`LumiaroAdmin.*` and `RedZone.LumiaroAdmin.*`) unless doing a deliberate repo-wide cleanup.
- For translations, add keys across all language sections in `LumiaroAdmin/wwwroot/Translations/Translations.json` to avoid `[key]` fallback rendering.

## Integration and Sensitive Config Notes
- SQL Server connection string is read from `ConnectionStrings:LumiaroDb` (`LumiaroAdmin/appsettings.json`).
- Azure publish profiles exist under `LumiaroAdmin/Properties/PublishProfiles/*.pubxml`; treat resource IDs, URLs, and usernames as sensitive metadata.
- Do not copy or echo secrets from `appsettings*.json` or publish profiles into commits, issues, or generated docs.

