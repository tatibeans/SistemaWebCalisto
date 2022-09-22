using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    public enum TipoInsumo
    {      
        [EnumMember]
        Medicamento = 1,

        [EnumMember]
        Comida = 2,

        [Display(Name = "Artículo de higiene")]
        [EnumMember]
        ArticuloHigiene = 3,

        [Display(Name = "Artículo médico")]
        [EnumMember]
        ArticuloMedico = 4,

        [EnumMember]
        Otro = 5
    }
}
