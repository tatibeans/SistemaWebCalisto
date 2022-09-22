using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CalistoNET6.Models
{
    public class ModeloUsuario
    {
        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Nombre")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe contener al menos 3 caracteres.")]
        public string Nombre { get; set; }

        [Key]
        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Cédula")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Cédula inválida.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Correo")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Debe ingresar un correo valido.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Debe ingresar un correo válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Debe ingresar una contraseña de al menos 8 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "*Campo requerido.")]
        [Display(Name = "Teléfono")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 15 dígitos.")]
        public string Telefono { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fechaCreacion { get; set; }

        [Required(ErrorMessage = "*Debe seleccionar al menos un rol para el usuario.")]
        [Display(Name = "Roles")]
        public ICollection<TipoUsuario> Tipos { get; set; }

        public string Token { get; set; }

        [Display(Name = "Centro")]
        public Centro Centro { get; set; }

        [Display(Name = "Residentes")]
        public ICollection<Residente> Residentes { get; set; }

    }
}