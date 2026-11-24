namespace ContentService.Infrastructure.Persistence;

using MongoDB.Driver;
using Domain.Entities;
using Domain.Interfaces;

public class ContentRepository : IContentRepository
{
    private readonly IMongoCollection<ContentItem> _collection;

    public ContentRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ContentItem>("content");
    }

    public async Task<List<ContentItem>> GetAllAsync(CancellationToken ct = default)
        => await _collection.Find(_ => true).ToListAsync(ct);

    public async Task<ContentItem?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task AddAsync(ContentItem item, CancellationToken ct = default)
        => await _collection.InsertOneAsync(item, null, ct);
}