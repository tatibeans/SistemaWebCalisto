using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Dominio.Entidades;
using MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace MVC.Controllers
{
    public class UsuarioController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();
        private HttpResponseMessage respuesta = new HttpResponseMessage();
        private Uri usuarioUri = null;
        private static string nombreCentro = System.Configuration.ConfigurationManager.AppSettings["NombreCentro"];

        public UsuarioController()
        {
            
            cliente.BaseAddress = new Uri("https://localhost:44399");
            usuarioUri = new Uri("https://localhost:44399/api");
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }


       

        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogIn(string mensaje)
        {
            if (Session["Usuario"] != null)
            {
                Usuario logueado = (Usuario)Session["Usuario"];
                ViewBag.Mensaje = "Usted está ingresado como " + logueado.Nombre +
                    ". Si desea ingresar con otra cuenta, debe cerrar su sesión.";
                return RedirectToAction("../Shared/Error", "Home");
            }
            

            ViewBag.Mensaje = mensaje;
            ModeloLogin modelo = new ModeloLogin();
            return View(modelo);

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
                return Json(new { result = "error", mensaje = "Las credenciales ingresadas no son válidas" }, JsonRequestBehavior.AllowGet);
            }

            response = cliente.GetAsync(usuarioUri + "/Usuario/LogIn?cedula=" + usu.Cedula + "&pass=" + usu.Password + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                var usuario = response.Content.ReadAsAsync<String>().Result;
                var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                List<string> rol = objeto.DevolverRol().Split(Char.Parse("|")).ToList();

                if (objeto.DevolverRol().Contains("RESPONSABLE") && rol.Count ==1)                {

                    ViewBag.Mensaje = "Los responsables deben ingresar a través de la aplicación";
                    return Json(new { result = "error", mensaje = "Los responsables deben ingresar a través de la aplicación" }, JsonRequestBehavior.AllowGet);
                  
                }
                else if(objeto.DevolverRol().Contains("RESPONSABLE") && rol.Count > 1)
                {
                    Session["Usuario"] = objeto;
                    return Redirect("VistaUsuario");
                }
                else
                {
                    Session["Usuario"] = objeto;
                    return Redirect("VistaUsuario");
                }
            }
            ViewBag.Mensaje = "Las credenciales ingresadas no son válidas";
            return Json(new { result = "error", mensaje = "Las credenciales ingresadas no son válidas" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult VistaUsuario() 
        {
            Usuario u = (Usuario)Session["Usuario"];
            ViewBag.NombreUsu = u.Nombre;           
          
            return View();
        }

        [HttpGet]
        public ActionResult RegistroDatos(string mensaje)
        {

            //if (Session["Usuario"] == null || Sesion.RolActivo().ToString() == "Responsable")
            //{
            //    ViewBag.Mensaje = "Usuario no autorizado para registrar usuarios.";
            //    return Json(new { result = "error", mensaje = "Usuario no autorizado para registrar usuarios." }, JsonRequestBehavior.AllowGet);
            //}
            Usuario usu = new Usuario();
            ViewBag.TipoUsu = ListaRoles(Sesion.RolActivo());


            try
            {
                ViewBag.Mensaje = mensaje;
                return View(usu);
            } catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return RedirectToAction("Error", "Shared");
            }

        }

        [HttpPost]
        public ActionResult RegistroDatos(Usuario usu, List<string> tipos)
        {
            usu.fechaCreacion = DateTime.Now;
            usu.GenerarPassword();

            if (tipos == null || tipos.Count == 0)
            {
                ViewBag.Mensaje = "Los datos ingresados no son correctos";
                ViewBag.TipoUsu = ListaRoles(Sesion.RolActivo());
                return View("RegistroDatos", usu);
            }

            foreach (string t in tipos)
            {
                var tipoU = t.Split('|').ToList();
                usu.Tipos.Add(new TipoUsuario() { Id = Int32.Parse(tipoU[0].ToString()), Tipo = tipoU[1] });

            }


            if (!usu.ValidarDatos())
            {
                ViewBag.Mensaje = "Los datos ingresados no son correctos";
                ViewBag.TipoUsu = ListaRoles(Sesion.RolActivo());
                return View("RegistroDatos", usu);
            }

            var usuModel = ParseUsuAModelo(usu);

            response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Registro?centro=" + nombreCentro, usuModel).Result;

            if (response.IsSuccessStatusCode)
            {
                Usuario usuario = response.Content.ReadAsAsync<Usuario>().Result;

                Models.ViewModel.RecoveryViewModel usuRecovery = new Models.ViewModel.RecoveryViewModel() { Cedula = usuario.Cedula };
                StartPassRecovery(usuRecovery);

                string mensaje = "Se ha enviado un mail al usuario";
                return RedirectToAction("RegistroDatos", "Usuario", new { mensaje = mensaje });
            }

            ViewBag.Message = response.RequestMessage;
            return View("Error","Shared");
        }



        [HttpGet]
        public ActionResult StartPassRecovery()
        {
            if (Session["Usuario"] != null)
            {
                ViewBag.Mensaje = "Usted ya está ingresado en el sistema.";
                return RedirectToAction("Error", "Shared");
            }
          
            Models.ViewModel.RecoveryViewModel modelo = new Models.ViewModel.RecoveryViewModel();
            return View(modelo);
        }


        [HttpPost]
        public ActionResult StartPassRecovery(Models.ViewModel.RecoveryViewModel modelo)
        {
            try
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
                //if (!ModelState.IsValid)
                //{
                //    return View(modelo);
                //}

                string token = Encriptar.GetSHA256(Guid.NewGuid().ToString());

                response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + modelo.Cedula + "&centro=" + nombreCentro).Result;

                if (response.IsSuccessStatusCode)
                {
                    var usuario = response.Content.ReadAsStringAsync().Result;
                    var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                    if (objeto != null)
                    {
                        var usuModel = ParseUsuAModelo(objeto);
                        usuModel.Token = token;
                        //Esto al mandar el usuario entero seria lo mismo que usar un metodo generico updateusuario
                        response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Token?centro=" + nombreCentro, usuModel).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            SendEmail(objeto.Correo, token);
                            string mensaje = "Se ha enviado un mail a " + usuModel.Correo;                 
                            return RedirectToAction("LogIn","Usuario", new { mensaje = mensaje });
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            ViewBag.Mensaje = "Usuario no encontrado.";
            return View();
        }

        [HttpGet]
        public ActionResult PassRecovery(string token)
        {
            if (Session["Usuario"] != null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return RedirectToAction("Error", "Shared");
            } 
            Models.ViewModel.RecoveryPassViewModel modelo = new Models.ViewModel.RecoveryPassViewModel();
            modelo.token = token;
            return View(modelo);
        }

        [HttpPost]
        public ActionResult PassRecovery(Models.ViewModel.RecoveryPassViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Mensaje = "Datos incorrectos.";
                return View(modelo);
            }

            if (String.Equals(modelo.Password, modelo.PasswordRepeat))
            {
                try
                {
                    ModeloUsuario usu = new ModeloUsuario() { Password = modelo.Password, Token = modelo.token };

                    response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/UpdatePassword?centro=" + nombreCentro, usu).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var usuario = response.Content.ReadAsAsync<String>().Result;
                        var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);
                    
                        string mensaje= "La contraseña fue cambiada con éxito.";
                        return RedirectToAction("LogIn", "Usuario", new {mensaje = mensaje });
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.Mensaje = "No fue posible cambiar la contraseña.";
                    return View();
                }

            }
            ViewBag.Mensaje = "Las contraseñas no coinciden. Inténtelo nuevamente.";
            return View();
        }



        [HttpGet]
        public ActionResult VerTodos()
        {
            if (Session["Usuario"] == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return Redirect("LogIn");
            }

            ICollection<Usuario> todos = new List<Usuario>();
            Usuario activo = (Usuario)Session["Usuario"];
            Centro actual = (Centro)Session["Centro"];
            string tipo = activo.DevolverRol();
            try
            {
                if (tipo.Contains("DESARROLLADOR"))
                {
                    response = cliente.GetAsync(usuarioUri + "/Usuario/Todos?centro=" + nombreCentro).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var listaUsus = response.Content.ReadAsStringAsync().Result;
                        todos = JsonConvert.DeserializeObject<List<Usuario>>(listaUsus);
                    }
                }
                else if (tipo.Contains("DIRECTOR"))
                {

                    response = cliente.GetAsync(usuarioUri + "/Usuario/Todos?centro=" + nombreCentro).Result;

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
                    response = cliente.GetAsync(usuarioUri + "/Usuario/Todos?centro=" + nombreCentro).Result;

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
        public ActionResult Actualizar(string cedula)
        {
            if (Session["Usuario"] == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return Redirect("LogIn");       
            }
            response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + cedula + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                var usuario = response.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                List<TipoUsuario> tipos = ListaRoles(Sesion.RolActivo());
                string listaTipos = "";
                for (int i = 0; i < tipos.Count; i++)
                {
                    if (i != 0) listaTipos += ", ";
                    listaTipos += tipos[i].Tipo;
                }

                ViewBag.TipoUsu = listaTipos;

                return PartialView("ActualizarUsuario", ParseUsuAModelo(objeto));
            }

            return View("Error", "Shared");
        }

        [HttpGet]
        public ActionResult DetallesUsuario(string cedula)
        {
            if (Session["Usuario"] == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return Redirect("LogIn");
            }
            response = cliente.GetAsync(usuarioUri + "/Usuario/Buscar?cedula=" + cedula + "&centro=" + nombreCentro).Result;

            if (response.IsSuccessStatusCode)
            {
                var usuario = response.Content.ReadAsStringAsync().Result;
                var objeto = JsonConvert.DeserializeObject<Usuario>(usuario);

                ViewBag.TipoUsu = ListaRoles(Sesion.RolActivo());

                return PartialView("DetallesUsuario", ParseUsuAModelo(objeto));
            }

            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult BorrarUsuario(string cedula)
        {
            if (Session["Usuario"] == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return Redirect("LogIn");
            }
            response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Borrar?centro=" + nombreCentro, cedula).Result;

            var respuesta = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)            
            {           
                return Json(new { result = "success", mensaje = "El usuario fue eliminado con éxito."}, JsonRequestBehavior.AllowGet);
            }

            switch (respuesta)
            {
                case "1":
                    return Json(new { result = "error", mensaje = "El centro no puede quedar sin Director" }, JsonRequestBehavior.AllowGet);                  
                case "2":
                    return Json(new { result = "error", mensaje = "El usuario esta asignado a un Residente activo" }, JsonRequestBehavior.AllowGet);
              
            }


            return View("Error", "Shared");
        }

        [HttpPost]
        public ActionResult Actualizar(string cedula, string telefono, string nombre, string correo)
        {
            if (Session["Usuario"] == null)
            {
                ViewBag.Mensaje = "Usuario no autorizado.";
                return Redirect("LogIn");
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
                response = cliente.PostAsJsonAsync(usuarioUri + "/Usuario/Actualizar?centro=" + nombreCentro, usu).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    Usuario u = (Usuario)Session["Usuario"];
                    var usuJson = response.Content.ReadAsStringAsync().Result;
                    Usuario actualizado = JsonConvert.DeserializeObject<Usuario>(usuJson);
                    if (u.Cedula == actualizado.Cedula) { Session["Usuario"] = actualizado; }
                    ViewBag.Mensaje = "Usuario actualizado con éxito.";
                    return Json(new { result = "success", mensaje = "El usuario fue actualizado con éxito." }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = "error", mensaje = "Ha habido un error." }, JsonRequestBehavior.AllowGet);
        }

        


        public ActionResult Logout()
        {
            if (Session["Usuario"] != null)
            {
                Session.Remove("Usuario");
                return View("LogIn");
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
            if (rol.Contains( "ADMINISTRADOR"))
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


        private void SendEmail(string EmailDestino, string token)
        {
            try
            {
                //Esta ruta hay que cambiarla por la de azure
                string urlDomain = "https://localhost:44341/";
               
                string EmailOrigen = "ProyectoCalisto@outlook.com";
                string Password = "Calisto2022!";
                string url = urlDomain + "Usuario/PassRecovery?token=" + token;
                MailMessage OMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Cambio de contraseña",
                    "<h3>Correo para recuperación de contraseña</h3> <br>" +
                    "<p>Usted ha recibido este correo porque ha solicitado un cambio de contraseña en el sistema Calisto.</p>" +
                    "<a href='" + url + "'> Haga click aquí para cambiar su contraseña </a>");

                OMailMessage.IsBodyHtml = true;
                SmtpClient oSmtpClient = new SmtpClient("smtp.office365.com");
                oSmtpClient.EnableSsl = true;
                oSmtpClient.UseDefaultCredentials = false;

                oSmtpClient.Port = 25;
                oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Password);
                oSmtpClient.Send(OMailMessage);
                oSmtpClient.Dispose();
            } catch (Exception ex)
            {

            }
        }


        public List<TipoUsuario> ObtenerTipos()
        {
            response = cliente.GetAsync(usuarioUri + "/Usuario/TodosTipos?centro=" + nombreCentro).Result;
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
    }
}
