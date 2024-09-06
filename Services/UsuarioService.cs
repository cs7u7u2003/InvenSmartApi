using InvenSmartApi.Models;
using InvenSmartApi.Repositories;
using InvenSmartApi.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPermisoRepository _permisoRepository;
    private readonly ErrorLogger _errorLogger;

    public UsuarioService(IUsuarioRepository usuarioRepository, IPermisoRepository permisoRepository, ErrorLogger errorLogger)
    {
        _usuarioRepository = usuarioRepository;
        _permisoRepository = permisoRepository;
        _errorLogger = errorLogger;
    }

    public async Task<IActionResult> GetUsuarioAsync(Credenciales credenciales)
    {
        try
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(credenciales);
            if (usuario != null)
            {
                if (PasswordHasher.VerifyPasswordHash(credenciales.Password, usuario.PasswordHash, usuario.PasswordSalt))
                {
                    var permisos = await _permisoRepository.GetPermisosByUsuarioAsync(usuario.Id);
                    var result = new
                    {
                        Usuario = usuario,
                        Permisos = permisos
                    };
                    return new OkObjectResult(result);
                }
                return new OkObjectResult(new { message = "El password no coincide." });
            }
            else
            {
                return new OkObjectResult(new { message = "Usuario no encontrado." });
            }
        }
        catch (Exception ex)
        {
            await _errorLogger.LogErrorAsync(ex.Message, ex.StackTrace, nameof(UsuarioService), nameof(GetUsuarioAsync));
            return new OkObjectResult(new { message = "Error interno del servidor." });
        }
    }

    public async Task<bool> InsertarUsuarioAsync(UsuarioQuery usuario)
    {
        try
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
            createUsuario.Cedula = usuario.Cedula;
            createUsuario.UserId = usuario.UserId;
            (createUsuario.PasswordHash, createUsuario.PasswordSalt) = PasswordHasher.CreatePasswordHash(usuario.Password);


            return await _usuarioRepository.InsertarUsuarioAsync(createUsuario);
        }
        catch (Exception ex)
        {
            await _errorLogger.LogErrorAsync(ex.Message, ex.StackTrace, nameof(UsuarioService), nameof(InsertarUsuarioAsync));
            return false;
        }
    }
}
