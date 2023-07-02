using JwtAuthentication.Data;
using JwtAuthentication.Entities;

namespace JwtAuthentication.Repository;

public interface IAuthenticationRepository
{
    User? FindUserByUsername(string username);
    bool IsUserExist(string username);
    User CreateUser(User user);
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
        return _dbContext.Users.FirstOrDefault(user => user.Username == username);
    }

    public bool IsUserExist(string username)
    {
        return _dbContext.Users.Any(user => user.Username == username);
    }
}