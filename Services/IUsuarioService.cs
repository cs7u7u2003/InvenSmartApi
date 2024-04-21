using InvenSmartApi.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUsuarioService
{
    Task<IActionResult> GetUsuarioAsync(Credenciales credenciales);
    Task<bool> InsertarUsuarioAsync(UsuarioQuery usuario);
}
