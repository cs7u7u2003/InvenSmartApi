using InvenSmartApi.Infrastructure.Security.Permissions;
using InvenSmartApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize]
public sealed class PermissionsController : ControllerBase
{
    private readonly IRolesService _svc;
    public PermissionsController(IRolesService svc) => _svc = svc;

    [RequirePermission("SCREEN.ROLES.VIEW")]
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _svc.ListPermissionsAsync());
}
