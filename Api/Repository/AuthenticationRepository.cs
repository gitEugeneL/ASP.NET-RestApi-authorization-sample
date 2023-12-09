using Microsoft.EntityFrameworkCore;
using sample.Data;
using sample.Entities;
using sample.Models;

namespace sample.Repository;

public interface IAuthenticationRepository
{
    Task<User?> FindUserByUsername(string username);
    Task<User?> FindUserById(int id);
    Task<bool> IsUserExist(string username);
    Task<User> CreateUser(User user);
    Task UpdateUserRefreshToken(User user, RefreshToken refreshToken);
    Task DeleteRefreshToken(User user);
}


public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public AuthenticationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUser(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> FindUserByUsername(string username)
    {
        return await _dbContext
            .Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> FindUserById(int id)
    {
        return await _dbContext
            .Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<bool> IsUserExist(string username)
    {
        return await _dbContext.Users.AnyAsync(user => user.Username == username);
    }
    
    public async Task UpdateUserRefreshToken(User user, RefreshToken? refreshToken = null)
    {
        user.RefreshToken = refreshToken?.Token ?? string.Empty;
        user.TokenCreated = refreshToken?.Created ?? default;
        user.TokenExpires = refreshToken?.Expires ?? default;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRefreshToken(User user)
    {
        await UpdateUserRefreshToken(user);
    }
}