using System.ComponentModel.DataAnnotations;

public class DireccionIp
{
    public int ID_ip { get; set; }

    [Required(ErrorMessage = "La dirección IP es obligatoria.")]
    public string IPV4 { get; set; }

    public string Estado { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una VLAN.")]
    public int id_vlan { get; set; }

    public string Nombre_Vlan { get; set; }  // Este campo es para almacenar el nombre de la VLAN

    public bool Activa { get; set; }
}
