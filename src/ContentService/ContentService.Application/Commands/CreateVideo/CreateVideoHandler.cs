namespace ContentService.Application.Commands.CreateVideo;

using MediatR;
using Domain.Entities;
using Domain.Interfaces;

public class CreateVideoHandler : IRequestHandler<CreateVideoCommand, string>
{
    private readonly IContentRepository _repository;

    public CreateVideoHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(CreateVideoCommand request, CancellationToken cancellationToken)
    {
        var video = new Video
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title,
            Url = request.Url,
            DurationSeconds = request.DurationSeconds,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(video, cancellationToken);
        return video.Id;
    }
}