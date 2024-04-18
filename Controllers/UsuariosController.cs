using Microsoft.AspNetCore.Mvc;
using InvenSmartApi.Models;

namespace InvenSmartApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsuarioAsync([FromQuery] Credenciales credenciales)
    {
        try
        {
            var usuario = await _usuarioService.GetUsuarioAsync(credenciales);
            return Ok(usuario);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiError(404, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiError(500, ex.Message));
        }
    }
}
