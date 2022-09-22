using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using Repositorios.Repositorios;
using System.Web.Script.Serialization;
using APIREST.Models;
using Dominio.Entidades;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace APIREST.Controllers
{
    [RoutePrefix("api/Centro")]
    //[EnableCors(origins: "localhost:3000", headers: "*", methods: "GET")]
    public class ApiCentroController : ApiController
    {
        [HttpGet]
        [Route("BuscarCentro")]
        public IHttpActionResult GetCentro(string contexto)
        {
            var context = System.Configuration.ConfigurationManager.ConnectionStrings[$"{contexto}"].ConnectionString;

            RepositorioCentro repoCentro = new RepositorioCentro();
            var centro = repoCentro.Find(context);
            if (centro != null)
            {
                var centroJson = JsonConvert.SerializeObject(centro);
                return Ok(centro);
            }
            else
            {
                return BadRequest("No existe un centro registrado con ese RUT.");
            }
        }

        [HttpPost]
        [Route("CrearCentro")]
        public IHttpActionResult PostCentro([FromBody] CentroModeloApi cenMod)
        {
            RepositorioCentro repoCen = new RepositorioCentro();
            var centro = ParseModeloACentro(cenMod);

            if (centro != null)            {

                bool flag = repoCen.Add(centro);
                if (flag)
                {
                    var centroJson = JsonConvert.SerializeObject(centro);
                    return Ok(centroJson);
                }
                else
                {
                    return BadRequest("Ya existe un centro registrado con ese RUT.");
                }

            }
            return BadRequest("Algo salió mal");

        }
        public IHttpActionResult UpdateCentro([FromBody] CentroModeloApi cenMod)
        {
            // se puede actualizar cuando se agregan o quitan usuarios o residentes,
            // cambiar el tel, la direccion o el correo, y operativo hasta

            if (cenMod != null)
            {
                RepositorioCentro repoCen = new RepositorioCentro();
                Centro centro = ParseModeloACentro(cenMod);

                bool actualizado = repoCen.Update(centro);
                if (actualizado) return Ok(centro);
                return BadRequest("No se pudo actualizar el centro.");
            }
            return BadRequest("Se recibió un objeto nulo.");
        }


        private Centro ParseModeloACentro(CentroModeloApi cenMod)
        {
            Centro parseado = new Centro
            {
                Nombre = cenMod.Nombre,
                Telefono = cenMod.Telefono,
                Correo = cenMod.Correo,
                Direccion = cenMod.Direccion,
                Rut = cenMod.Rut,
                OperativoDesde = cenMod.OperativoDesde,
                OperativoHasta = cenMod.OperativoHasta,
                Usuarios = cenMod.Usuarios,
                Residentes = cenMod.Residentes
               

            };
            return parseado;

        }

        private Centro LimpiarCentro(Centro cen)
        {
            if (cen != null)
            {
                foreach(Usuario u in cen.Usuarios)
                {
                    u.Residentes = null;
                    u.Centro = null;

                    // esto capaz da error!!
                    u.Tipos = null;
                }

                foreach (Residente r in cen.Residentes)
                {
                    r.unResponsable = null;
                    r.Centro = null;
                }
            }
            return cen;
        }

        private Residente ParseModeloAResidente(ResidenteModeloApi r)
        {
            Residente res = new Residente
            {
                Nombre = r.Nombre,
                Cedula = r.Cedula,
                Dieta = r.Dieta,
                Alergias = r.Alergias,
                Genero = r.Genero,
                Condiciones = r.Condiciones,
                unResponsable = (r.unResponsable),
                FechaNacimiento = r.FechaNacimiento,
                FechaEgreso = r.FechaEgreso,
                FechaIngreso = r.FechaIngreso,
                ServicioFunebre = r.ServicioFunebre,
                Mutualista = r.Mutualista,
                Tratamientos = r.Tratamientos,
                Centro = r.Centro
            };

            return res;
        }


        private Usuario ParseModeloAUsuario(ModeloUsuario u)
        {
            Usuario usu = new Usuario
            {
                Nombre = u.Nombre,
                Password = u.Password,
                Cedula = u.Cedula,
                Correo = u.Correo,
                fechaCreacion = u.fechaCreacion,
                Telefono = u.Telefono,
                Tipos = u.Tipos,
                Token = u.Token,
                Centro = u.Centro,
                Residentes = u.Residentes
            };
            return usu;
        }
    }
}