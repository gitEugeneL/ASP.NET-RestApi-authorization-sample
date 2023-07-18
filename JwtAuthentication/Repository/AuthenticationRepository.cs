using JwtAuthentication.Data;
using JwtAuthentication.Entities;
using JwtAuthentication.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Repository;

public interface IAuthenticationRepository
{
    Task<User?> FindUserByUsername(string username);
    User? FindUserById(int id);
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
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
    }

    public User? FindUserById(int id)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == id);
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