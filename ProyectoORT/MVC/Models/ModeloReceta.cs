using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class ModeloReceta
    {
        [Key]
        [Display(Name = "Código de la receta")]
        public int IdReceta { get; set; }

        [Display(Name = "Código del tratamiento")]
        public int IdTratamiento { get; set; }

        [Display(Name = "Tratamiento")]
        [ForeignKey("IdTratamiento")]
        public ModeloTratamiento Tratamiento { get; set; }

        [Display(Name = "Medicamento")]
        public string Medicamento { get; set; }

        [Display(Name = "Fecha de emisión")]
        public DateTime FechaEmision { get; set; }

        [Display(Name = "Fecha de vencimiento")]
        public DateTime FechaVencimiento { get; set; }
    }
}