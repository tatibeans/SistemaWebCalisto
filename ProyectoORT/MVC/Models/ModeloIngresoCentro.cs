using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class ModeloIngresoCentro
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Key]
        [Required]
        [Display(Name = "RUT")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "El número de RUT debe tener 12 cifras.")]
        public string Rut { get; set; }

        //[Required]
        //[Display(Name = "Cédulas de los directores")]
        //public virtual ICollection<string> CiDirectores { get; }


        //[Required]
        //[Display(Name = "Directores")]
        //public ICollection<ModeloUsuario> Directores { get; set; } = new List<ModeloUsuario>();

        [Required]
        [Display(Name = "Operativo desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}", ApplyFormatInEditMode = true)]
      //  [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime OperativoDesde { get; set; }
    }
}