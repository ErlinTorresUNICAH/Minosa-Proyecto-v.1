using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Minosa_Proyecto_v._1.Models
{
    public class EquipoDetalleViewModel
    {
        public int ID_equipo { get; set; }

        [Required(ErrorMessage = "El campo Número de Serie es obligatorio.")]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Descripción es obligatorio.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        public string Estado { get; set; } = string.Empty;

        public bool Activo { get; set; }

        [Required(ErrorMessage = "El campo Respaldo es obligatorio.")]
        public string Respaldo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Observaciones es obligatorio.")]
        public string Observaciones { get; set; } = string.Empty;

        // Detalle_Equipo fields
        public int ID_detalle_equipo { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Voltaje es obligatorio.")]
        public string Tipo_Voltaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Voltaje es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor del Voltaje debe ser un número positivo.")]
        public int Voltaje { get; set; }

        [Required(ErrorMessage = "El campo Amperaje es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor del Amperaje debe ser un número positivo.")]
        public int Amperaje { get; set; }

        [Required(ErrorMessage = "El campo Número de Puertos RJ45 es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor del Número de Puertos RJ45 debe ser un número positivo.")]
        public int Num_Puertos_RJ45 { get; set; }

        [Required(ErrorMessage = "El campo Número de Puertos SFP es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El valor del Número de Puertos SFP debe ser un número positivo.")]
        public int Num_Puertos_SFP { get; set; }

        [Required(ErrorMessage = "El campo Fecha de Compra es obligatorio.")]
        [DataType(DataType.Date, ErrorMessage = "El formato de la Fecha de Compra no es válido.")]
        public DateTime Fecha_Compra { get; set; }

        [Required(ErrorMessage = "El campo Fecha de Garantía es obligatorio.")]
        [DataType(DataType.Date, ErrorMessage = "El formato de la Fecha de Garantía no es válido.")]
        public DateTime Fecha_Garantia { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Garantía es obligatorio.")]
        public string Tipo_Garantia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Canal es obligatorio.")]
        public string Canal { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Fimware es obligatorio.")]
        public string Firmware { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Usuario es obligatorio.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        public string Contracena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Dirección MAC es obligatorio.")]
        public string MAC_Address { get; set; } = string.Empty;


        [Required(ErrorMessage = "El campo Fecha Instalacion es obligatorio.")]
        public string Fecha_Instalacion{ get; set; } = string.Empty;


        [Required(ErrorMessage = "El campo Ultima Actualizacion es obligatorio.")]
        public string Ultima_Actualizacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Voltaje Energia es obligatorio.")]
        public string Voltaje_Energia { get; set; } = string.Empty;

        // Relacionados con otras tablas
        [Required(ErrorMessage = "El campo Proveedor es obligatorio.")]
        public int ID_proveedor { get; set; }

        [Required(ErrorMessage = "El campo Modelo es obligatorio.")]
        public int ID_modelo { get; set; }

        [Required(ErrorMessage = "El campo Tipo de Equipo es obligatorio.")]
        public int ID_tipo_equipo { get; set; }

        [Required(ErrorMessage = "El campo Área es obligatorio.")]
        public int ID_area { get; set; }

        [Required(ErrorMessage = "El campo IP es obligatorio.")]
        public int ID_ip { get; set; }

        // Listas para los menús desplegables
        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<SelectListItem> Modelos { get; set; } = new();
        public List<SelectListItem> TiposEquipos { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> IPs { get; set; } = new();

        public string Nombre_Proveedor { get; internal set; }
        public string Direccion_Proveedor { get; internal set; }
        public string Telefono_Proveedor { get; internal set; }
        public string Correo_Proveedor { get; internal set; }

        public string Nombre_Area { get; set; }
        public string Nombre_Modelo { get; set; }
        public string? Direccion_IP { get; internal set; }
        public string? Tipo_Equipo { get; internal set; }
        public string DireccionIP { get; internal set; }
    }
}
