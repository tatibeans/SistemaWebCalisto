using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    [Table("TipoUsuario")]
    public class TipoUsuario
    {
        [Required]
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }


        [Required]
        [Display(Name = "Tipo")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Debe ingresar un tipo.")]
        public string Tipo { get; set; }


        [Display(Name = "Usuarios")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        public TipoUsuario() { }

        public TipoUsuario(int id, string tipo)
        {
            Id = id;
            Tipo = tipo;
        }
       
    }
}



