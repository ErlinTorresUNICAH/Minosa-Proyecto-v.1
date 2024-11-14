


namespace Minosa_Proyecto_v._1.Models
{
    public class Actividad
    {
        public int ID_Equipo { get; set; }
        public string DireccionIP { get; set; }
        public string Area { get; set; }
        public string DescripcionEquipo { get; set; }
        public string TipoEquipo { get; set; }
        public bool Ping { get; set; }
        public DateTime? UltimaHoraPing  { get; set; }


       

        /*public List<Actividad> Dispositivos { get; set; }
        public List<DireccionIp> DireccionesIp { get; set; }*/
    }
    public class HistorialPing
    {
        public int ID_HistorialPing { get; set; }
        public int ID_ip { get; set; }
        public DateTime HoraPing { get; set; }
        public bool ResultadoPing { get; set; }
    }


}
