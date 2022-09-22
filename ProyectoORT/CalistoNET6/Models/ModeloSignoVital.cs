using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dominio.Entidades;

namespace CalistoNET6.Models
{
    public class ModeloSignoVital
    {
        public int IdSignoVital { get; set; }

        [Required]
        public string CiFuncionario { get; set; }
        [Display(Name = "Funcionario")]
        [ForeignKey("Cedula")]
        public ModeloUsuario Funcionario { get; set; }

        [Required]
        public string CiResidente { get; set; }

        [Display(Name = "Residente")]
        [ForeignKey("Cedula")]
        public ModeloResidente Res { get; set; }


        [Required]
        [Display(Name = "Fecha de registro")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public string FechaRegistro { get; set; }

        //[Required(ErrorMessage = "*Campo requerido")]
        [Range((double)0.00, (double)30.00, ErrorMessage = "Valor inválido.")]
        [Display(Name = "Presión Maxima")]

        public string? PresionMaxima { get; set; }
        //[Required(ErrorMessage = "*Campo requerido")]

        [Range((double)0.00, (double)30.00, ErrorMessage = "Valor inválido.")]
        [Display(Name = "Presión Minima")]

        public string? PresionMinima { get; set; }
        //[Required(ErrorMessage = "*Campo requerido")]

        [Range((double)0.00, (double)10.00, ErrorMessage = "Valor inválido.")]
        [Display(Name = "Azúcar en sangre")]
        public string? Azucar { get; set; }

        //[Required(ErrorMessage = "*Campo requerido")]
        [Range((double)0.00, (double)100.00, ErrorMessage = "Valor inválido.")]
        [Display(Name = "Oxigenación")]
        public string? Oxigeno { get; set; }

        //[Required(ErrorMessage = "*Campo requerido")]
        [Range((double)30.00, (double)45.00, ErrorMessage = "Valor inválido.")]
        [Display(Name = "Temperatura corporal")]
        public string? Temperatura { get; set; }
        //[Required(ErrorMessage = "*Campo requerido")]
        [Range((double)40.00, (double)300.00, ErrorMessage = "Valor inválido.")]
        [Display(Name = "Pulso")]
        public string? Pulso { get; set; }
        //[Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Comentarios")]
        public string? Comentario { get; set; }

        public ModeloSignoVital() { }

    }
}