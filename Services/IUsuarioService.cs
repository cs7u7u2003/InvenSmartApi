using InvenSmartApi.Models;
namespace InvenSmartApi.Services;
public interface IUsuarioService
{
    Task<Usuario> GetUsuarioAsync(Credenciales credenciales);
}
