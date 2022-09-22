using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Interfaces;

namespace Repositorios.UtilidadesBD
{
    public class UsuarioListasId : IUsuario
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public string Token { get; set; }
        public DateTime fechaCreacion { get; set; }       
        public ICollection<string> CentroRut { get; set; }
        public ICollection<string> ResidenteCI { get; set; }
        public Centro Centro { get; set; }
    }
}
