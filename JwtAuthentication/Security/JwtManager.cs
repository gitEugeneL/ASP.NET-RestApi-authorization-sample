using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using JwtAuthentication.Entities;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthentication.Security;

public class JwtManager
{
    private readonly DateTime _tokenExpires = DateTime.Now.AddMinutes(10);
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
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var settingsToken = _configuration.GetSection("Authentication:Key").Value;
        if (settingsToken is null)
            throw new Exception("[!] AppSettings Token is null [!]");
        
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(settingsToken));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _tokenExpires,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }
}