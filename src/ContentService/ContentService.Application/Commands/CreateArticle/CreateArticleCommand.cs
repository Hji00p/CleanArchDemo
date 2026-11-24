namespace ContentService.Application.Commands.CreateArticle;

using MediatR;

public record CreateArticleCommand(string Title, string Body, string AuthorId)
    : IRequest<string>;