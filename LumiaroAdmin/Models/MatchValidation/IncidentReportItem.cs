namespace Lumiaro.MatchValidation.Models;

/// <summary>
/// An entry on the post-match incident report. Red cards and second-yellows
/// auto-generate mandatory items; the referee can also add general notes.
/// </summary>
public record IncidentReportItem(
    Guid Id,
    IncidentCategory Category,
    string Title,
    bool IsMandatory,
    string Description,
    Guid? LinkedEventId = null)
{
    public bool IsSatisfied => !IsMandatory || Description.Trim().Length > 10;
}

public enum IncidentCategory
{
    RedCard,
    General
}
