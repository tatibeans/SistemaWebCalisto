//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Net.Mail;
//using System.Web;
//using Dominio.Entidades;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Session;
//using System.Configuration;
//using CalistoNET6.Models;
//using CalistoNET6.Models.ViewModel;
//using System.Text.Json;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using Microsoft.Extensions.Configuration;

//namespace CalistoNET6.Controllers
//{
    //public class CentroController : Controller
    //{
    //    private HttpClient cliente = new HttpClient();
    //    private HttpResponseMessage response = new HttpResponseMessage();
    //    private HttpResponseMessage respuesta = new HttpResponseMessage();
    //    private Uri centroUri = null;
    //    private readonly IConfiguration _configuration;
    //    private CalistoNET6.Controllers.Sesion sesion = new CalistoNET6.Controllers.Sesion();


    //    public CentroController(IConfiguration configuration)
    //    {
    //        cliente.BaseAddress = new Uri("https://drcatfapi.azurewebsites.net");
    //        centroUri = new Uri("http://drcatfapi.azurewebsites.net/api");
    //        cliente.DefaultRequestHeaders.Accept.Clear();
    //        cliente.DefaultRequestHeaders.Accept.Add(
    //        new MediaTypeWithQualityHeaderValue("application/json"));
    //        _configuration = configuration;
    //    }

    //    public ActionResult AgregarDirector(string id) 
    //    {
    //        try
    //        {
    //            //Repositorios.Repositorios.RepositorioCentro repo = new Repositorios.Repositorios.RepositorioCentro();
    //            //var centro = repo.FindById(id);
    //            ViewBag.rut = id;

    //            return View("AgregarDirector"/*, centro*/);
               
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    [HttpPost]
    //    public ActionResult AgregarDirector(string rutCentro, string idDirector)
    //    {
    //        try
    //        {
    //            //Repositorios.Repositorios.RepositorioCentro repo = new Repositorios.Repositorios.RepositorioCentro();
    //            //var lista = repo.FindById(rutCentro);
               
    //            return PartialView("AgregarDirector");
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }


    //    public ActionResult Todos()
    //    {
    //        var usu = HttpContext.Session.GetString("Usuario");
          
           
    //        if (usu != null && usu.Contains("DESAROLLADOR"))
    //        {              
    //            try
    //            {
    //                // traer todos los centos
    //                response = cliente.GetAsync(centroUri + "/Centro/Todos").Result;

    //                if (response.IsSuccessStatusCode)
    //                {
    //                    var usuario = response.Content.ReadAsAsync<String>().Result;
    //                    var objeto = JsonConvert.DeserializeObject<IEnumerable<ModeloCentro>>(usuario);

    //                    return View(objeto);
    //                }

    //            }
    //            catch (Exception ex)
    //            {
    //                return View();

    //            }
    //        }
    //        ViewBag.Mensaje = "Usuario no autorizado";
    //        return View("../Shared/Error", "Home");
    //    }


    //    // GET: Centro/Create
    //    public ActionResult SegundoCrearCentro(string mensaje)
    //    {
    //        ViewBag.Mensaje = mensaje;
    //        return View();
    //    }

    //    [HttpPost]
    //    public ActionResult SegundoCrearCentro(ModeloIngresoCentro modCentro)
    //    {

    //        if (!ModelState.IsValid) return View("Index");
    //        int rut;
    //        bool soloNumeros = int.TryParse(modCentro.Rut, out rut);
    //        if (!soloNumeros)
    //        {
    //            ViewBag.Mensaje = "El RUT solo puede contener números.";
    //            return View(modCentro);
    //        }
    //        try
    //        {
    //            // voy a buscar si hay algún centro registrado con ese RUT
    //            response = cliente.GetAsync(centroUri + "/Centro/BuscarCentro?rut=" + modCentro.Rut).Result;

    //            if (response.IsSuccessStatusCode)
    //            {
    //                string mensaje = "Ya existe un centro residencial registrado con ese RUT.";
    //                return RedirectToAction("SegundoCrearCentro", "Centro", new { mensaje = mensaje });
    //            }

    //            //Si el centro no existe lo agrego
    //            response = cliente.PostAsJsonAsync(centroUri + "/Centro/CrearCentro", modCentro).Result;

    //            if (response.IsSuccessStatusCode)
    //            {
    //                Centro agregado = response.Content.ReadAsAsync<Centro>().Result;
    //                string mensaje = "Centro Creado" + agregado.Nombre;
    //                return RedirectToAction("SegundoCrearCentro", "Centro", new { mensaje = mensaje });
    //            }

    //            ViewBag.Mensaje = "No se pudo agregar el centro: " + response.ReasonPhrase;
    //            return RedirectToAction("Index");


    //        }
    //        catch (Exception ex)
    //        {
    //            ViewBag.Mensaje = ex.Message;
    //            return View("../Shared/Error", "Home");
    //        }
    //    }

     
    //}
//}
