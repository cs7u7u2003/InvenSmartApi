using InvenSmartApi.Infrastructure.Security.Permissions;
using InvenSmartApi.Models.Roles;
using InvenSmartApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize]
public sealed class RolesController : ControllerBase
{
    private readonly IRolesService _svc;
    public RolesController(IRolesService svc) => _svc = svc;

    [RequirePermission("SCREEN.ROLES.VIEW")]
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await _svc.ListRolesAsync());

    [RequirePermission("ROLE.CREATE")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoleDto dto)
        => Ok(new { id = await _svc.CreateRoleAsync(dto.Name) });

    [RequirePermission("ROLE.EDIT")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RoleDto dto)
        => Ok(new { id = await _svc.UpdateRoleAsync(id, dto.Name, dto.IsActive) });

    [RequirePermission("SCREEN.USERS.VIEW")]
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserRoles(int userId)
        => Ok(await _svc.GetUserRolesAsync(userId));

    [RequirePermission("USER.EDIT")]
    [HttpPut("user/{userId:int}")]
    public async Task<IActionResult> SetUserRoles(int userId, [FromBody] SetIdsRequest req)
    {
        await _svc.SetUserRolesAsync(userId, req.Ids);
        return Ok();
    }

    [RequirePermission("SCREEN.ROLES.VIEW")]
    [HttpGet("{roleId:int}/permissions")]
    public async Task<IActionResult> GetRolePerms(int roleId)
        => Ok(await _svc.GetRolePermissionsAsync(roleId));

    [RequirePermission("ROLE.EDIT")]
    [HttpPut("{roleId:int}/permissions")]
    public async Task<IActionResult> SetRolePerms(int roleId, [FromBody] SetIdsRequest req)
    {
        await _svc.SetRolePermissionsAsync(roleId, req.Ids);
        return Ok();
    }
}
