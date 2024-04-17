using InvenSmartApi.Models;
using InvenSmartApi.Repositories;
using System;
using System.Threading.Tasks;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario> GetUsuarioAsync(Credenciales credenciales)
    {
        var usuario = await _usuarioRepository.GetUsuarioAsync(credenciales);
        if (usuario != null)
        {
            return usuario;
        }
        else
        {
            throw new NotFoundException("Usuario no encontrado.");
        }
    }
}
