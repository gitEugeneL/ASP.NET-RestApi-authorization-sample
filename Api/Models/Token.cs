namespace sample.Models;

public class Token
{
    public required string JwtToken { get; set; }
    public required RefreshToken RefreshToken { get; set; }
}