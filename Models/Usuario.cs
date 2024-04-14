namespace InvenSmartApi.Models
{
    public class Usuario
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public required string UsuarioId { get; set; }
        public required string Password { get; set; }
        public string Cedula { get; set; }
        public int PermisoId { get; set; }
        public int Id { get; set; }
        public string Comentario { get; set; }
    }
}
