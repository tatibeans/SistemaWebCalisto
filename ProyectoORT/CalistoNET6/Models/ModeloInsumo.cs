using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dominio.Entidades;

namespace CalistoNET6.Models
{
    public class ModeloInsumo
    {

        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        public string CiFuncionario { get; set; }
        [Display(Name = "Funcionario")]
        [ForeignKey("Cedula")]
        public ModeloUsuario Funcionario { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        public string CiResidente { get; set; }
        [Required ( ErrorMessage = "*Campo requerido")]
        [Display(Name = "Descripción")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar una descripción.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")] 
        public string FechaIngreso { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Stock")]
        [Range(1, int.MaxValue, ErrorMessage = "El stock debe ser mayor a 0 ")]
        public int Stock { get; set; }

    
        [Display(Name = "Tipo de insumo")]          
        public string TipoString { get; set ; }

      
        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Seleccione un tipo") ,EnumDataType(typeof(TipoInsumo))]

        public TipoInsumo Tipo { get; set; }

        public ModeloInsumo() { }





    }
}