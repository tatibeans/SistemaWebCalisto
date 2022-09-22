using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class ModeloLogin
    { 
      
        [Required]
        [Display(Name ="Cedula")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "La cedula debe ser ingresada sin puntos ni guión")]
        public string Cedula { get; set; }

        [Required]
        [Display(Name = "Constraseña")]
        [DataType(DataType.Password)]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Las contraseñas tienen al menos 8 caracteres")]
        public string  Password { get; set; }

        public ModeloLogin() { }
    }
}