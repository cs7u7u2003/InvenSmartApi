using System.Security.Cryptography;
using System.Text;

public class LegacyPasswordHasher
{
    // Generar un hash para el password y una sal única
    public static (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHash(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

        byte[] passwordSalt;
        byte[] passwordHash;
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        return (passwordHash, passwordSalt);
    }

    // Verificar si el password proporcionado coincide con el hash almacenado
    public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("El password no puede estar vacío o contener solo espacios en blanco.", nameof(password));
        if (storedHash.Length != 64) throw new ArgumentException("Longitud del hash de password inválida (64 bytes esperados).", nameof(storedHash));
        if (storedSalt.Length != 128) throw new ArgumentException("Longitud de la sal de password inválida (128 bytes esperados).", nameof(storedSalt));

        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }
        }
        return true;
    }
}
