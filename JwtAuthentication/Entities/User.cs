using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public Role Role { get; set; }

    public byte[] PwdHash { get; set; } = Array.Empty<byte>();
    public byte[] PwdSalt { get; set; } = Array.Empty<byte>();
}

public enum Role { Admin, Manager, User }

