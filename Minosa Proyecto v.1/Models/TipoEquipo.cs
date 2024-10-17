using System;
using System.ComponentModel.DataAnnotations;

public class TipoEquipo
{
    public int ID_tipo_equipo { get; set; }

    [Required(ErrorMessage = "El tipo de equipo es obligatorio.")]
    public string Tipo_Equipo { get; set; } = string.Empty;

    public DateTime Creacion_Tipo_Equipo { get; set; } = DateTime.Now;
}
