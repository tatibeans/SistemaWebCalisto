using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IUsuario
    {
         string Nombre { get; set; }

         string Cedula { get; set; }

         string Correo { get; set; }

         string Password { get; set; }

         string Telefono { get; set; }

         string Token { get; set; }

         DateTime fechaCreacion { get; set; }

         Centro Centro { get; set; }

    }
}
