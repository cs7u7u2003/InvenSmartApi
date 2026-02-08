namespace InvenSmartApi.Models.Users;

public sealed class CreateUserRequest
{
    public string UserId { get; set; } = "";
    public string Password { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string? Cedula { get; set; }
    public string? Comment { get; set; }
}
