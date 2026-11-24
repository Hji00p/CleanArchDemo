namespace UserService.API.Requests;

using Application.Commands.RegisterUser;

public record UserRegistrationRequest(string Login, string Password, string FullName)
{
    public RegisterUserCommand ToCommand() => new(Login, Password, FullName);
}