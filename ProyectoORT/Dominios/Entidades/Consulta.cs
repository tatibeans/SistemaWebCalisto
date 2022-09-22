using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Consulta")]
    public class Consulta
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Médico")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Debe ingresar un médico.")]
        public string Medico { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Debe ingresar una dirección.")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "Especialidad")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Debe ingresar una especialidad.")]
        public string Especialidad { get; set; }

        [Required]
        [Display(Name = "Fecha de la consulta")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaConsulta { get; set; }

        public Consulta() { }
    }
}
