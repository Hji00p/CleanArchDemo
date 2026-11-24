namespace ContentService.Application.Commands.CreateArticle;

using MediatR;
using Domain.Entities;
using Domain.Interfaces;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, string>
{
    private readonly IContentRepository _repository;

    public CreateArticleHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = new Article
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title,
            Body = request.Body,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(article, cancellationToken);
        return article.Id;
    }
}