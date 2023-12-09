namespace sample.Models;

public class RefreshToken
{
    public required string Token { get; set; }
    public DateTime Created { get; } = DateTime.Now;
    public DateTime Expires { get; set; }
}