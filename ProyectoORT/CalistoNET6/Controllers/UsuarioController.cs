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

namespace CalistoNET6.Controllers
{
    public class UsuarioController : Controller
    {

        private HttpClient cliente = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();
        private HttpResponseMessage respuesta = new HttpResponseMessage();
        private Uri usuarioUri = null;
        private readonly IConfiguration _configuration;
        private static Sesion sesion = new Sesion();


        public UsuarioController(IConfiguration configuration)
        {
            cliente.BaseAddress = new Uri("https://proyectocalistoort.azurewebsites.net");
            usuarioUri = new Uri("https://proyectocalistoortapi.azurewebsites.net/api");
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            _configuration = configuration;


        }




        #region crear usuario

        /// <summary>
        /// // TEMPORAL!!!!
        /// </summary>
        [HttpGet]
        public ActionResult RegistrarUsuario()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            ViewBag.TipoUsu = ListaRoles(sesion.RolActivo(usuario));
            return View();
        }




        [HttpGet]
        public ActionResult RegistroDatos(string mensaje)
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            ViewBag.TipoUsu = ListaRoles(sesion.RolActivo(usuario));


            try
            {
                ViewBag.Mensaje = mensaje;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return RedirectToAction("Error", "Shared");
            }

        }

        [HttpPost]
        public ActionResult RegistroDatos(Usuario usu, List<string> tipos)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            usu.fechaCreacion = DateTime.Now;
            usu.GenerarPassword();

            if (tipos == null || tipos.Count == 0)
            {
                ViewBag.TipoUsu = ListaRoles(sesion.RolActivo(usuario));
                return Json(new { result = "error", mensaje = "Los datos ingresados no son correctos." });
            }

            foreach (string t in tipos)
            {
                var tipoU = t.Split('|').ToList();
                usu.Tipos.Add(new TipoUsuario() { Id = Int32.Parse(tipoU[0].ToString()), Tipo = tipoU[1] });
            }

            var usuModel = ParseUsuAModelo(usu);

            response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Registro?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, usuModel).Result;

            if (response.IsSuccessStatusCode)
            {
                Usuario usuarioRespuesta = response.Content.ReadAsAsync<Usuario>().Result;

                RecoveryViewModel usuRecovery = new RecoveryViewModel() { Cedula = usuarioRespuesta.Cedula };
                StartPassRecovery(usuRecovery);
                ViewBag.TipoUsu = ListaRoles(sesion.RolActivo(usuario));
                return Json(new { result = "success", mensaje = "Usuario ingresado con éxito. Se le ha enviado un correo." });

            }

            ViewBag.TipoUsu = ListaRoles(sesion.RolActivo(usuario));
            return Json(new { result = "error", mensaje = "Ha habido un error. Verifique que no exista un usuario con la misma cédula." });

        }



        [HttpGet]
        public ActionResult StartPassRecovery()
        {

            if (HttpContext.Session.GetString("Usuario") != null)
            {
                ViewBag.Mensaje = "Usted ya está ingresado en el sistema.";
                return RedirectToAction("Error", "Shared");
            }

            RecoveryViewModel modelo = new RecoveryViewModel();

            return PartialView("StartPassRecovery", modelo);
        }


        [HttpPost]
        public ActionResult StartPassRecovery(RecoveryViewModel modelo)
        {
            string mensaje;
            if (modelo != null)
            {
                try
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);

                    string token = Encriptar.GetSHA256(Guid.NewGuid().ToString());

                    response = cliente.GetAsync(usuarioUri + "/Usuario/BuscarParaRecuperar?cedula=" + modelo.Cedula + "&centro=" + _configuration["Centro"]).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var usuario = response.Content.ReadAsStringAsync().Result;
                        var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);
                        objeto.Residentes = null;

                        if (objeto != null)
                        {
                            var usuModel = ParseUsuAModelo(objeto);
                            usuModel.Token = token;
                            //Esto al mandar el usuario entero seria lo mismo que usar un metodo generico updateusuario
                            response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Token?centro=" + _configuration["Centro"], usuModel).Result;

                            if (response.IsSuccessStatusCode)
                            {
                                List<string> tiposUsu = new List<string>();
                                foreach (TipoUsuario t in objeto.Tipos) 
                                {
                                    tiposUsu.Add(t.Tipo);
                                
                                }

                                SendEmail(objeto.Correo,tiposUsu, token);
                                mensaje = "Se ha enviado un mail a " + usuModel.Correo;
                                //Ereturn Json(new { result = "success", mensaje = mensaje });
                                return RedirectToAction("LogIn", "Usuario", new { mensaje = mensaje });
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    return RedirectToAction("LogIn", "Usuario", new { mensaje = ex.Message });
                }

                mensaje = "Usuario no encontrado.";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = mensaje });

            }
            mensaje = "Debe ingresar una cédula";
            return RedirectToAction("LogIn", "Usuario", new { mensaje = mensaje });
        }
        #endregion

        #region login y cambio de contraseña
        [HttpGet]
        public ActionResult LogIn(string mensaje)
        {

            ViewBag.Mensaje = mensaje;
            ModeloLogin modelo = new ModeloLogin();
            return PartialView(modelo);

        }

        [HttpPost]
        public ActionResult LogIn(string cedula, string password)
        {
            ModeloLogin usu = new ModeloLogin();
            usu.Cedula = cedula;
            usu.Password = password;

            if (!TryValidateModel(usu))
            {
                ViewBag.Mensaje = "Las credenciales ingresadas no son válidas";
                return Json(new { result = "error", mensaje = "Las credenciales ingresadas no son válidas" });
            }

            response = cliente.GetAsync(usuarioUri + "/Usuario/LogIn?cedula=" + usu.Cedula + "&pass=" + usu.Password + "&centro=" + _configuration["Centro"]).Result;

            if (response.IsSuccessStatusCode)
            {
                var usuario = response.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                List<string> rol = objeto.DevolverRol().Split(Char.Parse("|")).ToList();

                if (objeto.DevolverRol().Contains("RESPONSABLE") && rol.Count == 1)
                {
                    
                   
                    return Json(new { result = "error", mensaje = "Los responsables deben ingresar a través de la aplicación <br> <br> <a href='https://proyectocalistoortpwa.azurewebsites.net'>Haga click aquí para ir al sitio</a>" });

                }
                else if (objeto.DevolverRol().Contains("RESPONSABLE") && rol.Count > 1)
                {
                    HttpContext.Session.SetString("Usuario", usuario);

                    return Json(new { result = "success"});
                }
                else
                {
                    HttpContext.Session.SetString("Usuario", usuario);
                   return Json(new { result = "success" });
                }
            }
            ViewBag.Mensaje = "Las credenciales ingresadas no son válidas";
            return Json(new { result = "error", mensaje = "Las credenciales ingresadas no son válidas" });
        }


        [HttpGet]
        public ActionResult PassRecovery(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            RecoveryPassViewModel modelo = new RecoveryPassViewModel();
            modelo.token = token;
            return View(modelo);
        }

        [HttpPost]
        public ActionResult PassRecovery(RecoveryPassViewModel modelo)
        {

            //if (HttpContext.Session.GetString("Usuario") != null)
            //{
            //    ViewBag.Mensaje = "Usuario no autorizado.";
            //    return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            //}


            if (!ModelState.IsValid)
            {
                //ViewBag.Mensaje = "Datos incorrectos.";
                return View(modelo);
            }

            if (String.Equals(modelo.Password, modelo.PasswordRepeat))
            {
                try
                {
                    ModeloUsuario usu = new ModeloUsuario() { Password = modelo.Password, Token = modelo.token };

                    response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/UpdatePassword?centro=" + _configuration["Centro"], usu).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //var usuario = response.Content.ReadAsAsync<String>().Result;
                        //var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                        var usuario = response.Content.ReadAsStringAsync().Result;
                        var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                        string mensaje = "La contraseña fue cambiada con éxito.";
                        return RedirectToAction("LogIn", "Usuario", new { mensaje = mensaje });
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Mensaje = "No fue posible cambiar la contraseña.";
                    return View();
                }

            }
            ViewBag.Mensaje = "No fue posible cambiar la contraseña";
            return View();
        }

        private void SendEmail(string EmailDestino,List<string> tipos, string token)
        {
            try
            {
                string urlDomain = "https://proyectocalistoort.azurewebsites.net/";

                string linksVideos = "";

                if (tipos.Contains("RESPONSABLE")) 
                {
                    linksVideos += "<br><p> Manual de usuario para el rol de responsable:" +
                        "<a href='https://youtu.be/Rq6U3DdJUco'> Ver video.</a> </p> <br>";
                }
                if (tipos.Contains("DIRECTOR"))
                {
                    linksVideos += "<br><p> Manual de usuario para el rol de director: " +
                        "<a href='https://youtu.be/W1HWXZ3qAaA'> Ver video.</a></p> <br>";
                }
                if (tipos.Contains("ADMINISTRADOR"))
                {
                    linksVideos += "<br><p> Manual de usuario para el rol de administrador:" +
                        "<a href='https://youtu.be/R_MFcHMa9xs'> Ver Video.</a> </p> <br>";
                }
                

                string EmailOrigen = "TF243515@fi365.ort.edu.uy";
                string Password = "ProyectoCalisto2022!";
                string url = urlDomain + "Usuario/PassRecovery?token=" + token;
                MailMessage OMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Cambio de contraseña",
                    "<h3>Correo para recuperación de contraseña</h3> <br>" +
                    "<p>Usted ha recibido este correo porque le han creado un usuario en el centro residencial" + _configuration["Centro"] +", o porque ha solicitado un cambio de contraseña.</p>" +
                    "<a href='" + url + "'> Haga click aquí para generar o cambiar su contraseña </a>" + linksVideos+
                    "<br><p> Saludos: " + _configuration["Centro"] + "</p>");

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
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region actualizar
        [HttpGet]
        public ActionResult Actualizar(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
            respuesta = cliente.GetAsync(usuarioUri + "/Usuario/TodosTipos?&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode && respuesta.IsSuccessStatusCode)
            {
                var u = response.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Usuario>(u);
                var objetoJson = respuesta.Content.ReadAsAsync<String>().Result;
                List<TipoUsuario> tipos = JsonConvert.DeserializeObject<List<TipoUsuario>>(objetoJson);

                ViewBag.TipoUsuYaTiene = objeto.Tipos;

                ViewBag.TiposUsuario = tipos;

                ViewBag.TipoUsu = ListaRoles(sesion.RolActivo(usuario));
                int posicion = ViewBag.TipoUsu.Count - 1;
                ViewBag.TipoUsuMayor = ViewBag.TipoUsu[posicion].Id;

                bool tieneResidentes = false;
                int i = 0;
                while(i < objeto.Tipos.Count && !tieneResidentes)
                {
                    if (ViewBag.TipoUsuYaTiene[i].Tipo.Equals("RESPONSABLE") && 
                        objeto.Residentes != null && objeto.Residentes.Count > 0)
                    {
                        tieneResidentes = true;
                    }
                    i++;
                }
                ViewBag.RespConResidentes = tieneResidentes;

                return PartialView("ActualizarUsuario", ParseUsuAModelo(objeto));
            }

            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult Actualizar(string cedula, string telefono, string nombre, string correo, List<string> tipos)
        {


            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }


            if (!String.IsNullOrEmpty(cedula) && !String.IsNullOrEmpty(telefono) && !String.IsNullOrEmpty(nombre) && !String.IsNullOrEmpty(correo))
            {
                Usuario usu = new Usuario
                {
                    Cedula = cedula,
                    Nombre = nombre,
                    Telefono = telefono,
                    Correo = correo
                };

                foreach (string t in tipos)
                {
                    var tipoU = t.Split('|').ToList();
                    usu.Tipos.Add(new TipoUsuario() { Id = Int32.Parse(tipoU[0].ToString()), Tipo = tipoU[1] });
                }

                response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Actualizar?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, usu).Result;

                if (response.IsSuccessStatusCode)
                {
                    var usuJson = response.Content.ReadAsStringAsync().Result;
                    Usuario actualizado = JsonConvert.DeserializeObject<Usuario>(usuJson);
                    if (usuario.Cedula == actualizado.Cedula) { HttpContext.Session.SetString("Usuario", usuJson); }
                    ViewBag.Mensaje = "Usuario actualizado con éxito.";
                    return Json(new { result = "success", mensaje = "El usuario fue actualizado con éxito." });
                }
            }
            return Json(new { result = "error", mensaje = "Ha habido un error." });
        }
        #endregion

        #region eliminar
        [HttpPost]
        public ActionResult BorrarUsuario(string cedula)
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Borrar?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi, cedula).Result;

            var respuesta = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {

                switch (respuesta)
                {
                    case "0":
                        return Json(new { result = "success", mensaje = "El usuario fue eliminado con éxito." });
                    case "1":
                        return Json(new { result = "error", mensaje = "El centro no puede quedar sin Director" });
                    case "2":
                        return Json(new { result = "error", mensaje = "El usuario esta asignado a un Residente activo" });
                    case "6":
                        return Json(new { result = "error", mensaje = "Comuniquese con el equipo de TI, ocurrió un error." });
                }
            }
            return Json(new { result = "error", mensaje = "Ocurrio un error inesperado" });

        }
        #endregion

        // GET: Usuario
        public ActionResult Index()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            //// traer ultimas alertas
            //response = cliente.GetAsync(usuarioUri + "/Residente/GetAlertas?centro=" +
            // _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;


            //if (response.IsSuccessStatusCode)
            //{
            //    var alertas = response.Content.ReadAsStringAsync().Result;
            //    var listaAlertas = JsonConvert.DeserializeObject<List<Alerta>>(alertas);
            //    listaAlertas = listaAlertas.OrderByDescending(x => x.Fecha).ToList();
            //    var listaFiltrada = listaAlertas.Take(10);
            //    ViewBag.Alertas = listaAlertas;
            //}


            //response = cliente.GetAsync(usuarioUri + "/Residente/FindAll?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var resJson = response.Content.ReadAsStringAsync().Result;
            //    var residentes = JsonConvert.DeserializeObject<IEnumerable<Residente>>(resJson);
            //    residentes = residentes.OrderByDescending(x => x.FechaIngreso).ToList();
            //    var listaResidentes = residentes.Take(4);
            //    List<Residente> nuevalista = new List<Residente>();
            //    foreach (Residente t in listaResidentes)
            //    {
            //        nuevalista.Add(t);
            //    }
            //    ViewBag.Residentes = nuevalista;

            //}



            // traer ultimos tratamientos
            //respuesta = cliente.GetAsync(usuarioUri + "/Residente/GetAllTratamientosGenerico?centro=" +
            //_configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            //if (respuesta.IsSuccessStatusCode)
            //{
            //    var modTraJson = respuesta.Content.ReadAsStringAsync().Result;
            //    var modTra = JsonConvert.DeserializeObject<List<Tratamiento>>(modTraJson);
            //    modTra = modTra.OrderByDescending(x => x.FechaIngreso).ToList();
            //    var modTraFiltrada = modTra.Take(10);
            //    List<Tratamiento> nuevalista = new List<Tratamiento>();
            //    foreach(Tratamiento t in modTraFiltrada)
            //    {
            //        nuevalista.Add(t);
            //    }

            //    ViewBag.Tratamientos = nuevalista;
            //}


            //        // traer ultimas consultas
            //        response = cliente.GetAsync(usuarioUri + "/Residente/GetAllEstudios?centro=" +
            //_configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            //        if (response.IsSuccessStatusCode)
            //        {
            //            var modEstJson = response.Content.ReadAsStringAsync().Result;
            //            var modEst = JsonConvert.DeserializeObject<List<Estudio>>(modEstJson);
            //            modEst = modEst.OrderByDescending(x => x.IdEstudio).ToList();
            //            var modEstFiltrada = modEst.Take(10);

            //            ViewBag.Estudios = modEstFiltrada;
            //        }

            //response = cliente.GetAsync(usuarioUri + "/Residente/GetAllConsultas?centro=" +
            //  _configuration["Centro"] +  "&token=" + usuario.TokenApi).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var modConJson = response.Content.ReadAsStringAsync().Result;
            //    var modCon = JsonConvert.DeserializeObject<List<Consulta>>(modConJson);

            //    modCon = modCon.OrderByDescending(x => x.Id).ToList();
            //    var modConFiltrada = modCon.Take(10);

            //    ViewBag.Consultas = modConFiltrada;
            //}

            return View();
        }


        [HttpGet]
        public ActionResult VistaUsuario()
        {

            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }
            
            // traer ultimas alertas
            response = cliente.GetAsync(usuarioUri + "/Residente/GetAlertas?centro=" +
             _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;


            if (response.IsSuccessStatusCode)
            {
                var alertas = response.Content.ReadAsStringAsync().Result;
                var listaAlertas = JsonConvert.DeserializeObject<List<Alerta>>(alertas);
                listaAlertas = listaAlertas.OrderByDescending(x => x.Fecha).ToList();
                var listaFiltrada = listaAlertas.Take(10);
                ViewBag.Alertas = listaAlertas;
              
            }


            response = cliente.GetAsync(usuarioUri + "/Residente/FindAll?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
            if (response.IsSuccessStatusCode)
            {
                var resJson = response.Content.ReadAsStringAsync().Result;
                var residentes = JsonConvert.DeserializeObject<IEnumerable<Residente>>(resJson);
                residentes = residentes.OrderByDescending(x => x.FechaIngreso).ToList();
                var listaResidentes = residentes.Take(4);
                List<Residente> nuevalista = new List<Residente>();
                foreach (Residente t in listaResidentes)
                {
                    nuevalista.Add(t);
                }
                ViewBag.Residentes = nuevalista;

            }
            return View();
        }



        [HttpGet]
        public ActionResult VerTodos()
        {
            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            ICollection<Usuario> todos = new List<Usuario>();

            string tipo = usuario.DevolverRol();
            try
            {
                if (tipo.Contains("DESARROLLADOR"))
                {
                    response = cliente.GetAsync(usuarioUri + "/Usuario/Todos?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var listaUsus = response.Content.ReadAsStringAsync().Result;
                        todos = JsonConvert.DeserializeObject<List<Usuario>>(listaUsus);
                    }
                }
                else if (tipo.Contains("DIRECTOR"))
                {

                    response = cliente.GetAsync(usuarioUri + "/Usuario/Todos?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var listaUsus = response.Content.ReadAsStringAsync().Result;
                        List<Usuario> sinFiltrar = JsonConvert.DeserializeObject<List<Usuario>>(listaUsus);

                        foreach (Usuario u in sinFiltrar)
                        {
                            if (!u.DevolverRol().Contains("DESARROLLADOR"))
                            {
                                todos.Add(u);
                            }
                        }
                    }
                }
                else if (tipo.Contains("ADMINISTRADOR"))
                {
                    response = cliente.GetAsync(usuarioUri + "/Usuario/Todos?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var listaUsus = response.Content.ReadAsStringAsync().Result;
                        List<Usuario> sinFiltrar = JsonConvert.DeserializeObject<List<Usuario>>(listaUsus);

                        foreach (Usuario u in sinFiltrar)
                        {
                            if (!u.DevolverRol().Contains("DESARROLLADOR") &&
                                !u.DevolverRol().Contains("DIRECTOR"))
                            {
                                todos.Add(u);
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.Mensaje = "Usuario no autorizado.";
                    return Redirect("LogIn");
                }

                return View(todos);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        [HttpGet]
        public ActionResult DetallesUsuario(string cedula)
        {


            var usuario = Logueado();
            if (usuario == null)
            {
                ViewBag.Mensaje = "No autorizado";
                return RedirectToAction("LogIn", "Usuario", new { mensaje = ViewBag.Mensaje });
            }

            response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + cedula + "&centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;

            if (response.IsSuccessStatusCode)
            {
                var usuarioRes = response.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Usuario>(usuarioRes);
                string tipos = "";
                foreach (TipoUsuario t in objeto.Tipos)
                {
                    tipos += t.Tipo + " | ";
                }
                tipos = tipos.Substring(0, tipos.Length - 2);
                ViewBag.TipoUsu = tipos;

                return PartialView("DetallesUsuario", ParseUsuAModelo(objeto));
            }

            return View("Error", "Shared");
        }



        public ActionResult Logout()
        {
            if (HttpContext.Session.GetString("Usuario") != null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("LogIn");
            }
            ViewBag.Message = "Usuario no autorizado";
            return RedirectToAction("Error", "Shared");
        }


        // para que funcione como se quiere hay que pasar en un string separado por | cada rol , todos tienen que ser IF nop else IF y no es == sino rol.contains("ADMIN")
        private List<TipoUsuario> ListaRoles(string rol)
        {
            rol = rol.ToUpper();


            List<TipoUsuario> roles = new List<TipoUsuario>();
            List<TipoUsuario> registrados = ObtenerTipos();
            if (rol.Contains("ADMINISTRADOR"))
            {
                roles.Add(registrados[3]);
            }

            if (rol.Contains("DIRECTOR"))
            {
                roles.Add(registrados[2]);
            }

            if (rol.Contains("DESARROLLADOR"))
            {
                roles.Add(registrados[0]);
                roles.Add(registrados[1]);
            }
            return roles;
        }


        #region auxiliares
        public List<TipoUsuario> ObtenerTipos()
        {
            var usuario = Logueado();
            response = cliente.GetAsync(usuarioUri + "/Usuario/TodosTipos?centro=" + _configuration["Centro"] + "&token=" + usuario.TokenApi).Result;
            if (response.IsSuccessStatusCode)
            {
                var objetoJson = response.Content.ReadAsAsync<String>().Result;
                List<TipoUsuario> tipos = JsonConvert.DeserializeObject<List<TipoUsuario>>(objetoJson);
                return tipos;
            }
            return null;
        }

        public ModeloUsuario ParseUsuAModelo(Usuario usu)
        {
            if (usu == null) return null;
            ModeloUsuario usuarioModelo = new ModeloUsuario
            {
                Nombre = usu.Nombre,
                Correo = usu.Correo,
                Password = usu.Password,
                Telefono = usu.Telefono,
                Tipos = usu.Tipos,
                Cedula = usu.Cedula,
                fechaCreacion = usu.fechaCreacion,
                Token = usu.Token,
                Centro = usu.Centro,
                Residentes = usu.Residentes
            };
            return usuarioModelo;
        }

        public Usuario ParseModAUsu(ModeloUsuario mod)
        {
            if (mod == null) return null;
            Usuario usuario = new Usuario
            {
                Nombre = mod.Nombre,
                Correo = mod.Correo,
                Password = mod.Password,
                Telefono = mod.Telefono,
                Tipos = mod.Tipos,
                Cedula = mod.Cedula,
                fechaCreacion = mod.fechaCreacion,
                Token = mod.Token,
                Centro = mod.Centro,
                Residentes = mod.Residentes
            };
            return usuario;
        }

        public Usuario Logueado()
        {
            var usu = HttpContext.Session.GetString("Usuario");
            if (usu != null)
            {
                var usuario = JsonConvert.DeserializeObject<Usuario>(usu);
                if (usuario == null || usuario.Tipos.Count() == 1 && usuario.Tipos.ToList()[0].ToString().ToUpper().Equals("RESPONSABLE"))
                {
                    return null;
                }
                return usuario;
            }
            return null;
        }
        #endregion
    }
}
