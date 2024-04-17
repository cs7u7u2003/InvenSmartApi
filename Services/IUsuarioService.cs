using InvenSmartApi.Models;

public interface IUsuarioService
{
    Task<Usuario> GetUsuarioAsync(Credenciales credenciales);
}
