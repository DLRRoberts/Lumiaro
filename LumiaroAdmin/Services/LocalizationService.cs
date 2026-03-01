using System.Text.Json;
using Microsoft.JSInterop;

namespace LumiaroAdmin.Services;

/// <summary>
/// Provides localized strings from a JSON translations file.
/// Inject as @inject ILocalizationService L, then use @L["key"] in Razor markup.
/// </summary>
public interface ILocalizationService
{
    string this[string key] { get; }
    string CurrentLanguage { get; }
    string CurrentLanguageLabel { get; }
    IReadOnlyList<(string Code, string Label, string Flag)> AvailableLanguages { get; }
    Task InitializeAsync();
    Task SetLanguageAsync(string languageCode);
    event Action? OnLanguageChanged;
}

public class LocalizationService : ILocalizationService
{
    private readonly IWebHostEnvironment _env;
    private readonly IJSRuntime _js;

    private Dictionary<string, Dictionary<string, string>> _translations = new();
    private string _currentLanguage = "en";
    private bool _initialized;

    private static readonly List<(string Code, string Label, string Flag)> Languages = new()
    {
        ("en", "English", "🇬🇧"),
        ("fr", "Français", "🇫🇷"),
        ("es", "Español", "🇪🇸"),
        ("de", "Deutsch", "🇩🇪"),
        ("it", "Italiano", "🇮🇹"),
    };

    public IReadOnlyList<(string Code, string Label, string Flag)> AvailableLanguages => Languages;
    public string CurrentLanguage => _currentLanguage;
    public string CurrentLanguageLabel => Languages.FirstOrDefault(l => l.Code == _currentLanguage).Label ?? "English";

    public event Action? OnLanguageChanged;

    public LocalizationService(IWebHostEnvironment env, IJSRuntime js)
    {
        _env = env;
        _js = js;
    }

    public string this[string key]
    {
        get
        {
            if (_translations.TryGetValue(_currentLanguage, out var langDict) &&
                langDict.TryGetValue(key, out var value))
                return value;

            // Fallback to English
            if (_currentLanguage != "en" &&
                _translations.TryGetValue("en", out var enDict) &&
                enDict.TryGetValue(key, out var enValue))
                return enValue;

            // Return key as-is if no translation found (helps debug)
            return $"[{key}]";
        }
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        // Load the translations JSON from wwwroot
        var filePath = Path.Combine(_env.WebRootPath, "Translations", "translations.json");
        if (File.Exists(filePath))
        {
            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _translations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json, options)
                            ?? new();
        }

        // Restore saved language from sessionStorage via JS interop
        try
        {
            var saved = await _js.InvokeAsync<string?>("lumiaro.getLanguage");
            if (!string.IsNullOrEmpty(saved) && _translations.ContainsKey(saved))
                _currentLanguage = saved;
        }
        catch
        {
            // JS interop may not be available during prerender — use default
        }

        _initialized = true;
    }

    public async Task SetLanguageAsync(string languageCode)
    {
        if (!_translations.ContainsKey(languageCode)) return;

        _currentLanguage = languageCode;

        // Persist to sessionStorage
        try
        {
            await _js.InvokeVoidAsync("lumiaro.setLanguage", languageCode);
        }
        catch
        {
            // JS interop may not be available
        }

        OnLanguageChanged?.Invoke();
    }
}
