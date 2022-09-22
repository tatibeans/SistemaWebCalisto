using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dominio.Entidades;

namespace APIREST.Models
{
    public class ResidenteModeloApi
    {
        public string Nombre { get; set; }

        public string Cedula { get; set; }

        public string Genero { get; set; }

        public string Dieta { get; set; }

        public string Alergias { get; set; }

        public string Condiciones { get; set; }
        public ICollection<SignoVital> SignosVitales { get; set; }

        public Usuario unResponsable { get; set; }

        public Centro Centro { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public DateTime FechaIngreso { get; set; }

        public DateTime? FechaEgreso { get; set; }

        public string Mutualista { get; set; }

        public string ServicioFunebre { get; set; }

        public ICollection<Tratamiento> Tratamientos { get; set; }

    }
}