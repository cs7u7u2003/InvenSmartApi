using Dapper;
using Microsoft.Data.SqlClient;
using InvenSmartApi.Infrastructure.Security;
namespace InvenSmartApi.Infrastructure.Database;

public static class DbSeedAdmin
{
    public static async Task EnsureAdminAsync(string connStr, CancellationToken ct)
    {
        const string userId = "admin";
        const string password = "P@ssw0rd!";

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(ct);

        var exists = await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM dbo.Usuarios WHERE UserId = @UserId",
            new { UserId = userId });

        var (hash, salt) = InvenSmartApi.Infrastructure.Security.PasswordHasher.CreateHash(password);


        if (exists == 0)
        {
            await conn.ExecuteAsync(@"
INSERT INTO dbo.Usuarios (Nombre, Apellido, UserId, PasswordHash, PasswordSalt, Cedula, PermissionId, Comment, IsActive)
VALUES (@Nombre, @Apellido, @UserId, @PasswordHash, @PasswordSalt, NULL, NULL, 'Seed admin', 1);",
                new
                {
                    Nombre = "Admin",
                    Apellido = "System",
                    UserId = userId,
                    PasswordHash = hash,
                    PasswordSalt = salt
                });

            return;
        }

        // Si ya existe, resetea password (para no perder tiempo en DEV)
        await conn.ExecuteAsync(@"
UPDATE dbo.Usuarios
SET PasswordHash = @PasswordHash,
    PasswordSalt = @PasswordSalt,
    IsActive = 1
WHERE UserId = @UserId;",
            new { PasswordHash = hash, PasswordSalt = salt, UserId = userId });
    }
}
