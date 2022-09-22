using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIREST.Models
{
    public class ModeloUsuario   {
      
        public string Nombre { get; set; }
       
        public string Cedula { get; set; }
       
        public string Correo { get; set; }
  
        public string Password { get; set; }
   
        public string Telefono { get; set; }
  
        public virtual ICollection<TipoUsuario> Tipos { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy MM dd}", ApplyFormatInEditMode = true)]
        public DateTime fechaCreacion { get; set; }

        public string Token { get; set; }

        [Display(Name = "Centro")]
        public Centro Centro { get; set; }

        [Display(Name = "Residentes")]
        public virtual ICollection<Residente> Residentes { get; set; }

    }
}