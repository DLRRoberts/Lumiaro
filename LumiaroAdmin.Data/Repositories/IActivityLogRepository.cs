using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Repositories;

public interface IActivityLogRepository : IRepository<ActivityLogEntity>
{
    Task<List<ActivityLogEntity>> GetRecentAsync(int count = 10, CancellationToken ct = default);
}