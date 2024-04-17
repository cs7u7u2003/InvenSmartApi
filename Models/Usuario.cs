public class Usuario
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string UsuarioId { get; set; }
    public string Password { get; set; }
    public string Cedula { get; set; }
    public int PermisoId { get; set; }
    public int Id { get; set; }
    public string Comentario { get; set; }

    public Usuario()
    {
        Nombre = string.Empty;
        Apellido = string.Empty;
        UsuarioId = string.Empty;
        Password = string.Empty;
        Cedula = string.Empty;
        Comentario = string.Empty;
    }

    public static Usuario CrearUsuarioVacio()
    {
        return new Usuario();
    }
}
