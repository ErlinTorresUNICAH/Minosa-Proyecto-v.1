namespace Minosa_Proyecto_v._1.Models
{
    public class UsuarioViewModel
    {
        public int ID_usuario { get; set; }
        public string Nombre_Usuario { get; set; }
        public string Contrasena { get; set; }
        public string Nombre_Rol { get; set; }
        public string Descripcion { get; set; }
    }
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Supervisor = "Supervisor";
        public const string Usuario = "Usuario";
    }

}
