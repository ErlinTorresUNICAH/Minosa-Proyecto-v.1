using System.ComponentModel.DataAnnotations;

public class Vlan
{
    // Vlans
    public int ID_vlan { get; set; }

    [Required(ErrorMessage = "El nombre de la VLAN es obligatorio.")]
    public string Nombre_Vlan { get; set; }

    [Required(ErrorMessage = "La Subnet es obligatoria.")]
    public string SubNet { get; set; }

    [Required(ErrorMessage = "La máscara de red es obligatoria.")]
    public string Gateway { get; set; }

    [Required(ErrorMessage = "El rango de direcciones IP es obligatorio.")]
    public string DhcpIni { get; set; }

    [Required(ErrorMessage = "El rango de direcciones IP es obligatorio.")]
    public string DhcpFin { get; set; }

    [Required(ErrorMessage = "El rango de direcciones IP es obligatorio.")]
    public string Observaciones { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime Creacion_vlan { get; set; }


    // Ips 
    public int ID_ip { get; set; }

    [Required(ErrorMessage = "La dirección IP es obligatoria.")]
    public string IPV4 { get; set; }
    [Required(ErrorMessage = "El estado es obligatoria.")]
    public string Estado { get; set; }

    public bool ping { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una VLAN.")]
    public int id_vlan { get; set; }

    public bool Activa { get; set; }
}
