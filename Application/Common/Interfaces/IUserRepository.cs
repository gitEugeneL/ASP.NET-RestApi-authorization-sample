using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);

    Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken);
}
