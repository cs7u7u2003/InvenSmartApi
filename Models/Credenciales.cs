namespace InvenSmartApi.Models;

public class Credenciales
{
    public string Usuario { get; set; }
    public string Password { get; set; }

    // Constructor sin parámetros requerido por el enlace del modelo
    public Credenciales()
    {
    }

    public Credenciales(string usuario, string password)
    {
        Usuario = usuario;
        Password = password;
    }
}