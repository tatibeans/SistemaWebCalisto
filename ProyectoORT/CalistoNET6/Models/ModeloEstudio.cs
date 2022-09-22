using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalistoNET6.Models
{
    public class ModeloEstudio
    {
        
        public int IdEstudio { get; set; }
        
        [Display(Name = "Cédula")]    
        public string CiResidente { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Descripción")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Debe ingresar una descripción.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Fecha del estudio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public string FechaEstudio { get; set; }

        [Display(Name = "Especificaciones")]
        [Required(ErrorMessage = "*Campo requerido")]
        public string Especificaciones { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Dirección")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Debe ingresar una dirección.")]
        public string Direccion { get; set; }


        [Display(Name = "Fecha estimada del resultado")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public string EstimadoResultado { get; set; }

    }
}