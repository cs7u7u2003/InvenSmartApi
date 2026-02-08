using InvenSmartApi.Models.Roles;

namespace InvenSmartApi.Repositories;

public interface IRolesRepository
{
    Task<IEnumerable<RoleDto>> ListAsync();
    Task<int> CreateAsync(string name);
    Task<int> UpdateAsync(int id, string name, bool isActive);

    Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);
    Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds);

    Task<IEnumerable<PermissionDto>> ListPermissionsAsync();
    Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId);
    Task SetRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds);
}
