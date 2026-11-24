namespace UserService.Application.Interfaces;

using Domain.Entities;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}