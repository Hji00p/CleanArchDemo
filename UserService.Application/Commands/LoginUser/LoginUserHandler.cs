namespace UserService.Application.Commands.LoginUser;

using MediatR;
using Domain.Interfaces;
using Exceptions;
using Interfaces;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly IUserRepository _repository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserHandler(
        IUserRepository repository,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByLoginAsync(request.Login, cancellationToken);

        // Одне повідомлення для обох випадків, щоб не підказувати, чи існує такий логін
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Невірний логін або пароль");

        return _jwtTokenGenerator.GenerateToken(user);
    }
}
