using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class ModeloResidenteActualizar
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Key]
        [Required]
        [Display(Name = "Cedula")]
        public string Cedula { get; set; }

        [Required]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        [Display(Name = "Dieta especial")]
        public string Dieta { get; set; }

        [Display(Name = "Alergias")]
        public string Alergias { get; set; }

        [Display(Name = "Condiciones médicas")]
        public string Condiciones { get; set; }

        [Display(Name = "CI de usuario responsable")]
        [Required]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "Cédula inválida.")]
        public string ResponsableCI { get; set; }

        [Required]
        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Display(Name = "Mutualista")]
        public string Mutualista { get; set; }

        [Display(Name = "Servicio fúnebre")]
        public string ServicioFunebre { get; set; }

      

        public ModeloResidenteActualizar() { }

    }
}