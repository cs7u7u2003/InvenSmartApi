using InvenSmartApi.Models.Users;

namespace InvenSmartApi.Services;

public interface IUsuarioService
{
    Task<UserResponse?> GetByUserIdAsync(string userId);
    Task<(bool ok, string? error, UserResponse? user)> CreateAsync(CreateUserRequest req);
}
