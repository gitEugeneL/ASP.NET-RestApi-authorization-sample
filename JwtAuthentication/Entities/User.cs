namespace JwtAuthentication.Entities;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
  
    public Role Role { get; set; }
    public required int RoleId { get; set; }

    public required byte[] PwdHash { get; set; }
    public required byte[] PwdSalt { get; set; }
    
    public string RefreshToken { get; set; } = string.Empty;  
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}