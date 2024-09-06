using InvenSmartApi.Models;

namespace InvenSmartApi.Repositories;

public interface IPermisoRepository
{
    Task<IEnumerable<PermisoFormularioDto>> GetPermisosByUsuarioAsync(int userId);
}