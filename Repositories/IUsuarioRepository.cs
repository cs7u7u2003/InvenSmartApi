using InvenSmartApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvenSmartApi.Repositories;

public interface IUsuarioRepository
{
    Task<UsuarioDto> GetUsuarioAsync(Credenciales credenciales);
    Task<bool> InsertarUsuarioAsync(UsuarioDto usuario);
}