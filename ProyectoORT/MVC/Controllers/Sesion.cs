using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC.Controllers
{
    public class Sesion  {

        public static string RolActivo()
        {
            Usuario user = HttpContext.Current.Session["Usuario"] as Usuario;
            if (user != null)
            {
                return user.DevolverRol();
            }
            else
                return "";
        }

        public static string DevolverCi()
        {
            Usuario user = HttpContext.Current.Session["Usuario"] as Usuario;
            if (user != null)
            {
                return user.Cedula;
            }
            else
                return null;
        }

        public static bool IngresoNoAutorizado(string rol)
        {

            return RolActivo().ToUpper().Trim() == rol.ToUpper().Trim();
        }
    }
}