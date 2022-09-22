using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Insumo")]
    public class Insumo
    {
        [Key]
        [Required]
        public int IdInsumo { get; set; }

        [Required]
        [Display(Name = "Funcionario")]
        public string CiFuncionario { get; set; }


        [Required]
        [Display(Name = "Descripción")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar una descripción.")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaIngreso { get; set; }

        [Required]
        [Display(Name = "Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required]
        [Display(Name = "Tipo de insumo")]
        public String Tipo { get; set; }

        public bool Validar()
        {
            return FechaIngreso.Day <= DateTime.Today.Day && Stock > 0 && !String.IsNullOrEmpty(Tipo) && !String.IsNullOrEmpty(Descripcion);
        }
        public Insumo() { }

    }
}
