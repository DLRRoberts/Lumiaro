using LumiaroAdmin.Data.Entities;
using DomainEnums = LumiaroAdmin.Data.Enums;

namespace RedZone.LumiaroAdmin.Data.Mapping;

/// <summary>
/// Maps between RefereeEntity (EF/DB) and the domain Referee model used by the Blazor UI.
/// Domain models live in LumiaroAdmin.Models — these mappers bridge the two namespaces.
/// Consumers reference Data.Enums and map to domain enums in their own layer.
/// </summary>
public static class RefereeMapper
{
    public static RefereeEntity ToEntity(
        string refCode, string firstName, string lastName, DateTime dateOfBirth,
        string nationality, string location, string region,
        DomainEnums.RefereeTier tier, DomainEnums.RefereeStatus status,
        DateTime registrationDate,
        string? photoUrl = null, string? email = null, string? phone = null, string? notes = null)
    {
        return new RefereeEntity
        {
            RefCode = refCode,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Nationality = nationality,
            Location = location,
            Region = region,
            Tier = tier,
            Status = status,
            RegistrationDate = registrationDate,
            PhotoUrl = photoUrl,
            Email = email,
            Phone = phone,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(RefereeEntity entity,
        string firstName, string lastName, string nationality,
        string location, string region,
        DomainEnums.RefereeTier tier, DomainEnums.RefereeStatus status,
        string? photoUrl, string? email, string? phone, string? notes)
    {
        entity.FirstName = firstName;
        entity.LastName = lastName;
        entity.Nationality = nationality;
        entity.Location = location;
        entity.Region = region;
        entity.Tier = tier;
        entity.Status = status;
        entity.PhotoUrl = photoUrl;
        entity.Email = email;
        entity.Phone = phone;
        entity.Notes = notes;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static CareerEntryEntity ToCareerEntryEntity(int refereeId, string period, string title, string detail, bool isCurrent, int sortOrder)
    {
        return new CareerEntryEntity
        {
            RefereeId = refereeId,
            Period = period,
            Title = title,
            Detail = detail,
            IsCurrent = isCurrent,
            SortOrder = sortOrder
        };
    }

    public static SeasonStatsEntity ToSeasonStatsEntity(int refereeId, string season, DomainEnums.RefereeTier tier,
        int matches, int yellows, int reds, int penalties, double avgScore, double trendChange)
    {
        return new SeasonStatsEntity
        {
            RefereeId = refereeId,
            Season = season,
            Tier = tier,
            Matches = matches,
            Yellows = yellows,
            Reds = reds,
            Penalties = penalties,
            AvgScore = avgScore,
            TrendChange = trendChange
        };
    }
}
