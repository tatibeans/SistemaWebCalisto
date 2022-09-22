using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models.ViewModel
{
    public class RecoveryPassViewModel
    {
        public string token { get; set; }

        [Required]
        [Display(Name = "Ingresa tu nuevo password")]
        public string Password { get; set; }

      
        [Required]
        [Compare("Password")]
        [Display(Name = "Ingresa nuevamente tu password")]
        public string PasswordRepeat { get; set; }
    }
}