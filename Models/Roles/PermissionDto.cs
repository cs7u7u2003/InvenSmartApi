namespace InvenSmartApi.Models.Roles;

public sealed class PermissionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
