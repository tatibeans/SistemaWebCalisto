using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entidades
{
    [Table("Residente")]
    public class Residente: IEquatable<Residente>
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(8,MinimumLength=6, ErrorMessage = "Cédula inválida.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; }

     
        [StringLength(1, MinimumLength = 1, ErrorMessage = "Debe ingresar solo 1 caracter.")]
        [RegularExpression("M|F", ErrorMessage = "El género debe ser 'M' o 'F'.")]
        [Display(Name = "Género")]
        public string Genero { get; set; }
        
        [Display(Name = "Dieta especial")]
        public string Dieta { get; set; }

        [Display(Name = "Alergias")]
        public string Alergias { get; set; }

        [Display(Name = "Condiciones médicas")]
        public string Condiciones { get; set; }

        [Required]
        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime),"01-01-1800","01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01-01-1800", "01-01-3000", ErrorMessage = "Fecha inválida.")]
        public DateTime FechaIngreso { get; set; } 
        
        [Display(Name = "Fecha de egreso")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "1800-01-01", "3000-01-01", ErrorMessage = "Fecha inválida.")]
        public DateTime? FechaEgreso { get; set; }

        [Required]
        [Display(Name = "Mutualista")]
        public string Mutualista { get; set; } 
        
        [Display(Name = "Servicio fúnebre")]
        public string ServicioFunebre { get; set; }

        public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();

        public virtual ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
        public virtual ICollection<SignoVital> SignosVitales { get; set; } = new List<SignoVital>();
        public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();       
        public virtual ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
        public virtual ICollection<Estudio> Estudios { get; set; } = new List<Estudio>();


        [Required]
        [Display(Name = "Cédula del responsable")]
        public virtual Usuario unResponsable { get; set; }

        public string Rut { get; set; } //agregado
        [ForeignKey("Rut")]
        [Display(Name = "Rut")]
        public virtual Centro Centro { get; set; }

        public Residente() { }

        public bool ValidarCedula()
        {   //Se podria usar alguna api de DNI
            // por el momento controlamos solo cedulas uruguayas
            return Cedula.Length > 6 && Cedula.Length < 9;
        }

        public bool Equals(Residente other)
        {
            if (other == null) return false;
            return (this.Cedula.Equals(other.Cedula));
        }

        public bool Validar()
        {
            //Ver el tema de la validacion de fechas con algun regex
            //Regex re = new Regex("^(0?[1-9]|1[0-9]|2|2[0-9]|3[0-1])/(0?[1-9]|1[0-2])/(d{2}|d{4})$");
            return this.Nombre != null && ValidarCedula() && this.Mutualista != null && this.unResponsable != null;
               
        }

        public bool AddTratamiento(Tratamiento unTratamiento)
        {
            if (unTratamiento == null)
             //   || !unTratamiento.Validar()
                return false;
            Tratamientos.Add(unTratamiento);
            return true;
        }

        //public bool AddAlergias(string alergia)
        //{
        //    if (alergia == null
        //          || Alergias.Contains(alergia))
        //        return false;
        //    Alergias.Add(alergia);
        //    return true;
        //}

        //public bool AddCondicion(string condicion)
        //{
        //    if (condicion == null
        //          || Condiciones.Contains(condicion))
        //        return false;
        //    Condiciones.Add(condicion);
        //    return true;
        //}

    }

    public enum Genero
    {
        M,
        F
    }
}
