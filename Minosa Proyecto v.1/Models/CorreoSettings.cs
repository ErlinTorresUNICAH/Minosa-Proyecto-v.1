using System.ComponentModel.DataAnnotations;

public class CorreoSettings
{
    [Required(ErrorMessage = "El servidor SMTP es obligatorio.")]
    public string SmtpServer { get; set; }

    [Required(ErrorMessage = "El puerto SMTP es obligatorio.")]
    [Range(1, 65535, ErrorMessage = "El puerto debe estar entre 1 y 65535.")]
    public int SmtpPort { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
    public string EmailFrom { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string EmailPassword { get; set; }
}
