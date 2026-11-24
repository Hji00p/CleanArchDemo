namespace ContentService.API.Requests;

using Application.Commands.CreateVideo;

public record CreateVideoRequest(string Title, string Url, int DurationSeconds)
{
    public CreateVideoCommand ToCommand(string authorId) => new(Title, Url, DurationSeconds, authorId);
}
