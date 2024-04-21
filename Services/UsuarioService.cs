using InvenSmartApi.Models;
using InvenSmartApi.Repositories;
using Microsoft.AspNetCore.Mvc;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IActionResult> GetUsuarioAsync(Credenciales credenciales)
    {
        var usuario = await _usuarioRepository.GetUsuarioAsync(credenciales);
        if (usuario != null)
        {
            if (PasswordHasher.VerifyPasswordHash(credenciales.Password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                return new OkObjectResult(usuario);
            }
            return new NotFoundObjectResult(new { message = "El password no coincide." });
        }
        else
        {
            return new NotFoundObjectResult(new { message = "Usuario no encontrado." });
        }
    }
    public async Task<bool> InsertarUsuarioAsync(UsuarioQuery usuario)
    {
        if (usuario == null)
        {
            throw new ArgumentNullException(nameof(usuario));
        }

        if (string.IsNullOrEmpty(usuario.Nombre) || string.IsNullOrEmpty(usuario.Apellido) ||
            string.IsNullOrEmpty(usuario.UserId) || string.IsNullOrEmpty(usuario.Password.ToString()) || string.IsNullOrEmpty(usuario.Cedula))
        {
            throw new ArgumentException("Todos los campos son requeridos.");
        }
       UsuarioDto createUsuario = new UsuarioDto();

        createUsuario.Nombre = usuario.Nombre;
        createUsuario.Apellido = usuario.Apellido;
        createUsuario.Cedula= usuario.Cedula;
        createUsuario.UserId = usuario.UserId;
        (createUsuario.PasswordHash, createUsuario.PasswordSalt) = PasswordHasher.CreatePasswordHash(usuario.Password);

        return await _usuarioRepository.InsertarUsuarioAsync(createUsuario);
    }
}
