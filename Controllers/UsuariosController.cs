using InvenSmartApi.Infrastructure.Security.Permissions;
using InvenSmartApi.Models.Users;
using InvenSmartApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service) => _service = service;

    [RequirePermission("SCREEN.USERS.VIEW")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var user = await _service.GetByUserIdAsync(userId);
        if (user is null) return NotFound();

        return Ok(user);
    }

    [RequirePermission("USER.CREATE")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        var (ok, error, user) = await _service.CreateAsync(req);
        if (!ok) return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetByUserId), new { userId = user!.UserId }, user);
    }
}
