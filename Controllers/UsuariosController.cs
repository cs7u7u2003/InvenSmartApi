using Microsoft.AspNetCore.Mvc;
using InvenSmartApi.Models;
using InvenSmartApi.Services;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    public async Task<IActionResult> GetUsuarioAsync(Credenciales credenciales)
    {
        var usuario = await _usuarioService.GetUsuarioAsync(credenciales);
        if (usuario == null)
        {
            return NotFound();
        }
        return Ok(usuario);
    }
}
