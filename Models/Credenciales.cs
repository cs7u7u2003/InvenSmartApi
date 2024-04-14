namespace InvenSmartApi.Models
{
    public class Credenciales
    {
        public string Usuario { get; }
        public string Password { get; }

        public Credenciales(string usuario, string password)
        {
            Usuario = usuario;
            Password = password;
        }
    }
}
