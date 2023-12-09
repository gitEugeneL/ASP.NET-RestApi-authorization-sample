using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using sample.Entities;
using sample.Models;

namespace sample.Security;

public class JwtManager
{
    private readonly IConfiguration _configuration;
    
    public JwtManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Value)
        };

        var settingsToken = _configuration.GetSection("Authentication:Key").Value;
        if (settingsToken is null)
            throw new Exception("[!] AppSettings Token is null [!]");
        
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(settingsToken));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(
                int.Parse(_configuration.GetSection("Authentication:TokenLifetimeMin").Value!)),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(256)) + "W" + user.Id,
            Expires = DateTime.Now.AddDays(
                int.Parse(_configuration.GetSection("Authentication:RefreshTokenLifetimeDays").Value!))
        };
    }
    
    public static void SetCookies(HttpResponse response, RefreshToken refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshToken.Expires,
        };
        response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }
}
