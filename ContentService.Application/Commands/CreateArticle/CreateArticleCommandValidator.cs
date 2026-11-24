namespace ContentService.Application.Commands.CreateArticle;

using FluentValidation;

public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
    }
}
