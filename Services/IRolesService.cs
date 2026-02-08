using InvenSmartApi.Models.Roles;

namespace InvenSmartApi.Services;

public interface IRolesService
{
    Task<IEnumerable<RoleDto>> ListRolesAsync();
    Task<int> CreateRoleAsync(string name);
    Task<int> UpdateRoleAsync(int id, string name, bool isActive);

    Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId);
    Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds);

    Task<IEnumerable<PermissionDto>> ListPermissionsAsync();
    Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId);
    Task SetRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds);
}
