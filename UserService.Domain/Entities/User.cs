namespace UserService.Domain.Entities;

using ValueObjects;

public class User
{
    public UserId Id { get; private set; }
    public string Login { get; private set; }
    public string PasswordHash { get; private set; }
    public string FullName { get; private set; }

    private User() { } // для EF Core

    public User(UserId id, string login, string passwordHash, string fullName)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
        FullName = fullName;
    }
}