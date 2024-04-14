using InvenSmartApi.Models;

namespace InvenSmartApi.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario> GetUsuarioAsync(Credenciales credenciales);
}