using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext _dataContext;
    
    public UserRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
    
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dataContext.Users
            .AddAsync(user, cancellationToken);
        await _dataContext
            .SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dataContext.Users
            .Include(user => user.Role)
            .Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<User?> FindUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await _dataContext.Users
            .Include(user => user.Role)
            .Include(user => user.RefreshTokens)
            .FirstOrDefaultAsync(user => user.RefreshTokens
                    .Any(rt => rt.Token == refreshToken), cancellationToken);
    }
    
    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        _dataContext.Users.Update(user);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }
}
