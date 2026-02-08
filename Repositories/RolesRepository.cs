using Dapper;
using InvenSmartApi.Models.Roles;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvenSmartApi.Repositories;

public sealed class RolesRepository : IRolesRepository
{
    private readonly IDbConnection _db;
    public RolesRepository(IDbConnection db) => _db = db;

    public Task<IEnumerable<RoleDto>> ListAsync() =>
        _db.QueryAsync<RoleDto>("spRoles_List", commandType: CommandType.StoredProcedure);

    public async Task<int> CreateAsync(string name)
    {
        var id = await _db.QuerySingleAsync<int>(
            "spRoles_Create",
            new { Name = name },
            commandType: CommandType.StoredProcedure);

        return id;
    }

    public async Task<int> UpdateAsync(int id, string name, bool isActive)
    {
        var updated = await _db.QuerySingleAsync<int>(
            "spRoles_Update",
            new { Id = id, Name = name, IsActive = isActive },
            commandType: CommandType.StoredProcedure);

        return updated;
    }

    public Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId) =>
        _db.QueryAsync<RoleDto>(
            "spUserRoles_GetByUser",
            new { UsuarioId = userId },
            commandType: CommandType.StoredProcedure);

    public async Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds)
    {
        var tvp = new DataTable();
        tvp.Columns.Add("Id", typeof(int));
        foreach (var id in roleIds.Distinct()) tvp.Rows.Add(id);

        var p = new DynamicParameters();
        p.Add("@UsuarioId", userId);
        p.Add("@RoleIds", tvp.AsTableValuedParameter("dbo.IntList"));

        await _db.ExecuteAsync("spUserRoles_Set", p, commandType: CommandType.StoredProcedure);
    }

    public Task<IEnumerable<PermissionDto>> ListPermissionsAsync() =>
        _db.QueryAsync<PermissionDto>("spPermisos_List", commandType: CommandType.StoredProcedure);

    public Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId) =>
        _db.QueryAsync<PermissionDto>(
            "spRolePermisos_Get",
            new { RoleId = roleId },
            commandType: CommandType.StoredProcedure);

    public async Task SetRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds)
    {
        var tvp = new DataTable();
        tvp.Columns.Add("Id", typeof(int));
        foreach (var id in permissionIds.Distinct()) tvp.Rows.Add(id);

        var p = new DynamicParameters();
        p.Add("@RoleId", roleId);
        p.Add("@PermisoIds", tvp.AsTableValuedParameter("dbo.IntList"));

        await _db.ExecuteAsync("spRolePermisos_Set", p, commandType: CommandType.StoredProcedure);
    }
}
