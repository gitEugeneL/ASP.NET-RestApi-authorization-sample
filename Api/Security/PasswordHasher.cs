namespace sample.Security;

public class PasswordHasher
{
    public void CreatePasswordHash(string password, out byte[] pswHash, out byte[] pswSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        pswHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        pswSalt = hmac.Key;
    }

    public bool VerifyPasswordHash(string password, byte[] pswHash, byte[] pswSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(pswSalt);
        byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(pswHash);
    }
}