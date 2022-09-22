using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models.ViewModel
{
    public class RecoveryViewModel
    {
        
        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Cédula inválida.")]
        public string Cedula { get; set; }

    }
}