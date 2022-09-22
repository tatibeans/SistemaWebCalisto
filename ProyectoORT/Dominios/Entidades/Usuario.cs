using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fare;
using Newtonsoft.Json;
using Dominio.Interfaces;

namespace Dominio.Entidades
{
    [Table("Usuario")]
    public class Usuario : IEquatable<Usuario>, IUsuario
    {

        [Required(ErrorMessage = "*Debe ingresar un nombre.")]
        [Display(Name = "Nombre")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "*Debe ingresar un nombre.")]
        public string Nombre { get; set; }

        [Key]
        [Required(ErrorMessage = "*Debe ingresar una cédula.")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Cédula")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "*Cédula inválida.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "*Debe ingresar un correo electrónico.")]
        [Display(Name = "Correo")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "*Debe ingresar un correo electrónico.")]
        public string Correo { get; set; }
      
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "*Debe ingresar un teléfono de contacto.")]
        [Display(Name = "Teléfono")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El teléfono debe tener entre 8 y 15 dígitos.")]
        public string Telefono { get; set; }

        [StringLength(200)]       
        public string Token { get; set; }

        [StringLength(200)]
        public string TokenApi { get; set; }

        [StringLength(200)]
        public string TokenPWA { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaApi { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fechaCreacion { get; set; }

        [Required(ErrorMessage = "*Debe seleccionar al menos un rol para el usuario.")]
        [Display(Name = "Tipo de Usuario")]
        public virtual ICollection<TipoUsuario> Tipos { get; set; } = new List<TipoUsuario>();

        // [ForeignKey("Rut")]
        [Display(Name = "Centro")]
        public virtual Centro Centro { get; set; }

        [Display(Name = "Residente")]
        public virtual ICollection<Residente> Residentes { get; set; } 


        //Constructores
        public Usuario (){ }

        public Usuario(string cedula, string pass)
        {
            Cedula = cedula;
            Password = pass;
        }

        public Usuario(string nombre, string cedula, string pass, string correo, string telefono, 
            ICollection<TipoUsuario> tipoUsu, Centro centro, ICollection<Residente> residentes)
        {
            Nombre = nombre;
            Cedula = cedula;
            Password = pass;
            Correo = correo;
            Telefono = telefono;
            Tipos = tipoUsu;
            Password = GenerarPassword();
            Centro = centro;
            Residentes = residentes;
        }

        public string GenerarPassword() {
                       
            Xeger xeger = new Xeger(@"^([a-z][A-Z][0-9][!@_*]){1}$");
            string pass = "";
            for (int i = 0; i < 3; i++)
            {
                pass += xeger.Generate();
            }
            //Xeger xeger = new Xeger(@"(?=.*\d)(?=.*[a - z])(?=.*[A - Z])(?=.*[!@_*]){10}");
            
            this.Password = pass;
            return pass;
        }

        //Metodos de validacion
        public bool ValidarCedula()
        {
            int a;
            return Cedula.Length > 6 && Cedula.Length < 9 && int.TryParse(Cedula,out a)
                && Cedula.Substring(0,1) != "0";
        }

        public bool ValidarDatos(){
            return ValidarCedula() &&
                Telefono != null && ValidarPassword()
                &&  Nombre != null && Tipos != null && Tipos.Count != 0 &&
                Regex.IsMatch(Nombre, @"^[a-zA-Z\u00C0-\u017F\s]+$")  
                && Password != null;
        }

        public bool ValidarRegistro()
        {
            return ValidarCedula() && Telefono != null && !String.IsNullOrEmpty(Telefono)
                && Nombre != null && !String.IsNullOrEmpty(Nombre) && 
                Regex.IsMatch(Nombre, @"^[a-zA-Z\u00C0-\u017F\s]+$");
        }

        public bool ValidarPassword()
        {
            return (Password.Length > 7 && Password.Length < 30 && Regex.IsMatch(Password, @"[a-z]") 
                && Regex.IsMatch(Password, @"[0-9]") && Regex.IsMatch(Password, @"[A-Z]") 
                && Regex.IsMatch(Password, @"[!@_*]") && !Password.Contains(Correo) 
                && !Password.Contains(Cedula) && !Password.Contains(Nombre) 
                && !Password.Contains(Telefono));

        }

        public bool Equals(Usuario other)
        {
            if (other == null) return false;
            return (Cedula.Equals(other.Cedula));
        }

        public string DevolverRol()
        {
            string ret = "";

            foreach (TipoUsuario t in Tipos) 
            {
                ret += t.Tipo.ToString().ToUpper() + "|";
            }
            ret = ret.Substring(0, ret.Length - 1);
            return ret;
        }

       
    }
}
