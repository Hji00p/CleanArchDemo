namespace UserService.Application.Commands.LoginUser;

using FluentValidation;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Login).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
