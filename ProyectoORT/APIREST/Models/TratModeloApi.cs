using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dominio.Entidades;

namespace APIREST.Models
{
    public class TratModeloApi
    {
        public int IdTratamiento { get; set; }

        public string CiFuncionario { get; set; }

        public string CiResidente { get; set; }

        public string Medico { get; set; }

        public string Descripcion { get; set; }

        public virtual ICollection<Receta> Recetas { get; set; }

        public DateTime? FechaIngreso { get; set; }

        public DateTime? FechaComienzo { get; set; }

        public DateTime? FechaFin { get; set; }

        public TratModeloApi() { }
    }
}