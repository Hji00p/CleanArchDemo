namespace ContentService.Application.Commands.CreateVideo;

using FluentValidation;

public class CreateVideoCommandValidator : AbstractValidator<CreateVideoCommand>
{
    public CreateVideoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Url)
            .NotEmpty()
            .Must(BeHttpUrl)
            .WithMessage("Url має бути абсолютним http/https посиланням");
        RuleFor(x => x.DurationSeconds).GreaterThan(0);
        RuleFor(x => x.AuthorId).NotEmpty();
    }

    private static bool BeHttpUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
