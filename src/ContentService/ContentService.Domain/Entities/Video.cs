namespace ContentService.Domain.Entities;

public class Video : ContentItem
{
    public string Url { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}