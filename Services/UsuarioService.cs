using InvenSmartApi.Infrastructure.Security;
using InvenSmartApi.Models;
using InvenSmartApi.Models.Users;
using InvenSmartApi.Repositories;

namespace InvenSmartApi.Services;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo) => _repo = repo;

    public async Task<UserResponse?> GetByUserIdAsync(string userId)
    {
        var u = await _repo.GetByUserIdAsync(userId);
        if (u is null) return null;

        return new UserResponse
        {
            Id = u.Id,
            UserId = u.UserId,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Cedula = u.Cedula,
            Comment = u.Comment,
            IsActive = true // si tu SP devuelve IsActive, mapéalo aquí
        };
    }

    public async Task<(bool ok, string? error, UserResponse? user)> CreateAsync(CreateUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UserId))
            return (false, "UserId es requerido.", null);

        if (string.IsNullOrWhiteSpace(req.Password) || req.Password.Length < 6)
            return (false, "Password inválido (mínimo 6 caracteres).", null);

        if (string.IsNullOrWhiteSpace(req.Nombre))
            return (false, "Nombre es requerido.", null);

        if (string.IsNullOrWhiteSpace(req.Apellido))
            return (false, "Apellido es requerido.", null);

        var existing = await _repo.GetByUserIdAsync(req.UserId.Trim());
        if (existing is not null)
            return (false, "Ya existe un usuario con ese UserId.", null);

        var (hash, salt) = PasswordHasher.CreateHash(req.Password);

        var dto = new UsuarioDto
        {
            Nombre = req.Nombre.Trim(),
            Apellido = req.Apellido.Trim(),
            UserId = req.UserId.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Cedula = req.Cedula?.Trim(),
            Comment = req.Comment?.Trim(),
            PermissionId = null
        };

        var newId = await _repo.InsertAsync(dto);

        var created = new UserResponse
        {
            Id = newId,
            UserId = dto.UserId,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Cedula = dto.Cedula,
            Comment = dto.Comment,
            IsActive = true
        };

        return (true, null, created);
    }
}
