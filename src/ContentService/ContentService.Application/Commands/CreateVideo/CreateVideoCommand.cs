namespace ContentService.Application.Commands.CreateVideo;

using MediatR;

public record CreateVideoCommand(string Title, string Url, int DurationSeconds, string AuthorId)
    : IRequest<string>;