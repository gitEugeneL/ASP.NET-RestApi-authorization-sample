using System.ComponentModel.DataAnnotations;

namespace JwtAuthentication.Models;

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}