using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CalistoNET6.Models.ViewModel
{
    public class RecoveryPassViewModel
    {
        public string token { get; set; }

        [Required(ErrorMessage = "Ingresa tu contraseña")]
        [Display(Name = "Ingresa tu contraseña")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[!@_*])(?=.*[A-Z]).{8,32}$", ErrorMessage = "La contraseña debe inculir al menos una mayusucula, una minusucla, un número y un caracter especial.")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Repite tu contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas deben ser iguales.")]
        [Display(Name = "Repite tu contraseña")]
        public string PasswordRepeat { get; set; }
    }
}