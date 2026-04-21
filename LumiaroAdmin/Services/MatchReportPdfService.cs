using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using LumiaroAdmin.Models;

namespace LumiaroAdmin.Services;

public interface IMatchReportPdfService
{
    MatchReportPdfFile Generate(MatchOfficialReport report, MatchReportPdfOptions? options = null);
}

public sealed record MatchReportPdfFile(byte[] Content, string FileName, string ContentType = "application/pdf");

public enum MatchReportPdfExportMode
{
    Internal,
    Public
}

public enum MatchReportPdfThemePreset
{
    Auto,
    Lumiaro,
    Uefa,
    PremierLeague,
    Efl,
    Neutral
}

public sealed record MatchReportPdfOptions
{
    public MatchReportPdfExportMode ExportMode { get; init; } = MatchReportPdfExportMode.Internal;
    public MatchReportPdfThemePreset ThemePreset { get; init; } = MatchReportPdfThemePreset.Auto;
    public string ReportTitle { get; init; } = "Lumiaro Match Official Report";
    public string BrandName { get; init; } = "Lumiaro";
    public string DistributionLabel { get; init; } = "Internal Review Copy";
    public string AccentColor { get; init; } = "#0EA5E9";
    public string AccentSoftColor { get; init; } = "#E0F2FE";
    public string HeadingColor { get; init; } = "#0F172A";
    public string TextColor { get; init; } = "#334155";
    public string MutedColor { get; init; } = "#64748B";
    public string BorderColor { get; init; } = "#CBD5E1";
    public string HeaderBackgroundColor { get; init; } = "#F8FAFC";
    public string SectionBackgroundColor { get; init; } = "#FFFFFF";
    public string HighlightBackgroundColor { get; init; } = "#F8FAFC";
    public bool IncludeConfidentialNotes { get; init; } = true;
}

public sealed class MatchReportPdfService : IMatchReportPdfService
{
    static MatchReportPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public MatchReportPdfFile Generate(MatchOfficialReport report, MatchReportPdfOptions? options = null)
    {
        options ??= new MatchReportPdfOptions();
        var resolvedOptions = ResolveOptions(report, options);

        var document = new MatchReportPdfDocument(report, resolvedOptions);
        var fileName = BuildFileName(report, resolvedOptions);
        var content = document.GeneratePdf();

        return new MatchReportPdfFile(content, fileName);
    }

    private static string BuildFileName(MatchOfficialReport report, MatchReportPdfOptions options)
    {
        var matchTitle = SanitizeFileNamePart(report.MatchTitle);
        var official = SanitizeFileNamePart(report.RefereeName);
        var date = report.MatchDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        var mode = options.ExportMode == MatchReportPdfExportMode.Public ? "public" : "internal";

        return $"{date}_{matchTitle}_{official}_{mode}_report.pdf";
    }

    private static MatchReportPdfOptions ResolveOptions(MatchOfficialReport report, MatchReportPdfOptions options)
    {
        var defaults = new MatchReportPdfOptions();
        var preset = options.ThemePreset == MatchReportPdfThemePreset.Auto
            ? InferThemePreset(report)
            : options.ThemePreset;

        var themed = GetThemeOptions(preset);

        return themed with
        {
            ExportMode = options.ExportMode,
            ThemePreset = preset,
            DistributionLabel = options.ExportMode == MatchReportPdfExportMode.Public ? "Public Distribution" : "Internal Review Copy",
            IncludeConfidentialNotes = options.ExportMode == MatchReportPdfExportMode.Internal && options.IncludeConfidentialNotes,
            ReportTitle = options.ReportTitle != defaults.ReportTitle ? options.ReportTitle : themed.ReportTitle,
            BrandName = options.BrandName != defaults.BrandName ? options.BrandName : themed.BrandName,
            AccentColor = options.AccentColor != defaults.AccentColor ? options.AccentColor : themed.AccentColor,
            AccentSoftColor = options.AccentSoftColor != defaults.AccentSoftColor ? options.AccentSoftColor : themed.AccentSoftColor,
            HeadingColor = options.HeadingColor != defaults.HeadingColor ? options.HeadingColor : themed.HeadingColor,
            TextColor = options.TextColor != defaults.TextColor ? options.TextColor : themed.TextColor,
            MutedColor = options.MutedColor != defaults.MutedColor ? options.MutedColor : themed.MutedColor,
            BorderColor = options.BorderColor != defaults.BorderColor ? options.BorderColor : themed.BorderColor,
            HeaderBackgroundColor = options.HeaderBackgroundColor != defaults.HeaderBackgroundColor ? options.HeaderBackgroundColor : themed.HeaderBackgroundColor,
            SectionBackgroundColor = options.SectionBackgroundColor != defaults.SectionBackgroundColor ? options.SectionBackgroundColor : themed.SectionBackgroundColor,
            HighlightBackgroundColor = options.HighlightBackgroundColor != defaults.HighlightBackgroundColor ? options.HighlightBackgroundColor : themed.HighlightBackgroundColor,
        };
    }

    private static MatchReportPdfThemePreset InferThemePreset(MatchOfficialReport report)
    {
        var competition = report.Competition?.Trim().ToLowerInvariant() ?? string.Empty;

        if (competition.Contains("uefa") || competition.Contains("fifa") || report.CompetitionTier == RefereeTier.FifaInternational)
            return MatchReportPdfThemePreset.Uefa;

        if (competition.Contains("premier") || report.CompetitionTier == RefereeTier.PremierLeague)
            return MatchReportPdfThemePreset.PremierLeague;

        if (competition.Contains("championship") || competition.Contains("league one") || competition.Contains("league two") || competition.Contains("efl") ||
            report.CompetitionTier == RefereeTier.Championship || report.CompetitionTier == RefereeTier.LeagueOne || report.CompetitionTier == RefereeTier.LeagueTwo)
            return MatchReportPdfThemePreset.Efl;

        return MatchReportPdfThemePreset.Lumiaro;
    }

    private static MatchReportPdfOptions GetThemeOptions(MatchReportPdfThemePreset preset) => preset switch
    {
        MatchReportPdfThemePreset.Uefa => new MatchReportPdfOptions
        {
            ThemePreset = MatchReportPdfThemePreset.Uefa,
            ReportTitle = "UEFA Match Official Report",
            BrandName = "UEFA",
            AccentColor = "#0B4F8A",
            AccentSoftColor = "#E0F2FE",
            HeadingColor = "#082F49",
            TextColor = "#1F2937",
            MutedColor = "#475569",
            BorderColor = "#BFDBFE",
            HeaderBackgroundColor = "#EFF6FF",
            SectionBackgroundColor = "#FFFFFF",
            HighlightBackgroundColor = "#F8FAFC"
        },
        MatchReportPdfThemePreset.PremierLeague => new MatchReportPdfOptions
        {
            ThemePreset = MatchReportPdfThemePreset.PremierLeague,
            ReportTitle = "Premier League Match Official Report",
            BrandName = "Premier League",
            AccentColor = "#6D28D9",
            AccentSoftColor = "#F3E8FF",
            HeadingColor = "#3B0764",
            TextColor = "#312E81",
            MutedColor = "#6B7280",
            BorderColor = "#DDD6FE",
            HeaderBackgroundColor = "#FAF5FF",
            SectionBackgroundColor = "#FFFFFF",
            HighlightBackgroundColor = "#F5F3FF"
        },
        MatchReportPdfThemePreset.Efl => new MatchReportPdfOptions
        {
            ThemePreset = MatchReportPdfThemePreset.Efl,
            ReportTitle = "EFL Match Official Report",
            BrandName = "EFL",
            AccentColor = "#DC2626",
            AccentSoftColor = "#FEE2E2",
            HeadingColor = "#7F1D1D",
            TextColor = "#3F3F46",
            MutedColor = "#71717A",
            BorderColor = "#FECACA",
            HeaderBackgroundColor = "#FEF2F2",
            SectionBackgroundColor = "#FFFFFF",
            HighlightBackgroundColor = "#FFF7ED"
        },
        MatchReportPdfThemePreset.Neutral => new MatchReportPdfOptions
        {
            ThemePreset = MatchReportPdfThemePreset.Neutral,
            ReportTitle = "Match Official Report",
            BrandName = "Neutral",
            AccentColor = "#334155",
            AccentSoftColor = "#E2E8F0",
            HeadingColor = "#0F172A",
            TextColor = "#334155",
            MutedColor = "#64748B",
            BorderColor = "#CBD5E1",
            HeaderBackgroundColor = "#F8FAFC",
            SectionBackgroundColor = "#FFFFFF",
            HighlightBackgroundColor = "#F1F5F9"
        },
        _ => new MatchReportPdfOptions
        {
            ThemePreset = MatchReportPdfThemePreset.Lumiaro,
            ReportTitle = "Lumiaro Match Official Report",
            BrandName = "Lumiaro"
        }
    };

    private static string SanitizeFileNamePart(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "match-report";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var chars = value
            .Trim()
            .Select(ch => invalidChars.Contains(ch) ? '-' : ch)
            .ToArray();

        return new string(chars)
            .Replace(" ", "-")
            .Replace("--", "-")
            .Trim('-');
    }

    private sealed class MatchReportPdfDocument : IDocument
    {
        private readonly MatchOfficialReport _report;
        private readonly MatchReportPdfOptions _options;

        public MatchReportPdfDocument(MatchOfficialReport report, MatchReportPdfOptions options)
        {
            _report = report;
            _options = options;
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"{_options.ReportTitle} - {_report.MatchTitle}",
            Author = _report.AuthorName,
            Subject = _options.DistributionLabel,
            Keywords = "Lumiaro, referee, match report, pdf"
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(_options.TextColor).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingVertical(12).Column(column =>
                {
                    column.Spacing(16);
                    column.Item().Element(ComposeSummary);
                    column.Item().Element(ComposeScoreBreakdown);
                    column.Item().Element(ComposeSections);

                    if (_report.Events.Count > 0)
                        column.Item().Element(ComposeEvents);

                    if (_report.VideoClips.Count > 0)
                        column.Item().Element(ComposeVideoClips);

                    if (HasConclusionContent())
                        column.Item().Element(ComposeConclusion);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span($"Generated {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC").FontSize(9).FontColor(_options.MutedColor);
                    text.Span("  •  ").FontSize(9).FontColor(_options.MutedColor);
                    text.CurrentPageNumber().FontSize(9).FontColor(_options.MutedColor);
                    text.Span(" / ").FontSize(9).FontColor(_options.MutedColor);
                    text.TotalPages().FontSize(9).FontColor(_options.MutedColor);
                });
            });
        }

        private bool HasConclusionContent() =>
            !string.IsNullOrWhiteSpace(_report.Conclusion) ||
            !string.IsNullOrWhiteSpace(_report.Recommendations) ||
            (_options.IncludeConfidentialNotes && !string.IsNullOrWhiteSpace(_report.ConfidentialNotes));

        private void ComposeHeader(IContainer container)
        {
            container
                .Background(_options.HeaderBackgroundColor)
                .Border(1)
                .BorderColor(_options.BorderColor)
                .Padding(18)
                .Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Spacing(4);
                        column.Item().Text(_options.ReportTitle).FontSize(22).SemiBold().FontColor(_options.HeadingColor);
                        column.Item().Row(tags =>
                        {
                            tags.AutoItem().Element(tag => ComposeHeaderTag(tag, _options.BrandName, _options.AccentSoftColor, _options.AccentColor));
                            tags.AutoItem().PaddingLeft(6).Element(tag => ComposeHeaderTag(tag, _options.DistributionLabel, _options.HighlightBackgroundColor, _options.HeadingColor));
                        });
                        column.Item().Text(_report.MatchTitle).FontSize(14).SemiBold().FontColor(_options.TextColor);
                        column.Item().Text(text =>
                        {
                            text.Span(_report.DateDisplay).FontColor(_options.MutedColor);
                            if (!string.IsNullOrWhiteSpace(_report.Competition))
                            {
                                text.Span("  •  ").FontColor(_options.MutedColor);
                                text.Span(_report.Competition).FontColor(_options.MutedColor);
                            }

                            if (!string.IsNullOrWhiteSpace(_report.Venue))
                            {
                                text.Span("  •  ").FontColor(_options.MutedColor);
                                text.Span(_report.Venue).FontColor(_options.MutedColor);
                            }
                        });
                    });

                    row.ConstantItem(140).AlignRight().Element(card =>
                        ComposeMetricCard(card, "Overall Rating", _report.OverallRating.ToString("F1"), _options.AccentColor));
                });
        }

        private void ComposeSummary(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(12);
                column.Item().Element(c => ComposeSectionHeader(c, "Report Overview"));
                column.Item().Row(row =>
                {
                    row.Spacing(12);

                    row.RelativeItem().Element(card => ComposeInfoCard(card, "Match Details", new[]
                    {
                        ("Match", _report.MatchTitle),
                        ("Competition", ValueOrDash(_report.Competition)),
                        ("Score", _report.ScoreDisplay),
                        ("Venue", ValueOrDash(_report.Venue))
                    }));

                    row.RelativeItem().Element(card => ComposeInfoCard(card, "Official Details", new[]
                    {
                        ("Official", _report.RefereeName),
                        ("Code", _report.RefereeCode),
                        ("Role", _report.OfficialRole.Label()),
                        ("Tier", GetTierLabel(_report.RefereeTier))
                    }));

                    row.RelativeItem().Element(card => ComposeInfoCard(card, "Report Metadata", new[]
                    {
                        ("Status", _report.StatusLabel),
                        ("Author", _report.AuthorName),
                        ("Theme", GetThemeLabel(_options.ThemePreset)),
                        ("Distribution", _options.DistributionLabel),
                        ("Created", _report.CreatedAt.ToString("dd MMM yyyy HH:mm")),
                        ("Updated", _report.UpdatedAt?.ToString("dd MMM yyyy HH:mm") ?? "—")
                    }));
                });
            });
        }

        private void ComposeScoreBreakdown(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Element(c => ComposeSectionHeader(c, "Score Breakdown"));
                column.Item().Element(card =>
                {
                    ComposeCard(card).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(TableHeaderCell).Text("Section").SemiBold();
                            header.Cell().Element(TableHeaderCell).AlignRight().Text("Rating").SemiBold();
                        });

                        AddScoreRow(table, "Performance Summary", _report.PerformanceSummary.Rating);
                        AddScoreRow(table, "Decision Making", _report.DecisionMaking.Rating);
                        AddScoreRow(table, "Positioning & Movement", _report.Positioning.Rating);
                        AddScoreRow(table, "Communication", _report.Communication.Rating);
                        AddScoreRow(table, "Management Style", _report.ManagementStyle.Rating);
                        AddScoreRow(table, "Fitness & Work Rate", _report.FitnessAndWorkRate.Rating);
                        AddScoreRow(table, "Overall Rating", _report.OverallRating, true);
                    });
                });
            });
        }

        private void ComposeSections(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Element(c => ComposeSectionHeader(c, "Performance Sections"));

                foreach (var (title, section) in GetSections())
                {
                    column.Item().Element(card => ComposeReportSection(card, title, section));
                }
            });
        }

        private void ComposeEvents(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Element(c => ComposeSectionHeader(c, "Event Timeline"));

                foreach (var evt in _report.Events.OrderBy(e => e.Minute).ThenBy(e => e.AddedTime ?? 0))
                {
                    column.Item().Element(card =>
                    {
                        ComposeCard(card).Column(content =>
                        {
                            content.Spacing(6);
                            content.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"{evt.MinuteDisplay} · {evt.EventTypeLabel}").SemiBold().FontColor(_options.HeadingColor);
                                row.ConstantItem(95).AlignRight().Text(evt.ImpactLabel).SemiBold().FontColor(MapImpactColor(evt.Impact));
                            });

                            if (!string.IsNullOrWhiteSpace(evt.Teams))
                                content.Item().Text($"Team: {evt.Teams}").FontColor(_options.MutedColor);

                            content.Item().Text(evt.Description);

                            if (!string.IsNullOrWhiteSpace(evt.OfficialResponse))
                            {
                                content.Item().PaddingTop(4).Element(highlight =>
                                {
                                    highlight.Background(_options.HighlightBackgroundColor)
                                        .BorderLeft(3)
                                        .BorderColor(_options.AccentColor)
                                        .Padding(10)
                                        .Text(text =>
                                        {
                                            text.Span("Official response: ").SemiBold();
                                            text.Span(evt.OfficialResponse);
                                        });
                                });
                            }

                            content.Item().DefaultTextStyle(x => x.FontColor(_options.MutedColor)).Text(text =>
                            {
                                text.Span("Official involved: ").SemiBold();
                                text.Span(evt.OfficialInvolved ? "Yes" : "No");
                                text.Span("  •  ");
                                text.Span("Decision accuracy: ").SemiBold();
                                text.Span(GetDecisionAccuracyLabel(evt.Accuracy));
                            });
                        });
                    });
                }
            });
        }

        private void ComposeVideoClips(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Element(c => ComposeSectionHeader(c, "Video Evidence"));

                foreach (var clip in _report.VideoClips)
                {
                    column.Item().Element(card =>
                    {
                        ComposeCard(card).Column(content =>
                        {
                            content.Spacing(5);
                            content.Item().Text(clip.Title).SemiBold().FontColor(_options.HeadingColor);

                            if (!string.IsNullOrWhiteSpace(clip.Description))
                                content.Item().Text(clip.Description);

                            var meta = new List<string>();
                            if (!string.IsNullOrWhiteSpace(clip.FileName)) meta.Add(clip.FileName);
                            if (clip.FileSize > 0) meta.Add(clip.FileSizeDisplay);
                            if (clip.MatchMinute.HasValue) meta.Add(clip.MinuteDisplay);
                            if (!string.IsNullOrWhiteSpace(clip.LinkedEventDescription)) meta.Add($"Linked: {clip.LinkedEventDescription}");

                            if (meta.Count > 0)
                                content.Item().Text(string.Join("  •  ", meta)).FontColor(_options.MutedColor);
                        });
                    });
                }
            });
        }

        private void ComposeConclusion(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Element(c => ComposeSectionHeader(c, "Conclusion & Recommendations"));

                if (!string.IsNullOrWhiteSpace(_report.Conclusion))
                    column.Item().Element(card => ComposeTextBlock(card, "Conclusion", _report.Conclusion!));

                if (!string.IsNullOrWhiteSpace(_report.Recommendations))
                    column.Item().Element(card => ComposeTextBlock(card, "Recommendations", _report.Recommendations!));

                if (_options.IncludeConfidentialNotes && !string.IsNullOrWhiteSpace(_report.ConfidentialNotes))
                    column.Item().Element(card => ComposeTextBlock(card, "Confidential Notes", _report.ConfidentialNotes!, "#991B1B", "#FEF2F2"));
            });
        }

        private void ComposeInfoCard(IContainer container, string title, IEnumerable<(string Label, string Value)> items)
        {
            ComposeCard(container).Column(column =>
            {
                column.Spacing(8);
                column.Item().Text(title).SemiBold().FontColor(_options.HeadingColor);

                foreach (var (label, value) in items)
                {
                    column.Item().Text(text =>
                    {
                        text.Span(label + ": ").SemiBold();
                        text.Span(value);
                    });
                }
            });
        }

        private void ComposeReportSection(IContainer container, string title, ReportSection section)
        {
            ComposeCard(container).Column(column =>
            {
                column.Spacing(8);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text(title).SemiBold().FontColor(_options.HeadingColor);
                    row.ConstantItem(60).AlignRight().Text(section.Rating.ToString("F1")).SemiBold().FontColor(_options.AccentColor);
                });

                if (!string.IsNullOrWhiteSpace(section.Narrative))
                    column.Item().Text(section.Narrative);

                if (!string.IsNullOrWhiteSpace(section.KeyObservations))
                    column.Item().Element(block => ComposeCallout(block, "Key Observations", section.KeyObservations!, _options.AccentColor, _options.AccentSoftColor));

                if (!string.IsNullOrWhiteSpace(section.AreasOfConcern))
                    column.Item().Element(block => ComposeCallout(block, "Areas of Concern", section.AreasOfConcern!, "#F97316", "#FFF7ED"));
            });
        }

        private void ComposeTextBlock(IContainer container, string title, string content, string? titleColor = null, string? background = null)
        {
            ComposeCard(container, background ?? _options.SectionBackgroundColor).Column(column =>
            {
                column.Spacing(6);
                column.Item().Text(title).SemiBold().FontColor(titleColor ?? _options.HeadingColor);
                column.Item().Text(content);
            });
        }

        private void ComposeCallout(IContainer container, string title, string content, string borderColor, string background)
        {
            container.Background(background)
                .BorderLeft(3)
                .BorderColor(borderColor)
                .Padding(10)
                .Column(column =>
                {
                    column.Spacing(4);
                    column.Item().Text(title).SemiBold().FontColor(_options.HeadingColor);
                    column.Item().Text(content);
                });
        }

        private void ComposeSectionHeader(IContainer container, string title)
        {
            container.Text(title).FontSize(14).SemiBold().FontColor(_options.HeadingColor);
        }

        private void ComposeMetricCard(IContainer container, string label, string value, string accentColor)
        {
            container.Border(1)
                .BorderColor(_options.BorderColor)
                .Background(Colors.White)
                .Padding(12)
                .Column(column =>
                {
                    column.Spacing(4);
                    column.Item().AlignRight().Text(label).FontSize(9).FontColor(_options.MutedColor);
                    column.Item().AlignRight().Text(value).FontSize(24).SemiBold().FontColor(accentColor);
                });
        }

        private void ComposeHeaderTag(IContainer container, string text, string background, string foreground)
        {
            container
                .Background(background)
                .Border(1)
                .BorderColor(_options.BorderColor)
                .PaddingVertical(4)
                .PaddingHorizontal(8)
                .Text(text)
                .FontSize(8)
                .SemiBold()
                .FontColor(foreground);
        }

        private IContainer ComposeCard(IContainer container, string? background = null)
        {
            return container
                .Border(1)
                .BorderColor(_options.BorderColor)
                .Background(background ?? _options.SectionBackgroundColor)
                .Padding(14);
        }

        private IContainer TableHeaderCell(IContainer container)
        {
            return container
                .Background(_options.HighlightBackgroundColor)
                .BorderBottom(1)
                .BorderColor(_options.BorderColor)
                .PaddingVertical(8)
                .PaddingHorizontal(10);
        }

        private void AddScoreRow(TableDescriptor table, string label, double rating, bool emphasise = false)
        {
            var leftCell = table.Cell().Element(cell => TableBodyCell(cell, emphasise));
            var leftText = leftCell.Text(label);
            if (emphasise)
                leftText.SemiBold();

            var rightCell = table.Cell().Element(cell => TableBodyCell(cell, emphasise)).AlignRight();
            var rightText = rightCell.Text(rating.ToString("F1")).FontColor(emphasise ? _options.AccentColor : _options.TextColor);
            if (emphasise)
                rightText.SemiBold();
        }

        private IContainer TableBodyCell(IContainer container, bool emphasise)
        {
            return container
                .BorderBottom(1)
                .BorderColor(_options.BorderColor)
                .Background(emphasise ? _options.AccentSoftColor : Colors.White)
                .PaddingVertical(8)
                .PaddingHorizontal(10);
        }

        private IEnumerable<(string Title, ReportSection Section)> GetSections()
        {
            yield return ("Performance Summary", _report.PerformanceSummary);
            yield return ("Decision Making", _report.DecisionMaking);
            yield return ("Positioning & Movement", _report.Positioning);
            yield return ("Communication", _report.Communication);
            yield return ("Management Style", _report.ManagementStyle);
            yield return ("Fitness & Work Rate", _report.FitnessAndWorkRate);
        }

        private static string ValueOrDash(string? value) => string.IsNullOrWhiteSpace(value) ? "—" : value;

        private static string GetTierLabel(RefereeTier tier) => tier switch
        {
            RefereeTier.FifaInternational => "FIFA International",
            RefereeTier.PremierLeague => "Premier League",
            RefereeTier.Championship => "Championship",
            RefereeTier.LeagueOne => "League One",
            RefereeTier.LeagueTwo => "League Two",
            RefereeTier.NationalLeague => "National League",
            RefereeTier.Grassroots => "Grassroots",
            _ => "—"
        };

        private static string GetDecisionAccuracyLabel(DecisionAccuracy accuracy) => accuracy switch
        {
            DecisionAccuracy.Correct => "Correct",
            DecisionAccuracy.Incorrect => "Incorrect",
            DecisionAccuracy.Debatable => "Debatable",
            DecisionAccuracy.NotApplicable => "N/A",
            _ => "—"
        };

        private static string GetThemeLabel(MatchReportPdfThemePreset preset) => preset switch
        {
            MatchReportPdfThemePreset.Uefa => "UEFA",
            MatchReportPdfThemePreset.PremierLeague => "Premier League",
            MatchReportPdfThemePreset.Efl => "EFL",
            MatchReportPdfThemePreset.Neutral => "Neutral",
            MatchReportPdfThemePreset.Lumiaro => "Lumiaro",
            _ => "Auto"
        };

        private static string MapImpactColor(EventImpact impact) => impact switch
        {
            EventImpact.Low => "#16A34A",
            EventImpact.Medium => "#2563EB",
            EventImpact.High => "#EA580C",
            EventImpact.Critical => "#DC2626",
            _ => "#334155"
        };
    }
}

