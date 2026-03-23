using Dapper;
using InvenSmartApi.Infrastructure.Security;
using Microsoft.Data.SqlClient;

namespace InvenSmartApi.Infrastructure.Database;

public static class DbSeedAdmin
{
    public static async Task EnsureAdminAsync(string connStr, CancellationToken ct)
    {
        const string userId = "admin";
        const string password = "P@ssw0rd!";

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync(ct);

        var adminRoleId = await conn.ExecuteScalarAsync<int?>(
            "SELECT TOP 1 Id FROM dbo.Roles WHERE Name = 'Admin'");

        if (adminRoleId is null)
            throw new InvalidOperationException("Admin role was not created by DatabaseScripts.");

        var existingUser = await conn.QueryFirstOrDefaultAsync<int?>(
            "SELECT TOP 1 Id FROM dbo.Usuarios WHERE UserId = @UserId",
            new { UserId = userId });

        var (hash, salt) = PasswordHasher.CreateHash(password);

        int userIdValue;
        if (existingUser is null)
        {
            userIdValue = await conn.ExecuteScalarAsync<int>(@"
INSERT INTO dbo.Usuarios (Nombre, Apellido, UserId, PasswordHash, PasswordSalt, Cedula, PermissionId, Comment, IsActive)
VALUES (@Nombre, @Apellido, @UserId, @PasswordHash, @PasswordSalt, NULL, NULL, 'Seed admin', 1);
SELECT CAST(SCOPE_IDENTITY() AS INT);",
                new
                {
                    Nombre = "Admin",
                    Apellido = "System",
                    UserId = userId,
                    PasswordHash = hash,
                    PasswordSalt = salt
                });
        }
        else
        {
            userIdValue = existingUser.Value;
            await conn.ExecuteAsync(@"
UPDATE dbo.Usuarios
SET PasswordHash = @PasswordHash,
    PasswordSalt = @PasswordSalt,
    IsActive = 1
WHERE Id = @Id;",
                new { Id = userIdValue, PasswordHash = hash, PasswordSalt = salt });
        }

        await conn.ExecuteAsync(@"
IF NOT EXISTS (SELECT 1 FROM dbo.UsuarioRoles WHERE UsuarioId = @UsuarioId AND RoleId = @RoleId)
BEGIN
    INSERT INTO dbo.UsuarioRoles(UsuarioId, RoleId) VALUES (@UsuarioId, @RoleId);
END",
            new { UsuarioId = userIdValue, RoleId = adminRoleId.Value });

        await conn.ExecuteAsync(@"
INSERT INTO dbo.RolPermisos(RoleId, PermisoId)
SELECT @RoleId, p.Id
FROM dbo.Permisos p
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RolPermisos rp WHERE rp.RoleId = @RoleId AND rp.PermisoId = p.Id
);",
            new { RoleId = adminRoleId.Value });
    }
}
