using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Minosa_Proyecto_v._1.Models
{
    public class Usuario
    {
        public int ID_usuario { get; set; }
        public required string Nombre_Usuario { get; set; }
        public required string Contrasena { get; set; }
        public int id_rol { get; set; }
    }
   
}
