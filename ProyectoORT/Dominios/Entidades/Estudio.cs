using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Estudio")]
    public class Estudio
    {
        [Key]
        [Required]
        public int IdEstudio { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Debe ingresar una descripción.")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Fecha del estudio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaEstudio { get; set; }

        [Display(Name = "Especificaciones")]
        public string Especificaciones { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Debe ingresar una dirección.")]
        public string Direccion { get; set; }


        [Display(Name = "Fecha estimada del resultado")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime? EstimadoResultado { get; set; }

        public Estudio() { }
    }
}
