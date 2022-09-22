using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dominio.Entidades;

namespace CalistoNET6.Models
{
    public class ModeloTratamiento
    {
        [Key]
        [Display(Name = "Código")]
        public int IdTratamiento { get; set; }

        [Required]
        [Display(Name = "Funcionario")]
        public string CiFuncionario { get; set; }

        [Display(Name = "Residente")]
        [Required]
        public string CiResidente { get; set; }

        [Display(Name = "Residente")]
        [ForeignKey("Cedula")]
        public ModeloResidente Res { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Médico")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Debe ingresar un médico.")]
        public string Medico { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Display(Name = "Recetas")]
        public ICollection<ModeloReceta> Recetas { get; set; }
        [Display(Name = "Medicamentos")]
        public ICollection<Medicamento> Medicamentos { get; set; }

        [Display(Name = "Fecha de ingreso")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string? FechaIngreso { get; set; }

        [Required(ErrorMessage = "*Debe ingresar una fecha.")]
        [Display(Name = "Fecha de inicio")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string? FechaComienzo { get; set; }

        [Display(Name = "Fecha de finalización")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string? FechaFin { get; set; }
    }
}