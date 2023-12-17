using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Operations.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, AuthenticationResponse>
{
    private readonly ITokenManager _tokenManager;
    private readonly IUserRepository _userRepository;

    public RefreshCommandHandler(ITokenManager tokenManager, IUserRepository userRepository)
    {
        _tokenManager = tokenManager;
        _userRepository = userRepository;
    }
    
    public async Task<AuthenticationResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        // find user by token
        var user = await _userRepository.FindUserByRefreshTokenAsync(request.RefreshToken, cancellationToken)
                   ?? throw new UnauthorizedException("Refresh token isn't valid");
        
        // find this token in the user
        var userRefreshToken = user.RefreshTokens
            .First(rt => rt.Token == request.RefreshToken);
        
        // check refresh token expiration time
        if (userRefreshToken.Expires < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token is outdated"); 
        
        // remove old refresh token
        user.RefreshTokens.Remove(userRefreshToken);
        
        var accessToken = _tokenManager.GenerateAccessToken(user);
        var refreshToken = _tokenManager.GenerateRefreshToken(user);
        
        user.RefreshTokens.Add(refreshToken);
        await _userRepository.UpdateUserAsync(user, cancellationToken);
        
        return new AuthenticationResponse(
            new JwtToken(accessToken),
            new CookieToken(refreshToken.Token, refreshToken.Expires)
        );
    }
}
