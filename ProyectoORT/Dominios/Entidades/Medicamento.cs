using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Medicamento")]
    public class Medicamento
    {
        [Key] 
        public int IdMedicamento { get; set; }

        [Required(ErrorMessage = "*Este campo es requerido.")]
        [Display(Name = "Sustancia")]
        public string Sustancia { get; set; }

        [Required(ErrorMessage = "*Este campo es requerido.")]
        [Display(Name = "Nombre comercial")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "*Este campo es requerido.")]
        [Display(Name = "Presentación (mg / mL)")]
        [Range(1, 1000, ErrorMessage = "Presentacion inválida")]
        public int Presentacion { get; set; }

        [Required(ErrorMessage = "*Este campo es requerido.")]
        [Display(Name = "Cantidad por empaque")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "*Este campo es requerido.")]
        [Display(Name = "Cantidad de cajas en stock")]
        
        public int Stock { get; set; }

        [Display(Name = "Fecha de inicio del stock")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaInicioStock { get; set; }

        [Display(Name = "Fecha de fin del stock")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaFinStock { get; set; }

        [Display(Name = "Laboratorio")]
        public string Laboratorio { get; set; }

        public decimal CajasRestantes()
        {
            return Math.Ceiling((decimal)(Stock / Cantidad));
        }
    }
}
