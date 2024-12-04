using System.ComponentModel.DataAnnotations;

public class Destinatario
{
    public int ID_destinatario { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre_Destinatario { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    public string Correo_Destinatario { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion_Destinatario { get; set; }
    public int id_alerta { get; set; }
    public string Nombre_Alerta { get; set; }
}
