using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dominio.Entidades;

namespace CalistoNET6.Models
{
    public class ModeloReceta
    {
        [Key, Column(Order = 0)]
        [Required]
        [Display(Name = "Código de receta")]
        public int IdReceta { get; set; }

        [Required]
        [Display(Name = "Tratamiento")]
        public int IdTratamiento { get; set; }

        [ForeignKey("IdTratamiento"), Column(Order = 1)]
        public Tratamiento Tratamiento { get; set; }

        [Required (ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Fecha de emisión")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string? FechaEmision { get; set; }

        [Display(Name = "Fecha de vencimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string? FechaVencimiento { get; set; }

        [Display(Name = "Sustancia del medicamento")]
        public string? Sustancia { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Dosis (mg / mL / sesiones)")]
        [Range(0.01, 1000, ErrorMessage = "Dosis inválida.")]
        public double Dosis { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Frecuencia (horas)")]
        [Range(1,24, ErrorMessage = "Frecuencia inválida.")]
        public int Frecuencia { get; set; }


        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Duración (días)")]
        [Range(1, 1000, ErrorMessage = "Duración inválida.")]
        public int Duracion { get; set; }
    }
}