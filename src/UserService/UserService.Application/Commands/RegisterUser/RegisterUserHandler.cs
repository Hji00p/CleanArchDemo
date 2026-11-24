namespace UserService.Application.Commands.RegisterUser;

using MediatR;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Exceptions;
using Interfaces;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        if (await _repository.ExistsByLoginAsync(request.Login, ct))
            throw new ConflictException("Користувач з таким логіном вже існує");

        var userId = UserId.New();
        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(userId, request.Login, passwordHash, request.FullName);

        await _repository.AddAsync(user, ct);

        return userId.Value;
    }
}
