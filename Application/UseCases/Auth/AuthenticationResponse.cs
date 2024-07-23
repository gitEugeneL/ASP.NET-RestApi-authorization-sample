using Application.Common.Models;

namespace Application.UseCases.Auth;

public record AuthenticationResponse(
    JwtToken JwtToken,
    CookieToken CookieToken
);