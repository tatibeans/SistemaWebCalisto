using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using Dominio.Entidades;
using MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MVC.Models.ViewModel;

namespace MVC.Controllers
{
    public class ResidenteController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();
        private HttpResponseMessage respuesta = new HttpResponseMessage();
        private Uri usuarioUri = null;
        private static string nombreCentro = System.Configuration.ConfigurationManager.AppSettings["NombreCentro"];


        public ResidenteController()
        {
            cliente.BaseAddress = new Uri("https://localhost:44399");
            usuarioUri = new Uri("https://localhost:44399/api");
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [HttpGet]
        public ActionResult DetallesResidente(string cedula)
        {
            Usuario actual = (Usuario)Session["Usuario"];
            if (actual == null || actual.Tipos.Count() == 1 && actual.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario");
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + nombreCentro).Result;

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
            if (Session["Usuario"] == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return Redirect("LogIn");
            }
            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/Borrar?centro=" + nombreCentro, cedula).Result;

            var respuesta = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                return Json(new { result = "success", mensaje = "El residente fue eliminado con éxito." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = "error", mensaje = "No se pudo elimiar el residente" }, JsonRequestBehavior.AllowGet);

            }          
        }

        // GET: Residente
        public ActionResult IngresoResidente()
        {
            Usuario activo = (Usuario)Session["Usuario"];
            if (activo != null && !(activo.Tipos.Count == 1 && activo.Tipos.First().ToString().ToUpper().Equals("RESPONSABLE"))){
                return View();
            }
            ViewBag.Mensaje = "Usuario no autorizado. El responsable debe ingresar por la aplicación.";
            return RedirectToAction("LogIn", "Usuario");
        }

        [HttpGet]
        public ActionResult InicioAgregarSigno()
        {
            Usuario activo = (Usuario)Session["Usuario"];
            if (activo != null && !(activo.Tipos.Count == 1 && activo.Tipos.First().ToString().ToUpper().Equals("RESPONSABLE")))
            {
                List<ModeloResSignos> todos = GetTodosParaSignos();
                return View(todos);
            }
            ViewBag.Mensaje = "Usuario no autorizado. El responsable debe ingresar por la aplicación.";
            return RedirectToAction("LogIn", "Usuario");
        }


        [HttpPost]
        public ActionResult IngresoResidente (FormCollection form)
        {            
            string alergias = form["listaAlergias"];
            string condiciones = form["listaCondiciones"];
            string nom = form["Nombre"];
            string ci = form["Cedula"];
            string gen = form["Genero"];
            string fechaString = form["FechaNacimiento"];
            string mut = form["Mutualista"];
            string dieta = form["Dieta"];
            string serv = form["ServicioFunebre"];
            string respCI = form["ResponsableCi"];



            DateTime fecha = DateTime.Parse(fechaString);
            Usuario resp = null;
            response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + respCI + "&centro=" + nombreCentro).Result;
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "No se encontró el usuario responsable con esa cédula.";
                return View();
            }
            var usuJson = response.Content.ReadAsStringAsync().Result;
            resp = JsonConvert.DeserializeObject<Usuario>(usuJson);

            response = cliente.GetAsync(usuarioUri + "/Centro/BuscarCentro?contexto=" + nombreCentro).Result;

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "Hubo un error. Contactar al equipo de IT.";
                return View();
            }
            var centroJson = response.Content.ReadAsAsync<String>().Result;
            Centro cen = JsonConvert.DeserializeObject<Centro>(centroJson);

            Residente res = new Residente
            {
                Nombre = nom,
                Cedula = ci,
                Genero = gen,
                Mutualista = mut,
                Dieta = dieta,
                ServicioFunebre = serv,
                FechaNacimiento = fecha,
                Condiciones = condiciones,
                Alergias = alergias,
                FechaIngreso = DateTime.Now,
                unResponsable = new Usuario() {Cedula = resp.Cedula },
                Centro = new Centro() {Rut = cen.Rut },
                FechaEgreso = null
            };
          
          

            response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/Ingresar?centro=" + nombreCentro, res).Result;

            if (response.IsSuccessStatusCode)
            {              
                ViewBag.Mensaje = "El residente fue ingresado con éxito.";
            }
            ViewBag.Mensaje = "No se pudo ingresar el residente.";
            return Redirect("Index");

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

        [HttpGet]
        public ActionResult ActualizarResidente(string cedula)
        {
            Usuario actual = (Usuario)Session["Usuario"];
            if (actual == null || actual.Tipos.Count() == 1 && actual.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario");
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);

                return PartialView("EditarResidente", ParseResidenteAModelActualizar(residente));
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult ActualizarResidente(ModeloResidenteActualizar mod,string listaAlergias ="", string listaCondiciones="")
        {
            mod.Condiciones = listaCondiciones;
            mod.Alergias = listaAlergias;
            //if (!ModelState.IsValid) return View();

            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + mod.Cedula + "&centro=" + nombreCentro).Result;
          
            if (response.IsSuccessStatusCode)
            {
                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/Actualizar?centro=" + nombreCentro, mod).Result;
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Datos actualizados con éxito.";
                    // falta mostrar el mensaje de que todo salio pipi cucu
                    return Redirect("Index");
                }
            }

            ViewBag.Mensaje = "Datos incorrectos.";
            //verificar si anda bien la vista error
            return PartialView("EditarResidente",mod);
        }

        
        [HttpGet]
        public ActionResult Index()
        {
            Usuario actual = (Usuario)Session["Usuario"];
            if (actual == null || actual.Tipos.Count() == 1 && actual.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario");
            }
            List<ModeloResidenteActualizar> todos = GetTodosParaActualizar();
            return View(todos);
        }


        [HttpGet]
        public ActionResult InformacionExtra(string ci)
        {
            Usuario actual = (Usuario)Session["Usuario"];
            if (actual == null || actual.Tipos.Count() == 1 && actual.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario");
            }

            response = cliente.GetAsync(usuarioUri + "/Residente/GetSignos?centro=" +
                nombreCentro + "&cedula=" + ci).Result;

            if (response.IsSuccessStatusCode)
            {
                var signosJson = response.Content.ReadAsStringAsync().Result;
                var signos = JsonConvert.DeserializeObject<List<SignoVital>>(signosJson);

                List<ModeloSignoVital> modSignos = new List<ModeloSignoVital>();
                foreach (SignoVital s in signos)
                {
                    ModeloSignoVital m = ParseSignoAModelo(s);
                    modSignos.Add(m);
                }

                return View(modSignos);
            }

            return Json(new { result = "error", mensaje = "Ha habido un error." }, JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        public ActionResult EditarSigno(string idSig)
        {
            Usuario actual = (Usuario)Session["Usuario"];
            if (actual == null || actual.Tipos.Count() == 1 && actual.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario");
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/GetSigno?id=" + idSig + "&centro=" + nombreCentro).Result;

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
            response = cliente.GetAsync(usuarioUri + "/Residente/GetSigno?id=" + mod.IdSignoVital + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/EditarSigno?centro=" + nombreCentro, mod).Result;
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Datos actualizados con éxito.";
                    // falta mostrar el mensaje de que todo salio pipi cucu
                    return Redirect("InformacionExtra");
                }
            }

            ViewBag.Mensaje = "Datos incorrectos.";
            //verificar si anda bien la vista error
            return PartialView("EditarResidente", mod);
        }


        [HttpGet]
        public ActionResult ActualizaRes(int indicePag = 1, int cantidadRes = 10)
        {
            try
            {
                List<Residente> listaRes = GetResidentesPaginable(indicePag,cantidadRes);
                List<ModeloResidenteActualizar> listaFormato = new List<ModeloResidenteActualizar>();
                foreach (Residente r in listaRes)
                {
                    listaFormato.Add(ParseResidenteAModelActualizar(r));
                }
                return PartialView("TablaResidentes",listaFormato);

            } catch (Exception ex)
            {
                return View(new List<ModeloResidente>());
            }
            
        }

        [HttpGet]
        public ActionResult AgregaSignoRes(int indicePag = 1, int cantidadRes = 10)
        {
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

        


        [HttpGet]
        public ActionResult BuscarResponsables()
        {
            if (Session["Usuario"] == null) return RedirectToAction("Error", "Shared");
            Usuario u = (Usuario)Session["Usuario"];
            if (u.Tipos.Count == 1 && u.Tipos.First().ToString() == "RESPONSABLE")
                return RedirectToAction("Error", "Shared");

            response = cliente.GetAsync(usuarioUri + "/Usuario/UsuariosPorTipo?tipo=" + "Responsable"
                + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                var listaJson = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(listaJson);
                var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(listaJson.ToString());

                if (usuarios.Count == 0) return Json(new { result = "error", mensaje = "No hay usuarios responsables registrados." }, JsonRequestBehavior.AllowGet);
                return Json(new { result = "success", data = usuarios }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "error", mensaje = "No se han encontrado usuarios responsables." }, JsonRequestBehavior.AllowGet);
        }


        public List<Residente> GetTodosGenerico()
        {
            response = cliente.GetAsync(usuarioUri + "/Residente/FindAll?centro=" + nombreCentro).Result;
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

        [HttpGet]
        public ActionResult AgregarSigno(string cedula)
        {
            Usuario activo = (Usuario)Session["Usuario"];
            if (activo == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario");
            }
            response = cliente.GetAsync(usuarioUri + "/Residente/Buscar?cedula=" + cedula + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residente = JsonConvert.DeserializeObject<Residente>(resJson);
                ModeloSignoVital modSig = new ModeloSignoVital
                {
                    CiFuncionario = activo.Cedula,
                    CiResidente = residente.Cedula
                };
                return PartialView("AgregarSigno", modSig);
            }

            // poner json
            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult AgregarSigno(ModeloSignoVital mod)
        {            // fijarle el funcionario, datetime, parsear presion
            if (mod != null)
            {

                Usuario usu = (Usuario)Session["Usuario"];
                mod.FechaRegistro = DateTime.Now;
                mod.CiFuncionario = usu.Cedula;

                SignoVital signo = ParseModeloASigno(mod);                     

                response = cliente.PostAsJsonAsync(usuarioUri + "/Residente/IngresarSignos?centro=" + nombreCentro + "&cedula=" + mod.CiResidente, signo).Result;

                if (response.IsSuccessStatusCode)
                {
                   // mostrar modal de esiiito
                }


                return null;
            }
            return null;
        }

        public ModeloResidente ParseResidenteAModel(Residente res)
        {
            if (res == null) return null;
            ICollection<ModeloTratamiento> TratModelo = new List<ModeloTratamiento>();
            if(res.Tratamientos.Count > 0)
            {
                foreach(Tratamiento t in res.Tratamientos)
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
                FechaNacimiento = res.FechaNacimiento,
                FechaEgreso = res.FechaEgreso,
                FechaIngreso = res.FechaIngreso,
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
                FechaNacimiento = res.FechaNacimiento,             
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

        public ModeloSignoVital ParseSignoAModelo(SignoVital s)
        {
            if (s == null) return null;
            return new ModeloSignoVital
            {
                IdSignoVital = s.IdSignoVital,
                FechaRegistro = s.FechaRegistro,
                PresionMaxima = s.PresionMaxima.ToString(),
                PresionMinima = s.PresionMinima.ToString(),
                Pulso = s.Pulso.ToString(),
                Temperatura = s.Temperatura.ToString(),
                Azucar =s.Azucar.ToString(),
                Oxigeno = s.Oxigeno.ToString(),
                Comentario = s.Comentario
            };
        }

        public SignoVital ParseModeloASigno(ModeloSignoVital s)
        {
            if (s == null) return null;
            return new SignoVital
            {
                IdSignoVital = s.IdSignoVital,
                CiFuncionario = s.CiFuncionario,
                FechaRegistro = s.FechaRegistro,
                PresionMaxima = double.Parse(string.IsNullOrEmpty(s.PresionMaxima) ? "0" : s.PresionMaxima.Replace(".", ",")),                
                PresionMinima = double.Parse(string.IsNullOrEmpty(s.PresionMinima) ? "0" : s.PresionMinima.Replace(".", ",")),
                Pulso = double.Parse(string.IsNullOrEmpty(s.Pulso) ? "0" : s.Pulso.Replace(".", ",")),
                Temperatura = double.Parse(string.IsNullOrEmpty(s.Temperatura) ? "0" : s.Temperatura.Replace(".", ",")),
                Azucar = double.Parse(string.IsNullOrEmpty(s.Azucar) ? "0" : s.Azucar.Replace(".", ",")),
                Oxigeno = double.Parse(string.IsNullOrEmpty(s.Oxigeno) ? "0" : s.Oxigeno.Replace(".", ",")),
                Comentario = s.Comentario
            };
        }

        public ModeloTratamiento ParseTratamientoAModelo(Tratamiento trat)
        {
            if (trat == null) return null;
            ICollection<ModeloReceta> RecetasModelo = new List<ModeloReceta>();
            if (trat.Recetas.Count > 0)
            {
                foreach(Receta r in trat.Recetas)
                {
                    RecetasModelo.Add(ParseRecetaAModelo(r));
                }
            }
            return new ModeloTratamiento
            {
                Medico = trat.Medico,
                Descripcion = trat.Descripcion,
                //Dosis = trat.Dosis,
                //Duracion = trat.Duracion,
                //Frecuencia = trat.Frecuencia,
                Recetas = RecetasModelo
            };

        }

        public ModeloReceta ParseRecetaAModelo(Receta rec)
        {
            if (rec == null) return null;
            return new ModeloReceta
            {
                IdTratamiento = rec.IdTratamiento,
                //Medicamento = rec.Medicamento,
                FechaEmision = rec.FechaEmision,
                FechaVencimiento = rec.FechaVencimiento
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
                FechaNacimiento = mod.FechaNacimiento,
                Genero = mod.Genero,
                Mutualista = mod.Mutualista,
                ServicioFunebre = mod.ServicioFunebre,
                unResponsable = responsable
            };
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

    }
}
