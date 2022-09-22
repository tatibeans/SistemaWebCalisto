//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using CalistoNET6.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CalistoNET6.Models
{
    public class ModeloResidente
    {
        [Required(ErrorMessage = "*Campo requerido")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe contener al menos 3 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        
        [Key]
        [Required(ErrorMessage = "*Campo requerido")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Cédula inválida.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Género")]
        public string Genero { get; set; }

        [Display(Name = "Dieta especial")]
        public string? Dieta { get; set; }

        [Display(Name = "Alergias")]
        public string? Alergias { get; set; }

        [Display(Name = "Condiciones médicas")]
        public string? Condiciones { get; set; }

        [Display(Name = "CI de usuario responsable")]
        [Required(ErrorMessage = "*Campo requerido")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "Cédula inválida.")]
        public string ResponsableCI { get; set; }


        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Fecha de nacimiento")]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public string FechaNacimiento { get; set; }

        
        [Display(Name = "Fecha de ingreso al centro")]
        public string FechaIngreso { get; set; }

        [Display(Name = "Fecha de egreso del centro")]
        public string FechaEgreso { get; set; }

        [Required(ErrorMessage = "*Campo requerido")]
        [Display(Name = "Mutualista")]
        public string Mutualista { get; set; }

        [Display(Name = "Servicio fúnebre")]
        public string? ServicioFunebre { get; set; }

        [Display(Name = "Tratamientos")]
        public ICollection<ModeloTratamiento> Tratamientos { get; set; }

        public virtual ICollection<ModeloSignoVital> SignosVitales { get; set; } = new List<ModeloSignoVital>();
        public virtual ICollection<ModeloInsumo> Insumos { get; set; } = new List<ModeloInsumo>();
        public virtual ICollection<ModeloConsulta> Consultas { get; set; } = new List<ModeloConsulta>();
        public virtual ICollection<ModeloEstudio> Estudios { get; set; } = new List<ModeloEstudio>();
    
        public ModeloResidente() { }
    }

    
}