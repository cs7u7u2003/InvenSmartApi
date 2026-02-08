namespace InvenSmartApi.Models.Users;

public sealed class UserResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string? Cedula { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
}
