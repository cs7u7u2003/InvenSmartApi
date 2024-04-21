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
            return await _usuarioService.GetUsuarioAsync(credenciales);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    [HttpPost]
    public async Task<IActionResult> InsertarUsuarioAsync([FromQuery] UsuarioQuery usuario)
    {
        try
        {
            var result = await _usuarioService.InsertarUsuarioAsync(usuario);
            if (result)
            {
                return Ok("Usuario creado correctamente.");
            }
            else
            {
                return BadRequest("No se pudo insertar el usuario.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}
