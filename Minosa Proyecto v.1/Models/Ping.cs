public class Dispositivo
{
    public string ID_Equipo { get; set; }
    public string DireccionIP { get; set; }
    public string Area { get; set; }
    public string Descripcion { get; set; }
    public string TipoEquipo { get; set; }
    public string Ping { get; set; }
    public object UltimaHoraPing { get; internal set; }
}