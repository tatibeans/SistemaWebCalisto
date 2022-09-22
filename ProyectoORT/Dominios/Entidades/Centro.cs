using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Dominio.Entidades
{
    [Table("Centro")]
    //[DataContract(IsReference = true)]
    public class Centro
    {

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Razón social")]
        public string RazonSocial { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 15 dígitos.")]
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

        [Required]
        [Display(Name = "Operativo desde")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime OperativoDesde { get; set; }

        [Display(Name = "Operativo hasta")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime? OperativoHasta { get; set; }

        [Display(Name = "Usuarios")]
        public virtual ICollection<Usuario> Usuarios { get; set; } 

        [Display(Name = "Residentes")]
        public virtual ICollection<Residente> Residentes { get; set; }

        public Centro() { }

        public bool Validar()
        {
            return !string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Direccion) && !string.IsNullOrEmpty(Telefono)
                && !string.IsNullOrEmpty(Correo) && ValidarRut() && OperativoDesde != null;
        }

        public bool ValidarRut()
        {
            // solo acepta numeros de 12 dígitos: ni más ni menos
            return !string.IsNullOrEmpty(Rut) && Regex.IsMatch(Rut.Trim(), @"^(\d{12})$");
        }
    }
}
