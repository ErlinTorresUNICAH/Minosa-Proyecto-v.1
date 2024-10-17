using System.ComponentModel.DataAnnotations;

public class Zona
{
    public int ID_zona { get; set; }

    [Required(ErrorMessage = "El nombre de la zona es obligatorio.")]
    public string Nombre_Zona { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion_Zona { get; set; }

    public bool Activo { get; set; }

    public DateTime Creacion_Zona { get; set; }
}
