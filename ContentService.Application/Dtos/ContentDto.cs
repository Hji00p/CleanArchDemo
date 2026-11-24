namespace ContentService.Application.Dtos;

using Domain.Entities;

// Плаский DTO: поля конкретного типу заповнені лише для нього (Article — Body, Video — Url і DurationSeconds)
public record ContentDto(
    string Id,
    string Type,
    string Title,
    string AuthorId,
    DateTime CreatedAt,
    string? Body = null,
    string? Url = null,
    int? DurationSeconds = null)
{
    public static ContentDto From(ContentItem item) => item switch
    {
        Article a => new(a.Id, "article", a.Title, a.AuthorId, a.CreatedAt, Body: a.Body),
        Video v => new(v.Id, "video", v.Title, v.AuthorId, v.CreatedAt, Url: v.Url, DurationSeconds: v.DurationSeconds),
        _ => new(item.Id, "unknown", item.Title, item.AuthorId, item.CreatedAt)
    };
}
