using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using APIREST.Models;
using Newtonsoft.Json;
using Repositorios.Repositorios;
using System.Web.Script.Serialization;
using Dominio.Entidades;
using MVC.Models;
using System.Web.Http.Cors;
using System.Web.UI;

namespace APIREST.Controllers
{
    [RoutePrefix("api/PWA")]
    public class ApiPWAController : ApiController
    {
        RepositorioCentro repoCentro = new RepositorioCentro();
        RepositorioResidente repoResi = new RepositorioResidente();
        RepositorioUsuario repoUsu = new RepositorioUsuario();
        RepositorioInsumo repoInsu = new RepositorioInsumo();
        RepositorioEstudio repoEst = new RepositorioEstudio();
        RepositorioConsulta repoCon = new RepositorioConsulta();

        [HttpGet]
        [Route("LogIn")]
        public IHttpActionResult GetUsuario(string cedula, string pass, string centro)
        {

            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            //Esta funcionando      
            var unUsuario = repoUsu.LoginUsuarioPWA(cedula, pass, contexto);
            if (unUsuario != null)
            {
                unUsuario = LimpiarUsuario(unUsuario);
                unUsuario.Tipos = LimpiarTipos((List<TipoUsuario>)unUsuario.Tipos);
                //var usuarioJson = JsonConvert.SerializeObject(unUsuario);
                return Ok(unUsuario);
            }
            else
            {
                return BadRequest("No se encontró un usuario con esas credenciales.");
            }
        }

        [HttpGet]
        [Route("BuscarUsuario")]
        public IHttpActionResult GetUsuarioSoloCedula(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
            if (UsuToken(token, centro))
            {
                var unUsuario = repoUsu.FindById(cedula, contexto);
                if (unUsuario != null)
                {
                    unUsuario = LimpiarUsuario(unUsuario);
                    unUsuario.Tipos = LimpiarTipos((List<TipoUsuario>)unUsuario.Tipos);
                    return Ok(unUsuario);
                }
                else
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("Buscar")]
        public IHttpActionResult GetResidenteCedula(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var unRes = repoResi.FindById(cedula, contexto);
                if (unRes != null)
                {

                    unRes = LimpiarResidente(unRes);
                    return Ok(unRes);
                }
                else
                {
                    return Ok("No existe un residente ingresado con esa cédula.");
                }
            }
            return BadRequest("Permisos denegados");

        }

        [HttpGet]
        [Route("GetTratamientos")]
        public IHttpActionResult FindAllTratamientos(string centro, string cedula, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoResi.FindAllTratamientos(contexto, cedula);
                List<Tratamiento> trats = new List<Tratamiento>();
                foreach (Tratamiento t in todos)
                {
                    trats.Add(LimpiarTratamiento(t));

                }

                if (trats != null && trats.Count() > 0)
                {
                    return Ok(trats);

                }
                return Ok("No hay tratamientos ingresados.");
            }
            return BadRequest("Permisos denegados.");

        }


        [HttpGet]
        [Route("GetConsultas")]
        public IHttpActionResult TodasLasConsultas(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                    var todos = repoCon.FindAllConsultasRes(contexto, cedula);
                    if (todos != null && todos.Count() > 0)
                    {

                        return Ok(todos);

                    }
                }
                return Ok("No hay consultas ingresadas.");
            }
            return BadRequest("Permisos denegados.");

        }

        [HttpGet]
        [Route("GetEstudios")]
        public IHttpActionResult TodosLosEstudios(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                    var todos = repoEst.FindAllEstudiosRes(contexto, cedula);
                    if (todos != null && todos.Count() > 0)
                    {

                        return Ok(todos);

                    }
                }
                return Ok("No hay estudios ingresados.");
            }
            return BadRequest("Permisos denegados.");

        }

        [HttpGet]
        [Route("GetSignos")]
        public IHttpActionResult TodosLosSignos(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                    var todos = repoResi.FindAllSignosRes(contexto, cedula);
                    if (todos != null && todos.Count() > 0)
                    {

                        return Ok(todos);

                    }
                }
                return Ok("No hay signos ingresados.");
            }
            return BadRequest("Permisos denegados.");

        }

        [HttpGet]
        [Route("GetInsumos")]
        public IHttpActionResult TodosLosInsumoss(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                    var todos = repoInsu.FindAllInsumosRes(contexto, cedula);
                    if (todos != null && todos.Count() > 0)
                    {

                        return Ok(todos);

                    }
                    return Ok("No hay insumos ingresados.");
                }
                
            }
            return BadRequest("Permisos denegados.");

        }

        [HttpGet]
        [Route("StockMedicamentos")]
        public IHttpActionResult VerStockMedicamentos(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                    // objeto con {medicamento, 10 o 3 dias, tratamiento}
                    var medsFaltan = repoResi.FaltaStockMedicamentos(contexto, cedula);
                    if (medsFaltan != null)
                    {
                        return Ok(medsFaltan);
                    }
                    return Ok();
                }
                
            }
            return BadRequest("Permisos denegados.");
        }

        [HttpGet]
        [Route("VenceReceta")]
        public IHttpActionResult VerRecetasQueVencen(string centro, string cedula, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                    // objeto con {receta, 10 o 3 dias, tratamiento}
                    var recsVencen = repoResi.RecetasVencen(contexto, cedula);
                    if (recsVencen != null)
                    {
                        return Ok(recsVencen);
                    }
                    return Ok();
                }

            }
            return BadRequest("Permisos denegados.");
        }

        [HttpGet]
        [Route("GetAlertas")]
        public IHttpActionResult TodasLasAlertasResidente(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (!string.IsNullOrEmpty(cedula))
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                    var todos = repoResi.FindAlertasRes(cedula, contexto);

                    if (todos != null && todos.Count() > 0)
                    {

                        return Ok(todos);

                    }
                }
                return Ok("No hay alertas ingresadas.");
            }
            return BadRequest("Permisos denegados.");

        }

        #region Manejadores

        private Usuario LimpiarUsuario(Usuario u)
        {
            if (u.Centro != null)
            {
                u.Centro.Usuarios = null;
                u.Centro.Residentes = null;

                foreach (Residente r in u.Residentes)
                {
                    r.unResponsable = null;
                    r.Centro = null;
                }
            }
            return u;
        }

        private List<TipoUsuario> LimpiarTipos(List<TipoUsuario> tipos)
        {
            foreach (TipoUsuario t in tipos)
            {
                if (t == null) return null;
                t.Usuarios = null;
            }
            return tipos;
        }

        private bool UsuToken(string token, string centro)
        {
            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

            if (token != null)
            {
                bool tokenPermitido = repoUsu.TokenPWAUsu(token, contexto);
                if (tokenPermitido)
                {
                    return true;
                }

            }
            return false;
        }

        private Tratamiento LimpiarTratamiento(Tratamiento t)
        {
            if (t != null)
            {
                List<Receta> rec = new List<Receta>();
                foreach (Receta r in t.Recetas)
                {
                    r.Tratamiento = null;
                    rec.Add(r);
                }
                t.Recetas = rec;
            }
            return t;
        }

        private Residente LimpiarResidente(Residente r)
        {
            if (r != null)
            {
                r.Centro.Usuarios = null;
                r.Centro.Residentes = null;
                r.unResponsable.Residentes = null;
                r.unResponsable.Centro = null;
                r.unResponsable.Tipos = LimpiarTipos((List<TipoUsuario>)r.unResponsable.Tipos);
            }

            return r;
        }
        #endregion

        #region Centro
        private bool CentroActivo(string centro)
        {
            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
            return repoCentro.CentroActivo(centro, contexto);


        }
        #endregion

    }
}
