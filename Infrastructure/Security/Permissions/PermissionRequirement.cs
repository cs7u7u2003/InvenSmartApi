using Microsoft.AspNetCore.Authorization;

namespace InvenSmartApi.Infrastructure.Security.Permissions;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}
