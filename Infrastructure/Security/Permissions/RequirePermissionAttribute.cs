using Microsoft.AspNetCore.Authorization;

namespace InvenSmartApi.Infrastructure.Security.Permissions;

public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"{PermissionPolicyProvider.PolicyPrefix}{permission}";
    }
}
