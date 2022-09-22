using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("SignoVital")]
    public class SignoVital : IEquatable<SignoVital>
    {
        [Key]
        [Required]
        public int IdSignoVital { get; set; }

        [Required]
        [Display(Name = "Funcionario")]
        public string CiFuncionario { get; set; }

       

        [Required]
        [Display(Name = "Fecha de registro")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaRegistro { get; set; }

        [Range(0, 30)]
        [Display(Name = "Presión máxima")]
        public double? PresionMaxima { get; set; }
        [Range(0, 30)]
        [Display(Name = "Presión mínima")]
        public double? PresionMinima { get; set; }

        [Range(0,10)]
        [Display(Name = "Azúcar en sangre")]
        public double? Azucar { get; set; }

        [Range(0,100)]
        [Display(Name = "Oxigenación")]
        public double? Oxigeno { get; set; }

        [Range(0,45)]
        [Display(Name = "Temperatura corporal")]
        public double? Temperatura { get; set; }

        [Range(0,200)]
        [Display(Name = "Pulso")]
        public double? Pulso { get; set; }

        [Display(Name = "Comentarios")]
        public string Comentario { get; set; }

        public SignoVital() { }


        public bool Validar()
        {
            bool valido = true;

            if (PresionMinima != null && PresionMaxima != null) valido = PresionMinima <= PresionMaxima;
            if ((PresionMaxima == null && PresionMinima != null) || (PresionMaxima != null && PresionMinima == null)) 
                valido = false;
            if (FechaRegistro.Day > DateTime.Today.Day) valido = false;
            return valido;
        }

        bool IEquatable<SignoVital>.Equals(SignoVital other)
        {
            return IdSignoVital == other.IdSignoVital &&
                DateTime.Compare(FechaRegistro, other.FechaRegistro) == 0
                 && Temperatura == other.Temperatura
                && Oxigeno == other.Oxigeno && Azucar == other.Azucar && Pulso == other.Pulso; 
               
        }
    }
}
