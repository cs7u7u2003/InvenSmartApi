using System.Data;
using System.Threading.Tasks;
using Dapper;
using InvenSmartApi.Models;
using Microsoft.Extensions.Configuration;

namespace InvenSmartApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbConnection _dbConnection;

        public UsuarioRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UsuarioDto> GetUsuarioAsync(Credenciales credenciales)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", credenciales.Usuario);

            return await _dbConnection.QueryFirstOrDefaultAsync<UsuarioDto>(
                "[dbo].[sp_GetUsuarioByUserId]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> InsertarUsuarioAsync(UsuarioDto usuario)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Nombre", usuario.Nombre);
            parameters.Add("@Apellido", usuario.Apellido);
            parameters.Add("@UserId", usuario.UserId);
            parameters.Add("@PasswordHash", usuario.PasswordHash);
            parameters.Add("@PasswordSalt", usuario.PasswordSalt);
            parameters.Add("@Cedula", usuario.Cedula);
            parameters.Add("@PermissionId", usuario.PermissionId);
            parameters.Add("@Comment", usuario.Comment);

            var result = await _dbConnection.ExecuteAsync(
                "[dbo].[sp_InsertarUsuario]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }
    }
}
