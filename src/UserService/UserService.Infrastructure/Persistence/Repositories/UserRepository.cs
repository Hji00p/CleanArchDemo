namespace UserService.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Application.Exceptions;
using Npgsql;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken ct = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login, ct);
    }

    public async Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default)
    {
        return await _context.Users.AnyAsync(u => u.Login == login, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Два одночасні запити з однаковим логіном: перевірку пройшли обидва, а індекс спрацював
            throw new ConflictException("Користувач з таким логіном вже існує");
        }
    }
}
