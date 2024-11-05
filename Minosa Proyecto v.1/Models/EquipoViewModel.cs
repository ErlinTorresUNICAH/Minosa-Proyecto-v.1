namespace Minosa_Proyecto_v._1.Models
{
    public class EquipoViewModel
    {
        public int ID_equipo { get; set; }
        public string NumeroSerie { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo_Equipo { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Direccion_IP { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty; // Fix: Initialize with a default value
        public bool Activo { get; set; }
        public string? Respaldo { get; set; }
        public string? Observaciones { get; set; }
        public bool? ping { get; set; }
    }
}
