using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class ModeloTratamiento
    {
        [Key]
        [Display(Name = "Código")]
        public int IdTratamiento { get; set; }

        [Display(Name = "Médico")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Debe ingresar un médico.")]
        public string Medico { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Dosis")]
        public string Dosis { get; set; }

        [Display(Name = "Frecuencia")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar una frecuencia.")]
        public string Frecuencia { get; set; }

        [Display(Name = "Duración")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar una duración.")]
        public string Duracion { get; set; }

        [Display(Name = "Recetas")]
        public ICollection<ModeloReceta> Recetas { get; set; }
    }
}