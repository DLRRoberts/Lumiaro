using RedZone.LumiaroAdmin.Data.Entities;

namespace LumiaroAdmin.Data.Repositories;

public interface IInterventionReviewRepository : IRepository<InterventionReviewEntity>
{
    Task<List<InterventionReviewEntity>> GetByReportAsync(int reportId, CancellationToken ct = default);
    Task<List<InterventionReviewEntity>> GetByReportAndEventAsync(int reportId, int eventId, CancellationToken ct = default);
    Task<List<InterventionReviewEntity>> GetByReviewerAsync(int reportId, string reviewerName, CancellationToken ct = default);
    Task<bool> HasReviewedAsync(int reportId, string reviewerName, CancellationToken ct = default);
    Task<int> GetReviewerCountAsync(int reportId, CancellationToken ct = default);
    Task<int> GetReviewCountAsync(int reportId, CancellationToken ct = default);
    Task RemoveByReviewerAsync(int reportId, string reviewerName, CancellationToken ct = default);
    
    Task SaveReviewsAsync(ICollection<InterventionReviewEntity> review);
}