using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dominio.Entidades;

namespace MVC.Models.ViewModel
{
    public class UsuarioParaResidente
    {
        
        public string Nombre { get; set; }

        public string Cedula { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string Password { get; set; }

        public DateTime FechaCreacion { get; set; }
        public Centro Centro { get; set; }

        public ICollection<Residente> Residentes { get; set; }

        public UsuarioParaResidente() { }
    }
}