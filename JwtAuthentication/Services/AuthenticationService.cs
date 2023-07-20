using JwtAuthentication.Entities;
using JwtAuthentication.Exceptions;
using JwtAuthentication.Models;
using JwtAuthentication.Models.Dto;
using JwtAuthentication.Repository;
using JwtAuthentication.Security;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthentication.Services;

public interface IAuthenticationService
{
    Task<UserResponseDto> Registration(UserRegistrationDto dto);
    Task<Token> Login(UserLoginDto dto);
    Task<Token> RefreshToken(string? requestRefreshToken);
    Task Logout(string? requestRefreshToken);
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

    public async Task<UserResponseDto> Registration(UserRegistrationDto dto)
    {
        if (await _authenticationRepository.IsUserExist(dto.Username))
            throw new AlreadyExistException($"User: {dto.Username} already exist");

        _passwordHasher.CreatePasswordHash(dto.Password, out byte[] pswHash, out byte[] pswSalt);

        var createdUser = await _authenticationRepository.CreateUser(
            new User { Username = dto.Username, RoleId = 1, PwdHash = pswHash, PwdSalt = pswSalt }
        );
        return new UserResponseDto { Id = createdUser.Id, Username = createdUser.Username };
    }
    
    public async Task<Token> Login(UserLoginDto dto)
    {
        var user = await _authenticationRepository.FindUserByUsername(dto.Username);
        if (user is null || !_passwordHasher.VerifyPasswordHash(dto.Password, user.PwdHash, user.PwdSalt))
            throw new NotFoundException($"User '{dto.Username}' doesn't exist or your password is incorrect");
        
        var token = _jwtManager.CreateToken(user);
        var refreshToken = _jwtManager.GenerateRefreshToken(user);
        
        await _authenticationRepository.UpdateUserRefreshToken(user, refreshToken);
        
        return new Token { JwtToken = token, RefreshToken = refreshToken };
    }
    
    public async Task<Token> RefreshToken(string? requestRefreshToken)
    {
        var validUser = await ValidateRefreshToken(requestRefreshToken);   
        
        Console.WriteLine(validUser.Role);
        
        var token = _jwtManager.CreateToken(validUser);
        var refreshToken = _jwtManager.GenerateRefreshToken(validUser);
        await _authenticationRepository.UpdateUserRefreshToken(validUser, refreshToken);
        return new Token { JwtToken = token, RefreshToken = refreshToken };
    }

    [Authorize]
    public async Task Logout(string? requestRefreshToken)
    {
        var validUser = await ValidateRefreshToken(requestRefreshToken);
        await _authenticationRepository.DeleteRefreshToken(validUser);
    }
    
    private async Task<User> ValidateRefreshToken(string? requestRefreshToken)
    {
        if (requestRefreshToken is null)
            throw new UnauthorizedException("Refresh token doesn't exist");
        
        int id = int.TryParse(requestRefreshToken[(requestRefreshToken.LastIndexOf('W') + 1)..], out int result) 
            ? result : -1;
        
        if (id is -1)
            throw new UnauthorizedException("This user doesn't exist");
        
        var user = await _authenticationRepository.FindUserById(id);
        
        if (user is null)
            throw new UnauthorizedException("This user doesn't exist");
        if (!user.RefreshToken.Equals(requestRefreshToken))
            throw new UnauthorizedException("Refresh token isn't valid");
        if (user.TokenExpires < DateTime.Now)
            throw new UnauthorizedException("Refresh token is outdated");
        
        return user;
    }
}