using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class ModeloUsuario
    {
        [Required]
        [Display(Name = "Nombre")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Debe ingresar un nombre.")]
        public string Nombre { get; set; }

        [Key]
        [Required]
        [Display(Name = "Cédula")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Cédula inválida.")]
        public string Cedula { get; set; }

        [Required]
        [Display(Name = "Correo")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar un correo.")]
        public string Correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Debe ingresar una contraseña de al menos 8 caracteres.")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 15 dígitos.")]
        public string Telefono { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fechaCreacion { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public ICollection <TipoUsuario> Tipos { get; set; }

        public string Token { get; set; }

        [Display(Name = "Centro")]
        public Centro Centro { get; set; }

        [Display(Name = "Residentes")]
        public ICollection<Residente> Residentes { get; set; }


        public ModeloUsuario() { }

    }
}