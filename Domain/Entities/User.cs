using Domain.Common;

namespace Domain.Entities;

public sealed class User : BaseAuditableEntity
{
    public required string Email { get; init; }
    public required byte[] PasswordHash { get; init; }
    public required byte[] PasswordSalt { get; init; }
    
    /**** Relations ****/
    public required Role Role { get; init; }
    public Guid RoleId { get; init; }

    public List<RefreshToken> RefreshTokens { get; init; } = new();
}
