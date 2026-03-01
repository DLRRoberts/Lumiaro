using LumiaroAdmin.Services;
using LumiaroAdmin.Components;
using LumiaroAdmin.Data;
using LumiaroAdmin.Data.Profiles;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using RedZone.LumiaroAdmin.Data;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

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
builder.Services.AddScoped<InterventionReviewService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
