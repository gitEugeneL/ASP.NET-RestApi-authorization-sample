using JwtAuthentication.Entities;
using JwtAuthentication.Exceptions;
using JwtAuthentication.Models;
using JwtAuthentication.Repository;
using JwtAuthentication.Security;

namespace JwtAuthentication.Services;

public interface IAuthenticationService
{
    UserResponseDto Registration(UserRegistrationDto dto);
    string Login(UserLoginDto dto);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly JwtManager _jwtManager;

    public AuthenticationService(IAuthenticationRepository authenticationRepository, PasswordHasher passwordHasher,
        JwtManager jwtManager)

    {
        _authenticationRepository = authenticationRepository;
        _passwordHasher = passwordHasher;
        _jwtManager = jwtManager;
    }

    public UserResponseDto Registration(UserRegistrationDto dto)
    {
        if (_authenticationRepository.IsUserExist(dto.Username))
            throw new AlreadyExistException($"User: {dto.Username} already exist");

        _passwordHasher.CreatePasswordHash(dto.Password, out byte[] pswHash, out byte[] pswSalt);

        var createdUser = _authenticationRepository.CreateUser(
            new User { Username = dto.Username, Role = Role.User, PwdHash = pswHash, PwdSalt = pswSalt }
        );
        return new UserResponseDto { Id = createdUser.Id, Username = createdUser.Username };
    }
    
    public string Login(UserLoginDto dto)
    {
        var user = _authenticationRepository.FindUserByUsername(dto.Username);
        if (user is null || !_passwordHasher.VerifyPasswordHash(dto.Password, user.PwdHash, user.PwdSalt))
            throw new NotFoundException($"User '{dto.Username}' doesn't exist or your password is incorrect");
        
        var token = _jwtManager.CreateToken(user);
        return token;
    }
}