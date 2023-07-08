using JwtAuthentication.Entities;
using JwtAuthentication.Exceptions;
using JwtAuthentication.Models;
using JwtAuthentication.Models.Dto;
using JwtAuthentication.Repository;
using JwtAuthentication.Security;

namespace JwtAuthentication.Services;

public interface IAuthenticationService
{
    UserResponseDto Registration(UserRegistrationDto dto);
    Token Login(UserLoginDto dto);
    Token RefreshToken(string? requestRefreshToken);
    void Logout(string? requestRefreshToken);
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
    
    public Token Login(UserLoginDto dto)
    {
        var user = _authenticationRepository.FindUserByUsername(dto.Username);
        if (user is null || !_passwordHasher.VerifyPasswordHash(dto.Password, user.PwdHash, user.PwdSalt))
            throw new NotFoundException($"User '{dto.Username}' doesn't exist or your password is incorrect");
        
        var token = _jwtManager.CreateToken(user);
        var refreshToken = _jwtManager.GenerateRefreshToken(user);
        
        _authenticationRepository.UpdateUserRefreshToken(user, refreshToken);
        
        return new Token { JwtToken = token, RefreshToken = refreshToken };
    }

    public Token RefreshToken(string? requestRefreshToken)
    {
        var validUser = ValidateRefreshToken(requestRefreshToken);   
        var token = _jwtManager.CreateToken(validUser);
        var refreshToken = _jwtManager.GenerateRefreshToken(validUser);
        _authenticationRepository.UpdateUserRefreshToken(validUser, refreshToken);
        return new Token { JwtToken = token, RefreshToken = refreshToken };
    }

    public void Logout(string? requestRefreshToken)
    {
        var validUser = ValidateRefreshToken(requestRefreshToken);
        _authenticationRepository.DeleteRefreshToken(validUser);
    }


    private User ValidateRefreshToken(string? requestRefreshToken)
    {
        if (requestRefreshToken is null)
            throw new UnauthorizedException("Refresh token doesn't exist");
        
        int id = int.TryParse(requestRefreshToken[(requestRefreshToken.LastIndexOf('W') + 1)..], out int result) 
            ? result : -1;
        
        if (id is -1)
            throw new UnauthorizedException("This user doesn't exist");
        
        var user = _authenticationRepository.FindUserById(id);
        
        if (user is null)
            throw new UnauthorizedException("This user doesn't exist");
        if (!user.RefreshToken.Equals(requestRefreshToken))
            throw new UnauthorizedException("Refresh token isn't valid");
        if (user.TokenExpires < DateTime.Now)
            throw new UnauthorizedException("Refresh token is outdated");
        
        return user;
    }
}