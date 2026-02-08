namespace InvenSmartApi.Models.Auth;

public sealed class LoginResponse
{
    public string Token { get; set; } = "";
    public DateTime ExpiresAtUtc { get; set; }

    public int UserId { get; set; }
    public string UserName { get; set; } = "";

    public string[] Roles { get; set; } = Array.Empty<string>();
    public string[] Permissions { get; set; } = Array.Empty<string>();
}
