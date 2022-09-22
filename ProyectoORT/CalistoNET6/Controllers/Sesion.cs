using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;
using Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using System.Configuration;
using CalistoNET6.Models;
using CalistoNET6.Models.ViewModel;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CalistoNET6.Controllers
{
    public class Sesion : Controller
    {

        public string RolActivo(Usuario usuario)
        {
            if (usuario != null)
            {
                return usuario.DevolverRol();
            }
            else
                return "";
        }

        public string DevolverCi(Usuario usuario)
        {
           
            if (usuario != null)
            {
                return usuario.Cedula;
            }
            else
                return null;
        }

        public bool IngresoNoAutorizado(string rol, Usuario usuario)
        {

            return RolActivo(usuario).ToUpper().Trim() == rol.ToUpper().Trim();
        }
    }
}