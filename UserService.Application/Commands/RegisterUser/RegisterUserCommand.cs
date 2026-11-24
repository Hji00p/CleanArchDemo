namespace UserService.Application.Commands.RegisterUser;

using MediatR;

public record RegisterUserCommand(string Login, string Password, string FullName)
    : IRequest<Guid>;