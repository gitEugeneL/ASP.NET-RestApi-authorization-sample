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

    public Task<User?> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dataContext.Users
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }
}
