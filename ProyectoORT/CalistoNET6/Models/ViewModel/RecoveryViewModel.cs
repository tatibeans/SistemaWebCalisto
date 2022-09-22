using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CalistoNET6.Models.ViewModel
{
    public class RecoveryViewModel
    {
        
        [Required (ErrorMessage = "Cédula inválida.")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Cédula inválida.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

    }
}