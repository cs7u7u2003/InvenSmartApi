using InvenSmartApi.Models;
using InvenSmartApi.Repositories;

namespace InvenSmartApi.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario> GetUsuarioAsync(Credenciales credenciales)
    {
        return await _usuarioRepository.GetUsuarioAsync(credenciales);
    }
}
