using System.Security.Cryptography;
using System.Text;

namespace InvenSmartApi.Infrastructure.Security;

public static class PasswordHasher
{
    public static (byte[] hash, byte[] salt) CreateHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required");

        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return (hash, salt);
    }

    public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (storedHash == null || storedHash.Length == 0) return false;
        if (storedSalt == null || storedSalt.Length == 0) return false;

        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
