using System.ComponentModel.DataAnnotations;

public class Modelo
{
    public int ID_modelo { get; set; }

    [Required(ErrorMessage = "El nombre del modelo es obligatorio.")]
    public string Nombre_Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar una marca.")]
    public int id_Marca { get; set; }

    public string Marca { get; set; } = string.Empty;  

    public bool Activo { get; set; } = true;
}
