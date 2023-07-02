using JwtAuthentication.Entities;
using JwtAuthentication.Exceptions;
using JwtAuthentication.Models;
using JwtAuthentication.Repository;
using JwtAuthentication.Security;

namespace JwtAuthentication.Services;

public interface IAuthenticationService
{
    UserResponseDto Registration(UserRegistrationDto dto);
}


public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly PasswordHasher _passwordHasher;

    public AuthenticationService(IAuthenticationRepository authenticationRepository, PasswordHasher passwordHasher)
    {
        _authenticationRepository = authenticationRepository;
        _passwordHasher = passwordHasher;
    }
    
    public UserResponseDto Registration(UserRegistrationDto dto)
    {
        if (_authenticationRepository.IsUserExist(dto.Username))
            throw new AlreadyExistException($"User: {dto.Username} already exist");

        _passwordHasher.CreatePasswordHash(dto.Password, out byte[] pswHash, out byte[] pswSalt);
        
        var createdUser = _authenticationRepository.CreateUser(
        new User { Username = dto.Username, PwdHash = pswHash, PwdSalt = pswSalt }
        );
        return new UserResponseDto { Id = createdUser.Id, Username = createdUser.Username };
    }
}