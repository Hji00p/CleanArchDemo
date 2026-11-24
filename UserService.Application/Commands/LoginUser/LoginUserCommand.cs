namespace UserService.Application.Commands.LoginUser;

using MediatR;

public record LoginUserCommand(string Login, string Password)
    : IRequest<string>; // повертає JWT токен