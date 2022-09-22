using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CalistoNET6.Models
{
    public class ModeloResidenteActualizar
    {
        [Required(ErrorMessage = "*Campo requerido")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe contener al menos 3 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Key]
        [Required(ErrorMessage = "*Campo requerido")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Cédula inválida.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        [Display(Name = "Dieta especial")]
        public string? Dieta { get; set; }

        [Display(Name = "Alergias")]
        public string? Alergias { get; set; }

        [Display(Name = "Condiciones médicas")]
        public string? Condiciones { get; set; }

        [Display(Name = "CI de usuario responsable")]
        [Required(ErrorMessage = "*Campo requerido")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "Cédula inválida.")]
        public string ResponsableCI { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string? FechaNacimiento { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Mutualista")]
        public string Mutualista { get; set; }

        [Display(Name = "Servicio fúnebre")]
        public string? ServicioFunebre { get; set; }

      

        public ModeloResidenteActualizar() { }

    }
}