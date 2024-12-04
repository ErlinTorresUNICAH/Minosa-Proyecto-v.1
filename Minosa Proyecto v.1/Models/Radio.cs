using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Minosa_Proyecto_v._1.Models
{
    public class Radio
    {
        // Propiedades de la tabla Radio
        public int ID_Radio { get; set; }

        [Required(ErrorMessage = "La frecuencia es obligatoria.")]
        public string Frecuencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rango de frecuencia es obligatorio.")]
        public string Frecuencia_Rango { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modo es obligatorio.")]
        public string Modo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Ssid es obligatorio.")]
        public string Ssid { get; set; } = string.Empty;

        [Required(ErrorMessage = "La modulación es obligatoria.")]
        public string Modulacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La potencia es obligatoria.")]
        public string Potencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Tx Power es obligatorio.")]
        public string Tx_Power { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Rx Level es obligatorio.")]
        public string Rx_Level { get; set; } = string.Empty;

        [Required(ErrorMessage = "La frecuencia de transmisión es obligatoria.")]
        public string Tx_Freq { get; set; } = string.Empty;

        // Propiedades de la tabla Equipos
        public int ID_equipo { get; set; }

        [Required(ErrorMessage = "El número de serie es requerido.")]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es requerido.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado activo es requerido.")]
        public bool Activo { get; set; }

        [Required(ErrorMessage = "El respaldo es requerido.")]
        public string Respaldo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las observaciones son requeridas.")]
        public string Observaciones { get; set; } = string.Empty;

        // Propiedades de la tabla Detalle_Equipo
        public int ID_detalle_equipo { get; set; }

        [Required(ErrorMessage = "El tipo de voltaje es requerido.")]
        public string Tipo_Voltaje { get; set; } = string.Empty;

        [Required(ErrorMessage = "El voltaje es requerido.")]
        public int Voltaje { get; set; }

        [Required(ErrorMessage = "El amperaje es requerido.")]
        public int Amperaje { get; set; }

        [Required(ErrorMessage = "El número de puertos RJ45 es requerido.")]
        public int Num_Puertos_RJ45 { get; set; }

        [Required(ErrorMessage = "El número de puertos SFP es requerido.")]
        public int Num_Puertos_SFP { get; set; }

        [Required(ErrorMessage = "La fecha de compra es requerida.")]
        public DateTime Fecha_Compra { get; set; }

        [Required(ErrorMessage = "La fecha de garantía es requerida.")]
        public DateTime Fecha_Garantia { get; set; }

        [Required(ErrorMessage = "El tipo de garantía es requerido.")]
        public string Tipo_Garantia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El canal es requerido.")]
        public string Canal { get; set; } = string.Empty;

        [Required(ErrorMessage = "El firmware es requerido.")]
        public string Firmware { get; set; } = string.Empty;

        [Required(ErrorMessage = "El usuario es requerido.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string Contracena { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección MAC es requerida.")]
        public string MAC_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de instalación es requerida.")]
        public string Fecha_Instalacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La última actualización es requerida.")]
        public string Ultima_Actualizacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El voltaje de energía es requerido.")]
        public string Voltaje_Energia { get; set; } = string.Empty;

        // Relacionados con otras tablas
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public int ID_proveedor { get; set; }

        [Required(ErrorMessage = "El modelo es requerido.")]
        public int ID_modelo { get; set; }

        [Required(ErrorMessage = "El tipo de equipo es requerido.")]
        public int ID_tipo_equipo { get; set; }

        [Required(ErrorMessage = "El área es requerida.")]
        public int ID_area { get; set; }

        [Required(ErrorMessage = "La dirección IP es requerida.")]
        public int ID_ip { get; set; }

        // Listas para los menús desplegables
        public List<SelectListItem> Proveedores { get; set; } = new();
        public List<SelectListItem> Modelos { get; set; } = new();
        public List<SelectListItem> TiposEquipos { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();
        public List<SelectListItem> IPs { get; set; } = new();

        // Información relacionada para mostrar en las vistas
        public string Nombre_Proveedor { get; internal set; }
        public string Direccion_Proveedor { get; internal set; }
        public string Telefono_Proveedor { get; internal set; }
        public string Correo_Proveedor { get; internal set; }

        public string Nombre_Area { get; set; }
        public string Nombre_Modelo { get; set; }
        public string? Direccion_IP { get; internal set; }
        public string? Tipo_Equipo { get; internal set; }

        // Campo para almacenar el nombre del equipo
        public string Nombre_Equipo { get; set; }
    }
}
