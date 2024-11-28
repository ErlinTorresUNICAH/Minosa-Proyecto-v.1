using System.ComponentModel.DataAnnotations;

//public class Usuario
//{
//    public int ID_usuario { get; set; }
//    public string Nombre_Usuario { get; set; }
//    public string Contrasena { get; set; }
//    public int id_rol { get; set; }
//    public string Nombre_Rol { get; set; }
//}

public class Usuario
{
    public int ID_usuario { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
    public string Nombre_Usuario { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(255, ErrorMessage = "La contraseña no puede exceder los 255 caracteres.")]
    public string Contrasena { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    public int id_rol { get; set; }

    public string Nombre_Rol { get; set; } // Para mostrar en las vistas
}
