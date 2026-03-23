using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("auth")]
public sealed class MeController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();
        var perms = User.FindAll("perm").Select(c => c.Value).Distinct().ToArray();

        return Ok(new
        {
            userId,
            userName,
            roles,
            perms
        });
    }
}
