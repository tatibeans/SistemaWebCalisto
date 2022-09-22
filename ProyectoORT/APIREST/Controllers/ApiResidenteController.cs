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

namespace APIREST.Controllers
{
    [RoutePrefix("api/Residente")]
    //[EnableCors(origins: "localhost:3000", headers: "*", methods: "GET")]
    //[EnableCors(origins: "localhost:3000", headers: "*", methods: "GET")]
    public class ApiResidenteController : ApiController
    {
        RepositorioCentro repoCentro = new RepositorioCentro();
        RepositorioResidente repoResi = new RepositorioResidente();
        RepositorioUsuario repoUsu = new RepositorioUsuario();
        RepositorioInsumo repoInsu = new RepositorioInsumo();
        RepositorioEstudio repoEst = new RepositorioEstudio();
        RepositorioConsulta repoCon = new RepositorioConsulta();


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
                    return BadRequest("No existe un residente ingresado con esa cédula.");
                }
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("Borrar")]
        public IHttpActionResult PostBorrarResidente([FromBody] string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var flag = repoResi.BorrarUsu(cedula, contexto);
                if (flag)
                {
                    return Ok(flag);
                }
                else
                {
                    return BadRequest("No se pud borrar el residente");
                }
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("Ingresar")]
        public IHttpActionResult Add([FromBody] ResidenteModeloApi res, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                bool existente = repoResi.FindById(res.Cedula, contexto) != null;
                if (existente) return BadRequest("Ya hay un residente registrado con esa cédula.");

                Residente parseado = ParseModeloARes(res);
                bool agregado = repoResi.Add(parseado, contexto);
                if (agregado) return Ok(parseado);
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

        }



        [HttpPost]
        [Route("Actualizar")]
        // se pueden actualizar: dieta, alergias, condiciones, mutualista, servicio fúnebre, signos vitales
        public IHttpActionResult ActualizarResidente([FromBody] ModeloResidenteActualizar res, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var resViejo = repoResi.FindById(res.Cedula, contexto);
                if (resViejo == null) return BadRequest("No se encontró el residente.");
                var resAActualizar = ParseModeloUpdateARes(res);


                bool actualizado = repoResi.Update(resAActualizar, contexto);

                if (actualizado) return Ok("Datos actualizados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

        }


        #region signos

        [HttpPost]
        [Route("IngresarSignos")]
        public IHttpActionResult IngresarSignosResidente([FromBody] SignoVital signos, string centro, string cedula, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                
                bool actualizado = repoResi.IngresarSignos(signos, contexto, cedula);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

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
                    if (todos != null)
                    {

                        return Ok(todos);

                    }
                }
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }

        [HttpGet]
        [Route("GetSigno")]
        public IHttpActionResult SignoPorId(int id, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                if (id > 0)
                {
                    var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                    var signo = repoResi.FindSigno(contexto, id);
                    if (signo != null)
                    {
                        //var signoJson = JsonConvert.SerializeObject(signo);
                        return Ok(signo);
                    }
                }
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }


        [HttpPost]
        [Route("EditarSigno")]
        public IHttpActionResult EditarSignoRes(string centro, [FromBody] ModeloSignoVitalApi mod, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var boolean = repoResi.EditarSigno(ParseModeloASignos(mod), contexto);
                if (boolean)
                {
                    //var signoJson = JsonConvert.SerializeObject(signo);
                    return Ok();
                }

                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }
        #endregion

        #region insumos

        [HttpPost]
        [Route("IngresarInsumos")]
        public IHttpActionResult IngresarInsumos([FromBody] Insumo ins, string centro, string cedula, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                bool actualizado = repoResi.IngresarInsumo(ins, contexto, cedula);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("BorrarInsumo")]
        public IHttpActionResult BorrarInsumo(string centro, [FromBody] int id, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var insumo = repoResi.BuscarInsumo(contexto, id);
                bool actualizado = repoResi.BorrarInsumo(contexto, insumo);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

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
                    if (todos != null)
                    {

                        return Ok(todos);

                    }
                }
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }
        #endregion

        #region estudios
        [HttpPost]
        [Route("IngresarEstudio")]
        public IHttpActionResult IngresarEstudio([FromBody] Estudio est, string centro, string cedula, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                bool actualizado = repoResi.IngresarEstudio(est, contexto, cedula);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("BorrarEstudio")]
        public IHttpActionResult BorrarEstudio(string centro, [FromBody] int id, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var estudio = repoResi.BuscarEstudio(contexto, id);
                bool actualizado = repoResi.BorrarEstudio(contexto, estudio);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

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
                    if (todos != null)
                    {

                        return Ok(todos);

                    }
                }
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }

        #endregion

        #region consultas
        [HttpPost]
        [Route("IngresarConsulta")]
        public IHttpActionResult IngresarConsulta([FromBody] Consulta con, string centro, string cedula, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                bool actualizado = repoResi.IngresarConsulta(con, contexto, cedula);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("BorrarConsulta")]
        public IHttpActionResult BorrarConsulta(string centro, [FromBody] int id, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var consulta = repoResi.BuscarConsulta(contexto, id);
                bool actualizado = repoResi.BorrarConsulta(contexto, consulta);

                if (actualizado) return Ok("Datos ingresados con éxito.");
                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

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
                    if (todos != null)
                    {

                        return Ok(todos);

                    }
                }
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }



        [HttpGet]
        [Route("GetAllConsultas")]
        public IHttpActionResult GetAllConsultas(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {

                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoResi.FindAllConsultas(contexto);
                if (todos != null)
                {

                    return Ok(todos);

                }

                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }

        [HttpGet]
        [Route("GetAllEstudios")]
        public IHttpActionResult GetAllEstudios(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {

                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoResi.FindAllEstudios(contexto);
                if (todos != null)
                {

                    return Ok(todos);

                }

                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }



        #endregion

        #region Alertas
        [HttpPost]
        [Route("CrearAlerta")]

        public IHttpActionResult CrearAlertaResidente([FromBody] Alerta alerta, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                bool creada = repoResi.CrearAlertaRes(alerta, contexto);

                if (creada)
                {
                    return Ok();
                }

                return BadRequest("Ha habido un error.");
            }
            return BadRequest("Permisos denegados");

        }



        [HttpGet]
        [Route("GetAlertas")]
        public IHttpActionResult GetAlertas(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoResi.FindAlertas(contexto);
                if (todos != null)
                {

                    return Ok(todos);

                }

            }
            return BadRequest("Permisos denegados");

        }


        #endregion

        [HttpGet]
        [Route("FindAll")]
        public IHttpActionResult FindAll(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoResi.FindAll(contexto);
                if (todos != null)
                {
                    var todosLimpios = new List<Residente>();
                    foreach (Residente res in todos)
                    {
                        todosLimpios.Add(LimpiarResidente(res));

                    }

                    return Ok(todosLimpios);

                }


                if (todos != null) return Ok(todos);
                return BadRequest("No hay residentes registrados.");
            }
            return BadRequest("Permisos denegados");

        }

        #region tratamientos, recetas, medicamentos
        [HttpPost]
        [Route("AgregarTratamiento")]
        public IHttpActionResult AgregarTratamiento(string centro, [FromBody] TratModeloApi mod, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                Tratamiento trat = ParseModATrat(mod);
                bool ingresado = false;
                bool sePuedeIngresar = true;
                if (trat.Recetas != null)
                {
                    foreach (Receta r in trat.Recetas)
                    {
                        if (trat.Recetas.Where(x => x.Sustancia == r.Sustancia).ToList().Count > 1)
                        {
                            sePuedeIngresar = false;
                        }
                    }
                }
                if (sePuedeIngresar)
                {
                    ingresado = repoResi.AgregarTratamiento(contexto, trat);
                }

                if (ingresado) { return Ok(); }
                return BadRequest();
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

                if (trats != null)
                {
                    return Ok(trats);

                }
                return BadRequest("No hay residentes registrados.");
            }
            return BadRequest("Permisos denegados");

        }


        [HttpGet]
        [Route("GetUnTratamiento")]
        public IHttpActionResult FindTratamiento(string centro, int id, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var trat = repoResi.FindTratamiento(contexto, id);
                if (trat != null)
                {
                    return Ok(trat);

                }
                return BadRequest("El Tratamiento buscando no exite.");
            }
            return BadRequest("Permisos denegados");

        }

        [HttpGet]
        [Route("GetUnaReceta")]
        public IHttpActionResult FindReceta(string centro, int id, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var receta = repoResi.FindReceta(contexto, id);
                if (receta != null)
                {
                    return Ok(receta);

                }
                return BadRequest("La receta no existe.");
            }
            return BadRequest("Permisos denegados");

        }



        [HttpPost]
        [Route("BorrarTratamiento")]
        public IHttpActionResult EliminarTrat(string centro, [FromBody] int idTrat, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                bool eliminado = repoResi.EliminarTrat(contexto, idTrat);
                if (eliminado) return Ok();
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("EditarTratamiento")]
        public IHttpActionResult EditarTratamiento(string centro, [FromBody] Tratamiento trat, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                bool editado = repoResi.EditarTratamiento(contexto, trat);
                if (editado) return Ok();
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }


        [HttpGet]
        [Route("GetAllTratamientosGenerico")]
        public IHttpActionResult GetAllTratamientosGenerico(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {

                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoResi.FindAllTratamientosGenerico(contexto);
                if (todos != null)
                {

                    return Ok(todos);

                }

                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }


        [HttpPost]
        [Route("AgregarReceta")]
        public IHttpActionResult AgregarReceta(string centro, [FromBody] Receta receta, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                Tratamiento trat = repoResi.FindTratamiento(contexto, receta.IdTratamiento);

                bool agregada = false;
                bool sePuedeIngresar = true;
                if (trat.Recetas != null)
                {
                    if (trat.Recetas.Where(x => x.Sustancia == receta.Sustancia).ToList().Count > 1)
                    {
                        sePuedeIngresar = false;
                    }
                }
                if (sePuedeIngresar)
                {
                    agregada = repoResi.AgregarReceta(contexto, receta);
                }

                if (agregada)
                {
                    return Ok();
                }
                return BadRequest();

            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("AgregarMedicamento")]
        public IHttpActionResult AgregarMedicamento(string centro, [FromBody] Medicamento nuevoMed, string token, int idTrat)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                bool agregada = repoResi.AgregarMedicamento(contexto, nuevoMed, idTrat);
                if (agregada) return Ok();
                return BadRequest();
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("BorrarMedicamento")]
        public IHttpActionResult BorrarMedicamento(string centro, [FromBody] int idMed, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                Medicamento med = repoResi.FindMedicamento(contexto, idMed);

                if (med == null) return BadRequest();

                bool eliminado = repoResi.EliminarMed(contexto, idMed);
                if (eliminado) return Ok();

            }
            return BadRequest("Permisos denegados");

        }




        [HttpPost]
        [Route("BorrarReceta")]
        public IHttpActionResult EliminarRec(string centro, [FromBody] int idRec, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                Receta receta = repoResi.FindReceta(contexto, idRec);
                if (receta == null) { return BadRequest(); }
                Tratamiento trat = repoResi.FindTratamiento(contexto, receta.IdTratamiento);
                if (trat == null) { return BadRequest(); }
                

                if (trat.Medicamentos != null && trat.Medicamentos.Where(x => x.Sustancia == receta.Sustancia).ToList().Count > 0)
                {
                    return BadRequest();

                }
                bool eliminada = repoResi.EliminarReceta(contexto, idRec);
                if (eliminada) return Ok();

            }
            return BadRequest("Permisos denegados");

        }


        #endregion

        #region Auxiliares

        private SignoVital ParseModeloASignos(ModeloSignoVitalApi signos)
        {
            SignoVital signo = new SignoVital
            {
                IdSignoVital = signos.IdSignoVital,
                CiFuncionario = signos.CiFuncionario,
                FechaRegistro = Convert.ToDateTime(signos.FechaRegistro),
                PresionMaxima = signos.PresionMaxima,
                PresionMinima = signos.PresionMinima,
                Azucar = signos.Azucar,
                Oxigeno = signos.Oxigeno,
                Temperatura = signos.Temperatura,
                Pulso = signos.Pulso,
                Comentario = signos.Comentario
            };

            return signo;

        }

        private Residente ParseModeloARes(ResidenteModeloApi mod)
        {
            Residente res = new Residente
            {
                Nombre = mod.Nombre,
                Cedula = mod.Cedula,
                unResponsable = mod.unResponsable,
                Dieta = mod.Dieta,
                Condiciones = mod.Condiciones,
                Alergias = mod.Alergias,
                FechaIngreso = mod.FechaIngreso,
                FechaEgreso = mod.FechaEgreso,
                FechaNacimiento = mod.FechaNacimiento,
                Genero = mod.Genero,
                Mutualista = mod.Mutualista,
                ServicioFunebre = mod.ServicioFunebre,
                SignosVitales = mod.SignosVitales,
                Tratamientos = mod.Tratamientos,
                Centro = mod.Centro
            };

            return res;
        }

        private Residente ParseModeloUpdateARes(ModeloResidenteActualizar mod)
        {
            Residente res = new Residente
            {
                Nombre = mod.Nombre,
                unResponsable = new Usuario() { Cedula = mod.ResponsableCI },
                Cedula = mod.Cedula,
                Dieta = mod.Dieta,
                Condiciones = mod.Condiciones,
                Alergias = mod.Alergias,
                FechaNacimiento = mod.FechaNacimiento,
                Genero = mod.Genero,
                Mutualista = mod.Mutualista,
                ServicioFunebre = mod.ServicioFunebre,

            };

            return res;
        }

        private Usuario ParseModeloAUsu(ModeloUsuario mod)
        {
            if (mod == null) return null;

            Usuario usu = new Usuario
            {
                Cedula = mod.Cedula,
                Centro = mod.Centro,
                Correo = mod.Correo,
                fechaCreacion = mod.fechaCreacion,
                Telefono = mod.Telefono,
                Tipos = mod.Tipos,
                Token = mod.Token,
                Nombre = mod.Nombre,
                Password = mod.Password
            };
            return usu;
        }

        private Tratamiento ParseModATrat(TratModeloApi mod)
        {
            if (mod == null) return null;
            Tratamiento t = new Tratamiento
            {
                IdTratamiento = mod.IdTratamiento,
                CiFuncionario = mod.CiFuncionario,
                CiResidente = mod.CiResidente,
                FechaComienzo = mod.FechaComienzo,
                FechaFin = mod.FechaFin,
                FechaIngreso = mod.FechaIngreso,
                Descripcion = mod.Descripcion,
                Medico = mod.Medico,
                Recetas = new List<Receta>()
            };

            foreach (Receta r in mod.Recetas)
            {
                //hardcodeado para seguir probando funcionamiento
                //r.FechaFinStock = DateTime.Now;
                t.Recetas.Add(r);
            }

            return t;
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
                bool tokenPermitido = repoUsu.TokenUsu(token, contexto);
                if (tokenPermitido)
                {
                    return true;
                }

            }
            return false;
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