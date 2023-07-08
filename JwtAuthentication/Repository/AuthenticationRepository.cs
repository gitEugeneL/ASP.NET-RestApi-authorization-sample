using JwtAuthentication.Data;
using JwtAuthentication.Entities;
using JwtAuthentication.Models;

namespace JwtAuthentication.Repository;

public interface IAuthenticationRepository
{
    User? FindUserByUsername(string username);
    User? FindUserById(int id);
    bool IsUserExist(string username);
    User CreateUser(User user);
    void UpdateUserRefreshToken(User user, RefreshToken refreshToken);
    void DeleteRefreshToken(User user);
}


public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public AuthenticationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public User CreateUser(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return user;
    }

    public User? FindUserByUsername(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username.ToLower() == username.ToLower());
    }

    public User? FindUserById(int id)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == id);
    }

    public bool IsUserExist(string username)
    {
        return _dbContext.Users.Any(user => user.Username == username);
    }
    
    public void UpdateUserRefreshToken(User user, RefreshToken? refreshToken = null)
    {
        user.RefreshToken = refreshToken?.Token ?? string.Empty;
        user.TokenCreated = refreshToken?.Created ?? default;
        user.TokenExpires = refreshToken?.Expires ?? default;
        _dbContext.SaveChanges();
    }

    public void DeleteRefreshToken(User user)
    {
        UpdateUserRefreshToken(user);
    }
}