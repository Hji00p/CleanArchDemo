namespace UserService.Domain.Interfaces;

using Entities;

public interface IUserRepository
{
    Task<User?> GetByLoginAsync(string login, CancellationToken ct = default);
    Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}
