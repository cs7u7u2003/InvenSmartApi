using InvenSmartApi.Models;

namespace InvenSmartApi.Repositories;

public interface IUsuarioRepository
{
    Task<UsuarioDto?> GetByUserIdAsync(string userId);
    Task<int> InsertAsync(UsuarioDto usuario); // devuelve newId
}
