using System.Data.SqlClient;
using System.Data;
using InvenSmartApi.Models;
using Dapper;

namespace InvenSmartApi.Repositories;

public class PermisoRepository : IPermisoRepository
{
    private readonly IDbConnection _dbConnection;

    public PermisoRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<PermisoFormularioDto>> GetPermisosByUsuarioAsync(int userId)
    {

        var parameters = new { UserId = userId };
        var permisos = await _dbConnection.QueryAsync<PermisoFormularioDto>(
            "spGetPermisosByUsuario", parameters, commandType: CommandType.StoredProcedure);
        return permisos;

    }
}