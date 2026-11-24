namespace ContentService.API.Requests;

using Application.Commands.CreateArticle;

public record CreateArticleRequest(string Title, string Body)
{
    public CreateArticleCommand ToCommand(string authorId) => new(Title, Body, authorId);
}
