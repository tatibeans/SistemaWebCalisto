using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    [Table("Alerta")]
    public class Alerta
    {
        [Key]
        [Required]
        public int IdAlerta { get; set; }

        [Display(Name = "Cédula del residente")]
        [Required]
        public string CiResidente { get; set; }

        [Display(Name = "Nombre del residente")]
        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Fecha de la alerta")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Situación")]
        public string Situacion { get; set; }

        [Required]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; }

        [Required]
        [Display(Name = "¿Es urgente?")]
        public bool Urgente { get; set; }

        public Alerta() { }
    }
}
