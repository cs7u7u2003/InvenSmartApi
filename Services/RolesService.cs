using InvenSmartApi.Models.Roles;
using InvenSmartApi.Repositories;

namespace InvenSmartApi.Services;

public sealed class RolesService : IRolesService
{
    private readonly IRolesRepository _repo;
    public RolesService(IRolesRepository repo) => _repo = repo;

    public Task<IEnumerable<RoleDto>> ListRolesAsync() => _repo.ListAsync();
    public Task<int> CreateRoleAsync(string name) => _repo.CreateAsync(name.Trim());
    public Task<int> UpdateRoleAsync(int id, string name, bool isActive) => _repo.UpdateAsync(id, name.Trim(), isActive);

    public Task<IEnumerable<RoleDto>> GetUserRolesAsync(int userId) => _repo.GetUserRolesAsync(userId);
    public Task SetUserRolesAsync(int userId, IEnumerable<int> roleIds) => _repo.SetUserRolesAsync(userId, roleIds);

    public Task<IEnumerable<PermissionDto>> ListPermissionsAsync() => _repo.ListPermissionsAsync();
    public Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(int roleId) => _repo.GetRolePermissionsAsync(roleId);
    public Task SetRolePermissionsAsync(int roleId, IEnumerable<int> permissionIds) => _repo.SetRolePermissionsAsync(roleId, permissionIds);
}
