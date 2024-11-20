using System.ComponentModel.DataAnnotations;

public class Material
{
    public int ID_Material { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "El área es obligatoria.")]
    public int id_area { get; set; }

    public string Nombre_Area { get; set; }
}
