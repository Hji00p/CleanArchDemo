namespace UserService.API.Requests;

using Application.Commands.LoginUser;

public record UserLoginRequest(string Login, string Password)
{
    public LoginUserCommand ToCommand() => new(Login, Password);
}