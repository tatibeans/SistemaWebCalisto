using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Entidades;
using System.Net.Http;
using System.Net.Http.Headers;
using MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace MVC.Controllers
{
    public class CentroController : Controller
    {
        private HttpClient cliente = new HttpClient();
        private HttpResponseMessage response = new HttpResponseMessage();
        private HttpResponseMessage respuesta = new HttpResponseMessage();
        private Uri centroUri = null;


        public CentroController()
        {
            cliente.BaseAddress = new Uri("https://drcatfapi.azurewebsites.net");
            centroUri = new Uri("http://drcatfapi.azurewebsites.net/api");
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult AgregarDirector(string id) 
        {
            try
            {
                //Repositorios.Repositorios.RepositorioCentro repo = new Repositorios.Repositorios.RepositorioCentro();
                //var centro = repo.FindById(id);
                ViewBag.rut = id;

                return View("AgregarDirector"/*, centro*/);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult AgregarDirector(string rutCentro, string idDirector)
        {
            try
            {
                //Repositorios.Repositorios.RepositorioCentro repo = new Repositorios.Repositorios.RepositorioCentro();
                //var lista = repo.FindById(rutCentro);
               
                return PartialView("AgregarDirector");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       


        // GET: Centro
        public ActionResult Index()
        {
            return View();
        }

        // GET: Centro/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Todos()
        {
            Usuario logueado = (Usuario)Session["Usuario"];
            if (logueado != null && logueado.Tipos.ToString().ToUpper() == "DESARROLLADOR")
            {
                try
                {
                    // traer todos los centos
                    response = cliente.GetAsync(centroUri + "/Centro/Todos").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var usuario = response.Content.ReadAsAsync<String>().Result;
                        var objeto = JsonConvert.DeserializeObject<IEnumerable<ModeloCentro>>(usuario);

                        return View(objeto);
                    }

                }
                catch (Exception ex)
                {
                    return View();

                }
            }
            ViewBag.Mensaje = "Usuario no autorizado";
            return View("../Shared/Error", "Home");
        }


        // GET: Centro/Create
        public ActionResult SegundoCrearCentro(string mensaje)
        {
            ViewBag.Mensaje = mensaje;
            return View();
        }

        [HttpPost]
        public ActionResult SegundoCrearCentro(ModeloIngresoCentro modCentro)
        {

            if (!ModelState.IsValid) return View("Index");
            int rut;
            bool soloNumeros = int.TryParse(modCentro.Rut, out rut);
            if (!soloNumeros)
            {
                ViewBag.Mensaje = "El RUT solo puede contener números.";
                return View(modCentro);
            }
            try
            {
                // voy a buscar si hay algún centro registrado con ese RUT
                response = cliente.GetAsync(centroUri + "/Centro/BuscarCentro?rut=" + modCentro.Rut).Result;

                if (response.IsSuccessStatusCode)
                {
                    string mensaje = "Ya existe un centro residencial registrado con ese RUT.";
                    return RedirectToAction("SegundoCrearCentro", "Centro", new { mensaje = mensaje });
                }

                //Si el centro no existe lo agrego
                response = cliente.PostAsJsonAsync(centroUri + "/Centro/CrearCentro", modCentro).Result;

                if (response.IsSuccessStatusCode)
                {
                    Centro agregado = response.Content.ReadAsAsync<Centro>().Result;
                    string mensaje = "Centro Creado" + agregado.Nombre;
                    return RedirectToAction("SegundoCrearCentro", "Centro", new { mensaje = mensaje });
                }

                ViewBag.Mensaje = "No se pudo agregar el centro: " + response.ReasonPhrase;
                return RedirectToAction("Index");

                /* //// hago un método en la api que reciba muchos usuarios y los guarde todos o no guarde ninguno
                 //// IMPORTANTE!!!! HAY QUE DEVOLVER LA LISTA DE DIRECTORES CON LOS USUARIOS ENTEROS
                 //response = cliente.PostAsJsonAsync(centroUri + "Usuario/RegistroMultiple", modCentro.Directores).Result;
                 //var directores = response.Content.ReadAsAsync<ICollection<ModeloUsuario>>().Result;

                 //// limpio todos los directores que vinieron desde la vista
                 //modCentro.Directores.Clear();

                 //foreach (ModeloUsuario usu in directores)
                 //{
                 //    modCentro.Directores.Add(usu);
                 //}*/

                // si los usuarios existen o se pudieron registrar, guardo el centro


            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
                return View("../Shared/Error", "Home");
            }
        }

        //private ModeloCentro ParseModIngresoAModCentro(ModeloIngresoCentro modInCentro, ICollection<ModeloUsuario> directores)
        //{
        //    ModeloCentro centro = new ModeloCentro
        //    {
        //        Nombre = modInCentro.Nombre,
        //        Direccion = modInCentro.Direccion,
        //        Telefono = modInCentro.Telefono,
        //        Correo = modInCentro.Correo,
        //        OperativoDesde = modInCentro.OperativoDesde,
        //        OperativoHasta = null,
        //        Rut = modInCentro.Rut
        //    };

        //    //foreach (ModeloUsuario usu in directores)
        //    //{
        //    //    centro.Directores.Add(usu);
        //    //}

        //    return centro;
        //}

        //private ModeloCentro ParseCentroAModelo(Centro centro)
        //{
        //    return new ModeloCentro
        //    {
        //        Nombre = centro.Nombre,
        //        Direccion = centro.Direccion,
        //        Telefono = centro.Telefono,
        //        Correo = centro.Correo,
        //        OperativoDesde = centro.OperativoDesde,
        //        OperativoHasta = centro.OperativoHasta,
        //        Rut = centro.Rut
        //    };


        //}

        // GET: Centro/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Centro/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Centro/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Centro/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
