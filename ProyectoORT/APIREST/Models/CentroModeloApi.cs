using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIREST.Models
{
    public class CentroModeloApi
    {
        public string Nombre { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public string Correo { get; set; }

        public string Rut { get; set; }

        public DateTime OperativoDesde { get; set; }

        public DateTime? OperativoHasta { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }

        public virtual ICollection<Residente> Residentes { get; set; }
    }
}