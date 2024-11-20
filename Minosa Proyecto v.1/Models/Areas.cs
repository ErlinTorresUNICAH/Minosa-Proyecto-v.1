using System.ComponentModel.DataAnnotations;

public class Area
{
    public int ID_area { get; set; }
    [Required(ErrorMessage = "El nombre del área es obligatorio.")]

    public string Nombre_Area { get; set; }
    [Required(ErrorMessage = "Debe seleccionar una zona.")]

    public int id_zona { get; set; }

    public string Nombre_Zona { get; set; }

    public bool Activo { get; set; } = true;
}
