namespace ContentService.Domain.Interfaces;

using Entities;

public interface IContentRepository
{
    Task<List<ContentItem>> GetAllAsync(CancellationToken ct = default);
    Task<ContentItem?> GetByIdAsync(string id, CancellationToken ct = default);
    Task AddAsync(ContentItem item, CancellationToken ct = default);
}