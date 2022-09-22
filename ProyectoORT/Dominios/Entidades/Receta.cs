using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Receta")]
    public class Receta
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

        [Required]
        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaIngreso { get; set; }

        [Required]
        [Display(Name = "Fecha de emisión")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaEmision { get; set; }

        [Display(Name = "Fecha de vencimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaVencimiento { get; set; }

        [Display(Name = "Sustancia del medicamento")]
        public string Sustancia { get; set; }

        [Required]
        [Display(Name = "Dosis (mg / mL / sesiones)")]
        [Range(0.01, 1000, ErrorMessage = "Dosis inválida.")]
        public double Dosis { get; set; }

        [Required]
        [Display(Name = "Frecuencia (horas)")]
        [Range(0.01, 1000, ErrorMessage = "Frecuencia inválida.")]
        public int Frecuencia { get; set; }


        [Required]
        [Display(Name = "Duración (días)")]
        [Range(1, 1000, ErrorMessage = "Duración inválida.")]
        public int Duracion { get; set; }

        public Receta() { }

    }
}
