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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace CalistoNET6.Controllers
{
    public class ResidenteController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();
        private HttpResponseMessage respuesta = new HttpResponseMessage();
        private Uri usuarioUri = null;
        private readonly IConfiguration _configuration;
        private Sesion sesion = new Sesion();


        public ResidenteController(IConfiguration configuration)
        {
            cliente.BaseAddress = new Uri("https://proyectocalistoort.azurewebsites.net");
            usuarioUri = new Uri("https://proyectocalistoortapi.azurewebsites.net/api");
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            _configuration = configuration;
        }

        #region ingreso, actualizar, borrar, ver
        [HttpGet]
        public ActionResult DetallesResidente(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var residente = response.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Residente>(residente);

                return PartialView("DetallesResidente", ParseResidenteAModel(objeto));
            }

            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult BorrarResidente(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/Borrar?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, cedula).Result;

            var respuesta = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "El residente fue eliminado con éxito." });
            }
            else
            {
                return Json(new { result = "error", mensaje = "No se pudo elimiar el residente" });

            }
        }

        // GET: Residente
        public ActionResult IngresoResidente()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
            return View();

        }


        [HttpPost]
        public ActionResult IngresoResidente(ModeloResidente modelo, string Alergias, string Condiciones)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            modelo.Alergias = Alergias;
            modelo.Condiciones = Condiciones;

            Usuario resp = null;
            response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + modelo.ResponsableCI + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
            if (!response.IsSuccessStatusCode)
            {               
                return Json(new { result = "error", mensaje = "No se encontró el usuario responsable con esa cédula." });               
            }
            var usuJson = response.Content.ReadAsStringAsync().Result;
            resp = JsonConvert.DeserializeObject<Usuario>(usuJson);

            response = cliente.GetAsync(usuarioUri + "/Centro/BuscarCentro?contexto=" + _configuration["Centro"]).Result;

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { result = "error", mensaje = "Hubo un error. Contactar al equipo de IT." });                
            }
            var centroJson = response.Content.ReadAsStringAsync().Result;
            Centro cen = JsonConvert.DeserializeObject<Centro>(centroJson);

            Residente res = new Residente
            {
                Nombre = modelo.Nombre,
                Cedula = modelo.Cedula,
                Genero = modelo.Genero,
                Mutualista = modelo.Mutualista,
                Dieta = modelo.Dieta,
                ServicioFunebre = modelo.ServicioFunebre,
                FechaNacimiento = Convert.ToDateTime(modelo.FechaNacimiento),
                Condiciones = modelo.Condiciones,
                Alergias = modelo.Alergias,
                FechaIngreso = DateTime.Now,
                unResponsable = new Usuario() { Cedula = resp.Cedula },
                Centro = new Centro() { Rut = cen.Rut },
                FechaEgreso = null
            };

            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/Ingresar?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, res).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "El residente fue ingresado con éxito." });
            }
           
            return Json(new { result = "error", mensaje = "Ya existe un residente ingresado con esta cedula." });           

        }




        [HttpGet]
        public ActionResult ActualizarResidente(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);

                ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
                return PartialView("EditarResidente", ParseResidenteAModelActualizar(residente));
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult ActualizarResidente(ModeloResidenteActualizar mod, string listaAlergias = "", string listaCondiciones = "")
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            mod.Condiciones = listaCondiciones;
            mod.Alergias = listaAlergias;

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + mod.Cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/Actualizar?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, mod).Result;
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Datos actualizados con éxito.";

                    return Redirect("Index");
                }
            }

            ViewBag.Mensaje = "Datos incorrectos.";
            return PartialView("EditarResidente", mod);
        }

        #endregion

        #region signos vitales


        [HttpGet]
        public ActionResult AgregarSigno(string cedula)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);
                ModeloSignoVital modSig = new ModeloSignoVital
                {
                    CiFuncionario = usuario.Cedula,
                    CiResidente = residente.Cedula
                };
                return PartialView("AgregarSigno", modSig);
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult AgregarSigno(ModeloSignoVital mod)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            if (mod != null)
            {
                mod.FechaRegistro = DateTime.Now.ToString();
                mod.CiFuncionario = usuario.Cedula;

                //SignoVital signo = ParseModeloASigno(mod);

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/IngresarSignos?centro=" + _configuration["Centro"] + "&cedula=" + mod.CiResidente + "&token=" + usuario.TokenApi, mod).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }

            }
            return View("Error", "Shared");
        }


        [HttpGet]
        public ActionResult EditarSigno(string idSig)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/GetSigno?id=" + idSig + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var sigJson = response.Content.ReadAsStringAsync().Result;
                var signo = JsonConvert.DeserializeObject<SignoVital>(sigJson);

                return PartialView("EditarOBorrarSigno", ParseSignoAModelo(signo));
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult EditarSigno(ModeloSignoVital mod)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/GetSigno?id=" + mod.IdSignoVital + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {

                mod.FechaRegistro = DateTime.Now.ToString();
                mod.CiFuncionario = usuario.Cedula;
                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/EditarSigno?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, mod).Result;
                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("InformacionExtra", "Residente", new { ci = mod.CiResidente });

                }
            }

            return View("Error", "Shared");
        }


        #endregion

        #region vistas y tablas
        [HttpGet]
        public ActionResult Index()
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            List<ModeloResidenteActualizar> todos = GetTodosParaActualizar();
            return View(todos);
        }


        [HttpGet]
        public ActionResult InicioAgregarSigno()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            List<ModeloResSignos> todos = GetTodosParaSignos();
            return PartialView(todos);

        }

        [HttpGet]
        public ActionResult InformacionExtra(string ci)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            response = cliente.GetAsync(usuarioUri + "/Residente/GetTratamientos?centro=" +
               _configuration["Centro"] + "&cedula=" + ci + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var modTratJson = response.Content.ReadAsStringAsync().Result;
                var modTrat = JsonConvert.DeserializeObject<List<Tratamiento>>(modTratJson);
                List<ModeloTratamiento> modList = new List<ModeloTratamiento>();

                foreach (Tratamiento t in modTrat)
                {
                    modList.Add(ParseTratamientoAModelo(t));
                }
                modList = modList.OrderByDescending(x => x.IdTratamiento).ToList();
                ViewData["Tratamiento"] = modList;
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/GetInsumos?centro=" +
                _configuration["Centro"] + "&cedula=" + ci + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var modInsuJson = response.Content.ReadAsStringAsync().Result;
                var modInsu = JsonConvert.DeserializeObject<List<Insumo>>(modInsuJson);
                List<ModeloInsumo> modList = new List<ModeloInsumo>();

                foreach (Insumo i in modInsu)
                {
                    modList.Add(ParseInsumoAModelo(i));
                }
                modList = modList.OrderByDescending(x => x.IdInsumo).ToList();
                ViewData["Insumos"] = modList;
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/GetEstudios?centro=" +
               _configuration["Centro"] + "&cedula=" + ci + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var modEstJson = response.Content.ReadAsStringAsync().Result;
                var modEst = JsonConvert.DeserializeObject<List<Estudio>>(modEstJson);
                List<ModeloEstudio> modList = new List<ModeloEstudio>();

                foreach (Estudio e in modEst)
                {
                    modList.Add(ParseEstudioAModelo(e));

                }
                modList = modList.OrderByDescending(x => x.IdEstudio).ToList();
                ViewData["Estudios"] = modList;
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/GetConsultas?centro=" +
              _configuration["Centro"] + "&cedula=" + ci + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var modConJson = response.Content.ReadAsStringAsync().Result;
                var modCon = JsonConvert.DeserializeObject<List<Consulta>>(modConJson);
                List<ModeloConsulta> modList = new List<ModeloConsulta>();

                foreach (Consulta c in modCon)
                {
                    modList.Add(ParseConsultaAModelo(c));

                }
                modList = modList.OrderByDescending(x => x.Id).ToList();
                ViewData["Consultas"] = modList;
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/GetSignos?centro=" +
                _configuration["Centro"] + "&cedula=" + ci + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var signosJson = response.Content.ReadAsStringAsync().Result;
                var signos = JsonConvert.DeserializeObject<List<SignoVital>>(signosJson);

                List<ModeloSignoVital> modSignos = new List<ModeloSignoVital>();
                foreach (SignoVital s in signos)
                {
                    ModeloSignoVital m = ParseSignoAModelo(s);
                    m.CiResidente = ci;
                    modSignos.Add(m);
                }


                return View(modSignos);
            }



            return Json(new { result = "error", mensaje = "Ha habido un error." });

        }



        [HttpGet]
        public ActionResult ActualizaRes(int indicePag = 1, int cantidadRes = 10)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            try
            {
                List<Residente> listaRes = GetResidentesPaginable(indicePag, cantidadRes);
                List<ModeloResidenteActualizar> listaFormato = new List<ModeloResidenteActualizar>();
                foreach (Residente r in listaRes)
                {
                    listaFormato.Add(ParseResidenteAModelActualizar(r));
                }
                return PartialView("TablaResidentes", listaFormato);

            }
            catch (Exception ex)
            {
                return View(new List<ModeloResidente>());
            }

        }

        [HttpGet]
        public ActionResult AgregaSignoRes(int indicePag = 1, int cantidadRes = 10)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            try
            {
                List<Residente> listaRes = GetResidentesPaginable(indicePag, cantidadRes);
                List<ModeloResSignos> listaFormato = new List<ModeloResSignos>();
                foreach (Residente r in listaRes)
                {
                    listaFormato.Add(ParseResidenteAModelSignos(r));
                }
                return PartialView("TablaResidentes", listaFormato);

            }
            catch (Exception ex)
            {
                return View(new List<ModeloResidente>());
            }

        }
        #endregion


        #region tratamientos y recetas

        [HttpGet]
        public ActionResult AgregarTratamiento(string cedula)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);
                ModeloTratamiento modTrat = new ModeloTratamiento
                {
                    CiFuncionario = usuario.Cedula,
                    CiResidente = residente.Cedula
                };
                ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
                return View("AgregarTratamiento", modTrat);
            }

            // poner json
            return Json(new { result = "error", mensaje = "Los datos ingresados no son válidos." });
        }


        [HttpGet]
        public ActionResult AgregarReceta()
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            ModeloReceta nuevaReceta = new ModeloReceta();
           
            ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
            return PartialView("AgregarReceta", nuevaReceta);

        }

        [HttpPost]
        public ActionResult AgregarRecetaDesdeTabla(ModeloReceta modelo)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            if (string.IsNullOrEmpty(modelo.FechaVencimiento)) { modelo.FechaVencimiento = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd"); }
            else
            {
                modelo.FechaVencimiento = Convert.ToDateTime(modelo.FechaVencimiento).ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrEmpty(modelo.FechaEmision)) { modelo.FechaEmision = DateTime.Today.ToString("yyyy-MM-dd"); }
            else
            {
                modelo.FechaEmision = Convert.ToDateTime(modelo.FechaEmision).ToString("yyyy-MM-dd");
            }


            //modelo.FechaEmision = DateTime.Today.ToString("yyyy-MM-dd");
            //if (String.IsNullOrEmpty(modelo.FechaVencimiento)) 
            //    modelo.FechaVencimiento = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            Receta rec = ParseModeloAReceta(modelo);

            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/AgregarReceta?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, rec).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "Receta agregada con éxito." });
            }

            return Json(new { result = "error", mensaje = "Ha ocurrido un error." });
        }


        [HttpPost]
        public ActionResult AgregarTratamiento(ModeloTratamiento mod, List<ModeloReceta> rec)
        {
          
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            try
            {
                if (mod != null)
                {

                    if (mod.FechaComienzo == null)
                    { 
                        mod.FechaComienzo = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                    mod.FechaIngreso = DateTime.Now.ToString("yyyy-MM-dd");

                    double maxFin;
                    if (rec.Count() == 0) { maxFin = 10; }
                    else maxFin = MaximaDuracion(rec);
                    

                    DateTime comienzo = Convert.ToDateTime(mod.FechaComienzo);
                    mod.FechaFin = comienzo.AddDays(maxFin).ToString("yyyy-MM-dd"); 
                    mod.Recetas = rec;


                    Tratamiento trat = ParseModeloATrat(mod);
                    trat.CiResidente = mod.CiResidente;
                    
                    response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/AgregarTratamiento?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, trat).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { result = "success", mensaje = "Tratamiento agregado con éxito" });
                    }
                }
                return Json(new { result = "error", mensaje = "Los datos ingresados no son válidos." });
            }
            catch (Exception ex) 
            {
                return Json(new { result = "error", mensaje = ex.Message });
            }
        }
        [HttpGet]
        public ActionResult EditTratamientoDesdeTabla(int id)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            response = cliente.GetAsync(usuarioUri + "/Residente/GetUnTratamiento?id=" + id + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var Trat = response.Content.ReadAsStringAsync().Result;
                var mod = JsonConvert.DeserializeObject<ModeloTratamiento>(Trat);
                mod.FechaFin = mod.FechaFin.Substring(0, 10);
                mod.FechaComienzo = mod.FechaComienzo.Substring(0, 10);
                ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
                return PartialView("EditTratamientoTablaTratamientos", mod);
            }

            return PartialView("Error");
        }

        [HttpGet]
        public ActionResult EditRecetaDesdeTabla(int id)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            response = cliente.GetAsync(usuarioUri + "/Residente/GetUnaReceta?id=" + id + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var Receta = response.Content.ReadAsStringAsync().Result;
                var mod = JsonConvert.DeserializeObject<ModeloReceta>(Receta);
                mod.FechaEmision = mod.FechaEmision.Substring(0, 10);
                mod.FechaVencimiento = mod.FechaVencimiento.Substring(0, 10);
                //.Replace(',', ':')
                ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");
                return PartialView("addRecetaTablaTratamientos", mod);
            }

            return PartialView("Error");
        }

        [HttpPost]
        public ActionResult EditarTratamiento(ModeloTratamiento mod)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            if (mod != null)
            {
                if (mod.FechaComienzo == null) mod.FechaComienzo = DateTime.Now.ToString("dd/MM/yyyy");

                Tratamiento trat = ParseModeloATrat(mod);
                trat.CiResidente = mod.CiResidente;
                trat.CiFuncionario = usuario.Cedula;


                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/EditarTratamiento?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, trat).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { result = "success", mensaje = "Tratamiento editado con éxito." });
                }
            }
            return Json(new { result = "error", mensaje = "Los datos ingresados no son válidos." });
        }


        [HttpPost]
        public ActionResult EliminarTratamiento(int idTrat)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/BorrarTratamiento?centro=" +
               _configuration["Centro"] + "&token=" + usuario.TokenApi, idTrat).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "Tratamiento eliminado con éxito" });
            }
            return Json(new { result = "error", mensaje = "Ha habido un error." });
        }

        [HttpGet]
        public ActionResult AddRecetaDesdeTabla(int idTrat)
        {
            ModeloReceta modReceta = new ModeloReceta();
            modReceta.IdTratamiento = idTrat;
            ViewBag.FechaHoy = DateTime.Today.ToString("yyyy-MM-dd");

            return PartialView("addRecetaTablaTratamientos", modReceta);
        }



        [HttpPost]
        public ActionResult EliminarReceta(int idRec)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/BorrarReceta?centro=" +
              _configuration["Centro"] + "&token=" + usuario.TokenApi, idRec).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "Receta eliminada con éxito" });
            }
            return Json(new { result = "error", mensaje = "Ha habido un error." });
        }


        [HttpPost]
        public ActionResult EditarReceta(ModeloReceta mod)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            if (mod != null)
            {
                Receta unaRec = ParseModeloAReceta(mod);

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/EditarReceta?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, unaRec).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { result = "success", mensaje = "Receta editada con éxito." });
                }
            }
            return Json(new { result = "error", mensaje = "Los datos ingresados no son válidos." });
        }

        #endregion

        #region Medicamentos

        [HttpGet]
        public ActionResult AgregarMedicamento(string cedula)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/GetTratamientos?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi + "&cedula="+ cedula).Result;

            if (response.IsSuccessStatusCode)
            {
                var objeto = response.Content.ReadAsStringAsync().Result;
                var tratamientos = JsonConvert.DeserializeObject<List<Tratamiento>>(objeto);
                //List<string> sustancias = new List<string>();
                //List<SelectListItem> listaSustancias = new List<SelectListItem>();
                //foreach (Receta r in recetas)
                //{
                //    if (!sustancias.Contains(r.Sustancia))
                //    {
                //        sustancias.Add(r.Sustancia);
                //        listaSustancias.Add( new SelectListItem { Text=r.Sustancia, Value=r.Sustancia});
                //    }
                //}
                //if (sustancias.Count > 0)
                //{
                //    ViewBag.Sustancias = listaSustancias;
                List<ModeloTratamiento> modTrat = new List<ModeloTratamiento>();
                foreach (Tratamiento t in tratamientos) 
                {
                    modTrat.Add(ParseTratamientoAModelo(t));
                }

                ViewBag.Tratamientos = modTrat;
                return PartialView("AgregarMedicamento");

                //return PartialView("AgregarMedicamento",modTrat);

                //}
            }
            return Json(new { result = "error", mensaje = "El residente todavia no posee ninguna receta" });
        }

        [HttpPost]
        public ActionResult AgregarMedicamento(ModeloTratamiento tratamiento,string sustancia, string nombre, string laboratorio, int presentacion, int cantidad, int stock)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            Medicamento nuevoMed = new Medicamento()
            {
                Cantidad = cantidad,
                Nombre = nombre,
                Presentacion = presentacion,
                Sustancia = sustancia,
                Stock = stock,
               Laboratorio = laboratorio
            };

            if(tratamiento.Medicamentos == null || tratamiento.Medicamentos.Where(s => s.Sustancia == sustancia).OrderByDescending(x => x.FechaFinStock).FirstOrDefault() == null)
            {
                nuevoMed.FechaInicioStock = DateTime.Now;
            }
            else
            {
                Medicamento medUltimaFecha = tratamiento.Medicamentos.Where(s => s.Sustancia == sustancia).OrderByDescending(x => x.FechaFinStock).FirstOrDefault();
                nuevoMed.FechaInicioStock = medUltimaFecha.FechaFinStock;

            }
         
            ModeloReceta rec = tratamiento.Recetas.Where(s => s.Sustancia == sustancia).FirstOrDefault();     
            
         
            double unidadesPorDia = (24 / (double)rec.Frecuencia) * ((double)rec.Dosis / (double)nuevoMed.Presentacion);
         
            double cantDias = (nuevoMed.Stock * nuevoMed.Cantidad ) / unidadesPorDia;
            nuevoMed.FechaFinStock = nuevoMed.FechaInicioStock.AddDays(Math.Floor(cantDias));



            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/AgregarMedicamento?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi + "&idTrat=" + tratamiento.IdTratamiento, nuevoMed).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "Medicamento agregado con éxito." });
            }

            return Json(new { result = "error", mensaje = "Ha ocurrido un error." });
        }


        [HttpPost]
        public ActionResult EliminarMedicamento(int idMed)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/BorrarMedicamento?centro=" +
              _configuration["Centro"] + "&token=" + usuario.TokenApi, idMed).Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "Medicamento eliminado con éxito" });
            }
            return Json(new { result = "error", mensaje = "Ha habido un error." });
        }

        #endregion

        #region insumoMetodos

        [HttpGet]
        public ActionResult AgregarInsumo(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);
                ModeloInsumo modIns = new ModeloInsumo
                {
                    CiFuncionario = usuario.Cedula,
                    CiResidente = residente.Cedula
                };
                return PartialView("AgregarInsumo", modIns);
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult AgregarInsumo(ModeloInsumo mod)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            if (mod != null && mod.Tipo != 0)
            {

                mod.FechaIngreso = DateTime.Now.ToString();
                mod.CiFuncionario = usuario.Cedula;

                Insumo ins = ParseModeloAInsumo(mod);
                if (!ins.Validar())
                    return PartialView("Error", "Shared");

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/IngresarInsumos?centro=" + _configuration["Centro"] + "&cedula=" + mod.CiResidente + "&token=" + usuario.TokenApi, ins).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }


                return PartialView("Error", "Shared");
            }
            return PartialView("Error", "Shared");
        }

        [HttpPost]
        public ActionResult BorrarInsumo(int id)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            if (id > 0)
            {
                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/BorrarInsumo?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }
            }
            return Json(new { result = "error", mensaje = "No se pudo borrar el insumo." });
        }
        #endregion

        #region estudioMetodos
        [HttpGet]
        public ActionResult AgregarEstudio(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);
                ModeloEstudio modEst = new ModeloEstudio
                {
                    CiResidente = residente.Cedula
                };
                return PartialView("AgregarEstudio", modEst);
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult AgregarEstudio(ModeloEstudio mod)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            if (mod != null)
            {

                Estudio est = ParseModeloAEstudio(mod);

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/IngresarEstudio?centro=" + _configuration["Centro"] + "&cedula=" + mod.CiResidente + "&token=" + usuario.TokenApi, est).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }


                return View("Error", "Shared");
            }
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult BorrarEstudio(int id)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            if (id > 0)
            {

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/BorrarEstudio?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }
            }
            return Json(new { result = "error", mensaje = "No se pudo borrar el Estudio." });
        }

        #endregion

        #region consultaMetodos
        [HttpGet]
        public ActionResult AgregarConsulta(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);
                ModeloConsulta modCon = new ModeloConsulta
                {
                    CiResidente = residente.Cedula
                };
                return PartialView("AgregarConsulta", modCon);
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult AgregarConsulta(ModeloConsulta mod)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            if (mod != null)
            {
                Consulta con = ParseModeloAConsulta(mod);

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/IngresarConsulta?centro=" + _configuration["Centro"] + "&cedula=" + mod.CiResidente + "&token=" + usuario.TokenApi, con).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }

                return View("Error", "Shared");
            }
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult BorrarConsulta(int id)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            if (id > 0)
            {

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/BorrarConsulta?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return Redirect("InicioAgregarSigno");
                }
            }
            return Json(new { result = "error", mensaje = "No se pudo borrar la consulta." });
        }

        #endregion

        #region Alertas

        [HttpGet]
        public ActionResult CrearAlerta()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            List<Residente> lista = GetTodosGenerico();
            List<ModeloResidente> listaModelo = new List<ModeloResidente>();
            foreach (Residente r in lista)
            {

                listaModelo.Add(ParseResidenteAModel(r));
            }
            return View(listaModelo);

            //// poner json
            //return View("Error", "Shared");
        }


        [HttpPost]
        public ActionResult CrearAlerta(string cedula, bool urgente, string situacion, string mensaje,string nombre)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            Alerta nuevaAlerta = new Alerta()
            {
                Urgente = urgente,
                Situacion = situacion,
                Mensaje = mensaje,
                CiResidente = cedula,
                Fecha = DateTime.Now,
                Nombre= nombre
            };
            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/CrearAlerta?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, nuevaAlerta).Result;


            respuesta = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
                       
            
            if (response.IsSuccessStatusCode )
            {
                var residente = respuesta.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Residente>(residente);

                if (respuesta.IsSuccessStatusCode)
                {
                    try {
                        EnviarAlerta(objeto.unResponsable.Correo, nuevaAlerta.Mensaje, nuevaAlerta.Situacion, nuevaAlerta.Urgente);
                    }
                    catch
                    {
                        return Json(new { result = "error", mensaje = "No se ha podido enviar el mail" });
                    }

                    
                    return Json(new { result = "succes", mensaje = "Alerta creada con éxito." });
                }

                return Json(new { result = "error", mensaje = "No se ha podido enviar el mail" });
            }

            return Json(new { result = "error", mensaje = "No se pudo crear la alerta" });

        }


        [HttpGet]
        public ActionResult VerAlertas()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/GetAlertas?centro=" +
               _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;


            if (response.IsSuccessStatusCode)
            {
                var alertas = response.Content.ReadAsStringAsync().Result;
                var listaAlertas = JsonConvert.DeserializeObject<List<Alerta>>(alertas);

                return View(listaAlertas);
            }

            return View();

        }

        #endregion

        #region Auxiliares

        #region parseInsumos
        public ModeloInsumo ParseInsumoAModelo(Insumo i)
        {
            if (i == null) return null;
            return new ModeloInsumo
            {
                IdInsumo = i.IdInsumo,
                Descripcion = i.Descripcion,
                Stock = i.Stock,
                FechaIngreso = i.FechaIngreso.ToString("dd/MM/yyyy"),
                CiFuncionario = i.CiFuncionario,
                Tipo = (TipoInsumo)Enum.Parse(typeof(TipoInsumo), i.Tipo),
                TipoString = i.Tipo

            };
        }

        public Insumo ParseModeloAInsumo(ModeloInsumo i)
        {
            if (i == null) return null;
            return new Insumo
            {
                IdInsumo = i.IdInsumo,
                Descripcion = i.Descripcion,
                Stock = i.Stock,
                FechaIngreso = Convert.ToDateTime(i.FechaIngreso),
                CiFuncionario = i.CiFuncionario,
                Tipo = i.Tipo.ToString()
            };
        }
        #endregion

        #region parseConsulta
        public Consulta ParseModeloAConsulta(ModeloConsulta c)
        {
            if (c == null) return null;
            return new Consulta
            {
                Id = c.Id,
                Medico = c.Medico,
                Direccion = c.Direccion,
                Especialidad = c.Especialidad,
                FechaConsulta = Convert.ToDateTime(c.FechaConsulta)

            };
        }

        public ModeloConsulta ParseConsultaAModelo(Consulta c)
        {
            if (c == null) return null;
            return new ModeloConsulta
            {
                Id = c.Id,
                Medico = c.Medico,
                Direccion = c.Direccion,
                Especialidad = c.Especialidad,
                FechaConsulta = c.FechaConsulta.ToString("dd/MM/yyyy")

            };
        }
        #endregion

        #region parseEstudios
        public Estudio ParseModeloAEstudio(ModeloEstudio e)
        {
            if (e == null) return null;
            return new Estudio
            {
                IdEstudio = e.IdEstudio,
                Descripcion = e.Descripcion,
                FechaEstudio = Convert.ToDateTime(e.FechaEstudio),
                Direccion = e.Direccion,
                Especificaciones = e.Especificaciones,
                EstimadoResultado = Convert.ToDateTime(e.EstimadoResultado)
            };
        }

        public ModeloEstudio ParseEstudioAModelo(Estudio e)
        {
            if (e == null) return null;
            return new ModeloEstudio
            {
                IdEstudio = e.IdEstudio,
                Descripcion = e.Descripcion,
                FechaEstudio = e.FechaEstudio.ToString("dd/MM/yyyy"),
                Direccion = e.Direccion,
                Especificaciones = e.Especificaciones,
                EstimadoResultado = e.EstimadoResultado != null ? e.EstimadoResultado.Value.ToShortDateString() : ""

            };
        }
        #endregion

        #region parseTratamiento
        public Tratamiento ParseModeloATrat(ModeloTratamiento mod)
        {
            try
            {
                
                if (mod == null) return null;
                Tratamiento t = new Tratamiento
                {
                    IdTratamiento = mod.IdTratamiento,
                    CiFuncionario = mod.CiFuncionario,
                    FechaComienzo = string.IsNullOrEmpty(mod.FechaComienzo) ? DateTime.Now : Convert.ToDateTime(mod.FechaComienzo),
                    FechaFin = string.IsNullOrEmpty(mod.FechaFin) ? DateTime.Now.AddDays(7) : Convert.ToDateTime(mod.FechaFin),
                    FechaIngreso = string.IsNullOrEmpty(mod.FechaIngreso) ? DateTime.Now : Convert.ToDateTime(mod.FechaIngreso),
                    Descripcion = mod.Descripcion,
                    Medico = mod.Medico,
                    Recetas = new List<Receta>(),
                    Medicamentos = new List<Medicamento>()


                };

                if (mod.Recetas != null && mod.Recetas.Count() > 0)
                {
                    foreach (ModeloReceta r in mod.Recetas)
                    {

                        if (string.IsNullOrEmpty(r.FechaVencimiento)) { r.FechaVencimiento = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd"); }
                        else 
                        {
                            r.FechaVencimiento = Convert.ToDateTime(r.FechaVencimiento).ToString("yyyy-MM-dd");
                        }
                        if (string.IsNullOrEmpty(r.FechaEmision)) { r.FechaEmision = DateTime.Today.ToString("yyyy-MM-dd"); }
                        else 
                        {
                            r.FechaEmision = Convert.ToDateTime(r.FechaEmision).ToString("yyyy-MM-dd");
                        }
                        

                        t.Recetas.Add( ParseModeloAReceta(r));
                    }
                }
                if (mod.Medicamentos != null && mod.Medicamentos.Count() > 0)
                {
                    foreach (Medicamento m in mod.Medicamentos)
                    {
                        t.Medicamentos.Add(m);
                    }
                }

                return t;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            
            }
        }

        public ModeloTratamiento ParseTratamientoAModelo(Tratamiento trat)
        {
            if (trat == null) return null;
            ICollection<ModeloReceta> RecetasModelo = new List<ModeloReceta>();
            if (trat.Recetas.Count > 0)
            {
                foreach (Receta r in trat.Recetas)
                {
                    RecetasModelo.Add(ParseRecetaAModelo(r));
                }
            }
            ICollection<Medicamento> medicamentos = new List<Medicamento>();
            if (trat.Medicamentos.Count > 0)
            {
                foreach (Medicamento m in trat.Medicamentos)
                {
                    medicamentos.Add(m);
                }
            }
            return new ModeloTratamiento
            {
                IdTratamiento = trat.IdTratamiento,
                CiFuncionario = trat.CiFuncionario,
                CiResidente = trat.CiResidente,
                Medico = trat.Medico,
                Descripcion = trat.Descripcion,
                FechaComienzo = trat.FechaComienzo.HasValue ? trat.FechaComienzo.Value.ToShortDateString() : "",
                FechaFin = trat.FechaFin.HasValue ? trat.FechaFin.Value.ToShortDateString() : "",
                FechaIngreso = trat.FechaIngreso.HasValue ? trat.FechaComienzo.Value.ToShortDateString() : "",
                Recetas = RecetasModelo,
                Medicamentos = medicamentos
            };

        }
        #endregion

        #region parseRecetas
        public Receta ParseModeloAReceta(ModeloReceta mod)
        {
            if (mod == null) return null;
            
            return new Receta
            {
                IdReceta = mod.IdReceta,
                IdTratamiento = mod.IdTratamiento,
                FechaIngreso = DateTime.Now,
                FechaEmision = Convert.ToDateTime(mod.FechaEmision),
                FechaVencimiento = Convert.ToDateTime(mod.FechaVencimiento),
                Sustancia = mod.Sustancia,
                //Medicamento = mod.Medicamento,
                Dosis = mod.Dosis,
                Duracion = mod.Duracion,
                Frecuencia = mod.Frecuencia
            };
        }

        public ModeloReceta ParseRecetaAModelo(Receta rec)
        {
            if (rec == null) return null;
            return new ModeloReceta
            {
                IdReceta = rec.IdReceta,
                IdTratamiento = rec.IdTratamiento,
                Sustancia = rec.Sustancia,
                Frecuencia = rec.Frecuencia,
                Duracion = rec.Duracion,
                Dosis = rec.Dosis,
                FechaEmision = rec.FechaEmision.ToString("dd/MM/yyyy"),
                FechaVencimiento = rec.FechaVencimiento.ToString("dd/MM/yyyy")
            };

        }

        public ModeloReceta[] DeserializarRecetas(string recs)
        {
            string[] separados = recs.Split('|');

            ModeloReceta[] modelos = new ModeloReceta[separados.Length];

            for (int i = 0; i < separados.Length; i++)
            {
                ModeloReceta mod = JsonConvert.DeserializeObject<ModeloReceta>(separados[i]);
                modelos[i] = mod;
            }
            return modelos;
        }

        public double MaximaDuracion(List<ModeloReceta> recetas)
        {
            double max = 0;
            if (recetas == null || recetas.Count() == 0) return 0;
            foreach (ModeloReceta r in recetas)
            {
                if (r.Duracion > max) max = r.Duracion;
            }
            return max;
        }

        #endregion

        #region parseResidente
        public ModeloResidente ParseResidenteAModel(Residente res)
        {
            if (res == null) return null;
            ICollection<ModeloTratamiento> TratModelo = new List<ModeloTratamiento>();
            if (res.Tratamientos.Count > 0)
            {
                foreach (Tratamiento t in res.Tratamientos)
                {
                    TratModelo.Add(ParseTratamientoAModelo(t));
                }
            }
            return new ModeloResidente
            {
                Nombre = res.Nombre,
                Cedula = res.Cedula,
                Genero = res.Genero,
                ResponsableCI = res.unResponsable.Cedula,
                Alergias = res.Alergias,
                Dieta = res.Dieta,
                FechaNacimiento = res.FechaNacimiento.ToString("dd/MM/yyyy"),
                FechaEgreso = res.FechaEgreso != null ? res.FechaEgreso.Value.ToString("dd/MM/yyyy") : "",
                FechaIngreso = res.FechaIngreso.ToString("dd/MM/yyyy"),
                Tratamientos = TratModelo,
                Mutualista = res.Mutualista,
                ServicioFunebre = res.ServicioFunebre,
                Condiciones = res.Condiciones

            };
        }

        public ModeloResidenteActualizar ParseResidenteAModelActualizar(Residente res)
        {
            if (res == null) return null;
            ICollection<ModeloTratamiento> TratModelo = new List<ModeloTratamiento>();
            if (res.Tratamientos.Count > 0)
            {
                foreach (Tratamiento t in res.Tratamientos)
                {
                    TratModelo.Add(ParseTratamientoAModelo(t));
                }
            }
            return new ModeloResidenteActualizar
            {
                Nombre = res.Nombre,
                Cedula = res.Cedula,
                Genero = res.Genero,
                Dieta = res.Dieta,
                Alergias = res.Alergias,
                Condiciones = res.Condiciones,
                ResponsableCI = res.unResponsable.Cedula,
                FechaNacimiento = res.FechaNacimiento.ToString("yyyy-MM-dd"),
                Mutualista = res.Mutualista,
                ServicioFunebre = res.ServicioFunebre
            };
        }


        public ModeloResSignos ParseResidenteAModelSignos(Residente res)
        {
            if (res == null) return null;
            ICollection<ModeloSignoVital> sigModelo = new List<ModeloSignoVital>();
            if (res.SignosVitales.Count > 0)
            {
                foreach (SignoVital s in res.SignosVitales)
                {
                    sigModelo.Add(ParseSignoAModelo(s));
                }
            }
            return new ModeloResSignos
            {
                Nombre = res.Nombre,
                Cedula = res.Cedula,
                Genero = res.Genero,
                ResponsableCI = res.unResponsable.Cedula,
                SignosVitales = sigModelo
            };
        }

        private Residente ParseModeloARes(ModeloResidente mod, Usuario responsable)
        {
            return new Residente
            {
                Nombre = mod.Nombre,
                Cedula = mod.Cedula,
                Dieta = mod.Dieta,
                Alergias = mod.Alergias,
                FechaNacimiento = Convert.ToDateTime(mod.FechaNacimiento),
                Genero = mod.Genero,
                Mutualista = mod.Mutualista,
                ServicioFunebre = mod.ServicioFunebre,
                unResponsable = responsable
            };
        }

        #endregion

        #region parseUsuario
        public ModeloUsuario ParseUsuAModelo(Usuario usu)
        {
            if (usu == null) return null;

            return new ModeloUsuario
            {
                Nombre = usu.Nombre,
                Correo = usu.Correo,
                Password = usu.Password,
                Telefono = usu.Telefono,
                Tipos = usu.Tipos,
                Cedula = usu.Cedula
            };
        }

        private Usuario ParseViewModelAUsu(UsuarioParaResidente resp)
        {
            return new Usuario
            {
                Nombre = resp.Nombre,
                Cedula = resp.Cedula,
                Telefono = resp.Telefono,
                Correo = resp.Correo,
                fechaCreacion = resp.FechaCreacion,
                Centro = resp.Centro,
                Residentes = resp.Residentes
            };
        }
        #endregion

        #region parseSignoVital

        public ModeloSignoVital ParseSignoAModelo(SignoVital s)
        {
            if (s == null) return null;
            return new ModeloSignoVital
            {
                IdSignoVital = s.IdSignoVital,
                CiFuncionario = s.CiFuncionario,
                FechaRegistro = s.FechaRegistro.ToString("dd/MM/yyyy"),
                PresionMaxima = s.PresionMaxima == null ? null : s.PresionMaxima.ToString(),
                PresionMinima = s.PresionMinima == null ? null : s.PresionMinima.ToString(),
                Pulso = s.Pulso == null ? null : s.Pulso.ToString().Replace(",", "."),
                Temperatura = s.Temperatura == null ? null : s.Temperatura.ToString().Replace(",", "."),
                Azucar = s.Azucar == null ? null : s.Azucar.ToString().Replace(",", "."),
                Oxigeno = s.Oxigeno == null ? null : s.Oxigeno.ToString().Replace(",", "."),
                Comentario = s.Comentario
            };
        }

        public SignoVital ParseModeloASigno(ModeloSignoVital s)
        {
            if (s == null) return null;

            double PresionMaxima = double.Parse(string.IsNullOrEmpty(s.PresionMaxima) ? "0" : s.PresionMaxima.Replace(".", ","));
            double PresionMinima = double.Parse(string.IsNullOrEmpty(s.PresionMinima) ? "0" : s.PresionMinima.Replace(".", ","));
            double Pulso = double.Parse(string.IsNullOrEmpty(s.Pulso) ? "0" : s.Pulso.Replace(".", ","));
            double Temperatura = double.Parse(string.IsNullOrEmpty(s.Temperatura) ? "0" : s.Temperatura.Replace(".", ","));
            double Azucar = double.Parse(string.IsNullOrEmpty(s.Azucar) ? "0" : s.Azucar.Replace(".", ","));
            double Oxigeno = double.Parse(string.IsNullOrEmpty(s.Oxigeno) ? "0" : s.Oxigeno.Replace(".", ","));

            //return new SignoVital
            //{
            //    IdSignoVital = s.IdSignoVital,
            //    CiFuncionario = s.CiFuncionario,
            //    FechaRegistro = Convert.ToDateTime(s.FechaRegistro),
            //    PresionMaxima = double.Parse(s.PresionMaxima),
            //    PresionMinima = double.Parse(s.PresionMinima),
            //    Pulso = double.Parse(s.Pulso),
            //    Temperatura = double.Parse(s.Temperatura),
            //    Azucar = double.Parse(s.Azucar),
            //    Oxigeno = double.Parse(s.Oxigeno),
            //    Comentario = s.Comentario
            //};

            return new SignoVital
            {
                IdSignoVital = s.IdSignoVital,
                CiFuncionario = s.CiFuncionario,
                FechaRegistro = Convert.ToDateTime(s.FechaRegistro),
                PresionMaxima = PresionMaxima,
                PresionMinima = PresionMinima,
                Pulso = Pulso,
                Temperatura =Temperatura,
                Azucar = Azucar,
                Oxigeno = Oxigeno,
                Comentario = s.Comentario
            };
        }
        #endregion

        #region vistas

        public List<Residente> GetTodosGenerico()
        {
            var usuario = Logueado();
            response = cliente.GetAsync(usuarioUri + "/Residente/FindAll?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residentes = JsonConvert.DeserializeObject<IEnumerable<Residente>>(resJson);

                return residentes.ToList();

            }
            return null;
        }

        public List<ModeloResidenteActualizar> GetTodosParaActualizar()
        {
            List<ModeloResidenteActualizar> modResidentes = new List<ModeloResidenteActualizar>();
            List<Residente> todos = GetTodosGenerico();

            foreach (Residente r in todos)
            {
                modResidentes.Add(ParseResidenteAModelActualizar(r));
            }

            return modResidentes;
        }

        public List<ModeloResSignos> GetTodosParaSignos()
        {
            List<ModeloResSignos> modResidentes = new List<ModeloResSignos>();
            List<Residente> todos = GetTodosGenerico();
            foreach (Residente r in todos)
            {
                modResidentes.Add(ParseResidenteAModelSignos(r));
            }
            return modResidentes;
        }


        public List<Residente> GetResidentesPaginable(int indicePag = 1, int cantPorPag = 10)
        {
            List<Residente> todosRes = new List<Residente>();
            todosRes = GetTodosGenerico();

            ViewBag.CantidadResidentes = Math.Ceiling((decimal)todosRes.Count / cantPorPag);

            List<Residente> resPaginable = todosRes.Skip((indicePag - 1) * cantPorPag)
                .Take(cantPorPag).ToList();

            return resPaginable;
        }

        #endregion

        [HttpGet]
        public ActionResult BuscarResponsables()
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Usuario/UsuariosPorTipo?tipo=" + "Responsable"
                + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var listaJson = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(listaJson);
                var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(listaJson.ToString());

                if (usuarios.Count == 0) return Json(new { result = "error", mensaje = "No hay usuarios responsables registrados." });
                return Json(new { result = "success", data = usuarios });
            }
            return Json(new { result = "error", mensaje = "No se han encontrado usuarios responsables." });
        }

        public Usuario Logueado()
        {
            var usu = HttpContext.Session.GetString("Usuario");
            if (usu != null)
            {
                var usuario = JsonConvert.DeserializeObject<Usuario>(usu);
                //(actual == null || actual.Tipos.Count() == 1 && actual.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE")
                if (usuario == null || usuario.Tipos.Count() == 1 && usuario.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
                {
                    return null;
                }
                return usuario;
            }
            return null;
        }
        #endregion


        #region Auxiliar
        private void EnviarAlerta(string EmailDestino, string mensaje, string situacion, bool alerta)
        {
            try
            {
              
                string urlDomain = "https://proyectocalistoort.azurewebsites.net/";

                string EmailOrigen = "TF243515@fi365.ort.edu.uy";
                string Password = "ProyectoCalisto2022!";
                string asunto = "";
                string texto = "";

                if (alerta)
                {
                    asunto = "¡URGENTE! - " + _configuration["Centro"];
                    texto = "<h2 style='color:#650000'>URGENTE</h2>";
                }
                else 
                {
                    asunto = "Se ha emitido una alerta - " + _configuration["Centro"];
                }


                MailMessage OMailMessage = new MailMessage(EmailOrigen, EmailDestino, asunto,
                    texto +
                    "<h3>Situación: "+ situacion+ "</h3> <br>" +
                    "<p>" + mensaje + "</p>" +
                    "<p> Saludos: " + _configuration["Centro"] +"</p>");

                OMailMessage.IsBodyHtml = true;
                SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com");
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;

                oSmtpClient.Port = 587;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Password, "fi365.ort.edu.uy");
                oSmtpClient.Send(OMailMessage);
                oSmtpClient.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception("No se ha enviado el mail");
            }
        }

        #endregion

    }
}
