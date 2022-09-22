using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalistoNET6.Models
{
    public class ModeloResSignos
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Key]
        [Required]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Required]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        [Display(Name = "CI del usuario responsable")]
        [Required]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "Cédula inválida.")]
        public string ResponsableCI { get; set; }

        public virtual ICollection<ModeloSignoVital> SignosVitales { get; set; } = new List<ModeloSignoVital>();

        public ModeloResSignos() { }
    }
}