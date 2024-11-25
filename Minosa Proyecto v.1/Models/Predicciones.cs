namespace Minosa_Proyecto_v._1.Models
{
    public class Predicciones
    {
        public string hora { get; set; }
        public string Estado { get; set; }
        public string ip { get; set; }
        // Sobrescribir ToString para mostrar información personalizada
        //public override string ToString()
        //{
        //    return $"Hora: {Hora}, Estado: {Estado}";
        //}
    }
}
