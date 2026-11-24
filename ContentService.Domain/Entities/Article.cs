namespace ContentService.Domain.Entities;

public class Article : ContentItem
{
    public string Body { get; set; } = string.Empty;
}