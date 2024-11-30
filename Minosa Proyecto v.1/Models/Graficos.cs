using System.Collections.Generic;

namespace Minosa_Proyecto_v._1.Models
{
    // Modelo para Equipos por Tipo
    public class EquipoPorTipo
    {
        public string TipoEquipo { get; set; }
        public int Cantidad { get; set; }
    }

    // Modelo para Equipos por Área
    public class EquipoPorArea
    {
        public string Area { get; set; }
        public int Cantidad { get; set; }
    }

    // Modelo para Equipos por Zona
    public class EquipoPorZona
    {
        public string Zona { get; set; }
        public int Cantidad { get; set; }
    }

    // Modelo para Equipos por Área con Tipo
    public class EquipoPorAreaConTipo
    {
        public string Area { get; set; }
        public string TipoEquipo { get; set; }
        public int Cantidad { get; set; }
    }

    // ViewModel principal para Graficos
    public class GraficosViewModel
    {
        public List<EquipoPorTipo> EquiposPorTipo { get; set; }
        public List<EquipoPorArea> EquiposPorArea { get; set; }
        public List<EquipoPorZona> EquiposPorZona { get; set; }
        public List<EquipoPorAreaConTipo> EquiposPorAreaConTipo { get; set; }
    }
}
