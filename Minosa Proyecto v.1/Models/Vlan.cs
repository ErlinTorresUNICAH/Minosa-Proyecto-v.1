using System.ComponentModel.DataAnnotations;

public class Vlan
{
    public int ID_vlan { get; set; }

    [Required(ErrorMessage = "El nombre de la VLAN es obligatorio.")]
    public string Nombre_Vlan { get; set; }

    [Required(ErrorMessage = "La Subnet es obligatoria.")]
    public string SubNet { get; set; }

    public string Gateway { get; set; }

    public string DhcpIni { get; set; }

    public string DhcpFin { get; set; }

    public string Observaciones { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime Creacion_vlan { get; set; }
}
