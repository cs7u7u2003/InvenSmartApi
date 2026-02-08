using System.Data;
using Dapper;
using InvenSmartApi.Models;

namespace InvenSmartApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _db;

        public UsuarioRepository(IDbConnection db) => _db = db;

        public async Task<UsuarioDto?> GetByUserIdAsync(string userId)
        {
            return await _db.QueryFirstOrDefaultAsync<UsuarioDto>(
                "[dbo].[sp_GetUsuarioByUserId]",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(UsuarioDto usuario)
        {
            // si tu SP sp_InsertarUsuario devuelve NewId, lo leemos:
            var newId = await _db.ExecuteScalarAsync<int>(
                "[dbo].[sp_InsertarUsuario]",
                new
                {
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    UserId = usuario.UserId,
                    PasswordHash = usuario.PasswordHash,
                    PasswordSalt = usuario.PasswordSalt,
                    Cedula = usuario.Cedula,
                    PermissionId = (int?)null, // ya lo estamos migrando a RBAC
                    Comment = usuario.Comment
                },
                commandType: CommandType.StoredProcedure);

            return newId;
        }
    }
}
