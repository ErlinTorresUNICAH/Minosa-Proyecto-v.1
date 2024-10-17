using System.ComponentModel.DataAnnotations;

public class Marca
{
    public int ID_marca { get; set; }

    [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
    public string NombreMarca { get; set; } = string.Empty;

    public bool Activa { get; set; } = true;
}
