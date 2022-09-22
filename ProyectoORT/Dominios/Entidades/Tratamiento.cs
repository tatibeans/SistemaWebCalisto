using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Tratamiento")]
    public class Tratamiento
    {
        [Key]
        [Display(Name = "Código")]
        public int IdTratamiento { get; set; }

        [Required]
        [Display(Name = "Funcionario")]
        public string CiFuncionario { get; set; }

        [Required]
        [Display(Name = "Residente")]
        public string CiResidente { get; set; }

        [Required]
        [Display(Name = "Médico")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar un médico.")]
        public string Medico { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Display(Name = "Recetas")]
        public virtual ICollection<Receta> Recetas { get; set; }
        [Display(Name = "Medicamentos")]
        public virtual ICollection<Medicamento> Medicamentos { get; set; }

        [Display(Name = "Fecha de ingreso")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaIngreso { get; set; }

        [Display(Name = "Fecha de inicio")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaComienzo { get; set; }

        [Display(Name = "Fecha de finalización")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaFin { get; set; }

        public Tratamiento() { }

    }
}
