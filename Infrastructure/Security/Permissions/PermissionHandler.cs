using Microsoft.AspNetCore.Authorization;

namespace InvenSmartApi.Infrastructure.Security.Permissions;

public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var has = context.User.Claims
            .Where(c => c.Type == "perm")
            .Any(c => string.Equals(c.Value, requirement.Permission, StringComparison.OrdinalIgnoreCase));

        if (has) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
