using LumiaroAdmin.Services;
using LumiaroAdmin.Components;
using LumiaroAdmin.Data;
using LumiaroAdmin.Data.Profiles;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using QuestPDF.Infrastructure;
using RedZone.LumiaroAdmin.Data;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

// Configure antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddAutoMapper(ex => ex.AddProfiles([
    new DtoToModelProfile()
]));

// ── Entity Framework data layer (SQL Server) ──
var connectionString = builder.Configuration.GetConnectionString("LumiaroDb")
    ?? throw new InvalidOperationException("Connection string 'LumiaroDb' not found in configuration.");
builder.Services.AddLumiaroData(connectionString);

// ── In-memory services (retained during migration to EF) ──
builder.Services.AddScoped<RefereeService>();
builder.Services.AddScoped<FixtureService>();
builder.Services.AddScoped<AssessmentService>();
builder.Services.AddScoped<MatchReportService>();
builder.Services.AddScoped<IMatchReportPdfService, MatchReportPdfService>();
builder.Services.AddScoped<InterventionReviewService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

// ── Match Validation ──
builder.Services.AddScoped<Lumiaro.MatchValidation.Services.IMatchValidationSessionService, Lumiaro.MatchValidation.Services.MatchValidationSessionService>();
builder.Services.AddSingleton<Lumiaro.MatchValidation.Services.IChargeCodeProvider, Lumiaro.MatchValidation.Services.StaticChargeCodeProvider>();
builder.Services.AddSingleton<Lumiaro.MatchValidation.Services.IMinuteFormatter, Lumiaro.MatchValidation.Services.MinuteFormatter>();
builder.Services.AddScoped<Lumiaro.MatchValidation.Services.ISquadProvider, Lumiaro.MatchValidation.Services.StaticSquadProvider>();
builder.Services.AddScoped<Lumiaro.MatchValidation.Services.IDemoVdcPushFactory, Lumiaro.MatchValidation.Services.DemoVdcPushFactory>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // In development, ensure the database schema is created
    await app.Services.EnsureLumiaroDatabaseCreatedAsync();
    await LumiaroDbSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/reports/{id:int}/pdf", async (int id, string? mode, string? theme, MatchReportService reportService, IMatchReportPdfService pdfService) =>
{
    var report = await reportService.GetByIdAsync(id);
    if (report is null)
    {
        return Results.NotFound();
    }

    var pdf = pdfService.Generate(report, new MatchReportPdfOptions
    {
        ExportMode = mode?.ToLowerInvariant() switch
        {
            "public" => MatchReportPdfExportMode.Public,
            _ => MatchReportPdfExportMode.Internal
        },
        ThemePreset = theme?.ToLowerInvariant() switch
        {
            "uefa" => MatchReportPdfThemePreset.Uefa,
            "premier" => MatchReportPdfThemePreset.PremierLeague,
            "efl" => MatchReportPdfThemePreset.Efl,
            "neutral" => MatchReportPdfThemePreset.Neutral,
            "lumiaro" => MatchReportPdfThemePreset.Lumiaro,
            _ => MatchReportPdfThemePreset.Auto
        }
    });

    return Results.File(pdf.Content, pdf.ContentType, pdf.FileName);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
