using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIREST.Models
{
    public class ModeloSignoVitalApi
    {
        public int IdSignoVital { get; set; }
        
        public string CiFuncionario { get; set; }    
    
        public string FechaRegistro { get; set; }
     
        public double? PresionMaxima { get; set; }
     
        public double? PresionMinima { get; set; }

        public double? Azucar { get; set; }

        public double? Oxigeno { get; set; }

        public double? Temperatura { get; set; }

        public double? Pulso { get; set; }

        public string Comentario { get; set; }

        public ModeloSignoVitalApi() { }


    }


}