using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using InvenSmartApi.Models;

namespace InvenSmartApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IConfiguration _config;

        public UsuarioRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<Usuario> GetUsuarioAsync(Credenciales credenciales)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@Usuario", credenciales.Usuario);
            parameters.Add("@Password", credenciales.Password);

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>("sp_ConsultarUsuario",
                                                                            parameters,
                                                                            commandType: CommandType.StoredProcedure);

            return usuario ?? throw new NotFoundException("Usuario no encontrado.");
        }
    }
}
