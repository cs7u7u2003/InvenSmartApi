public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string UserId { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Cedula { get; set; }
    public int PermissionId { get; set; }
    public string Comment { get; set; }
}
