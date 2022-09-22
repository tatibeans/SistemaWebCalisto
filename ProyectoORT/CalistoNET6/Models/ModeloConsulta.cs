using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CalistoNET6.Models
{
    public class ModeloConsulta
    {
     
        public int Id { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Médico")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Debe ingresar un médico.")]
        public string Medico { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Cédula")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Debe tener la cedula del residente.")]
        public string CiResidente { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Dirección")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Debe ingresar una dirección.")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Especialidad")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Debe ingresar una especialidad.")]
        public string Especialidad { get; set; }

        [Required(ErrorMessage = "*Debe seleccionar una fecha")]
        [Display(Name = "Fecha de la consulta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public string FechaConsulta { get; set; }

    }
}