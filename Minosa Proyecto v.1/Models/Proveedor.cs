using System.ComponentModel.DataAnnotations;

public class Proveedor
{
    public int ID_proveedor { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria.")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono no es válido.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo no es válido.")]
    public string Correo { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
 //[Phone(ErrorMessage = "El teléfono no es válido.")]
 //   public string Telefono { get; set; } = string.Empty;

 //   [EmailAddress(ErrorMessage = "El correo no es válido.")]
 //   public string Correo { get; set; } = string.Empty;