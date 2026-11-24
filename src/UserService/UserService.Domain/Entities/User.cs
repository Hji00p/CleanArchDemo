namespace UserService.Domain.Entities;

using ValueObjects;

public class User
{
    public UserId Id { get; private set; }
    public string Login { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }

    // Для EF Core: він сам заповнить властивості з бази одразу після створення обʼєкта
    private User()
    {
        Login = null!;
        PasswordHash = null!;
        FullName = null!;
    }

    public User(UserId id, string login, string passwordHash, string fullName)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        FullName = fullName;
    }
}