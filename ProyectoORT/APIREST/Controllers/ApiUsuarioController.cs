using Dominio.Entidades;
using Repositorios.Repositorios;
using System.Web.Http;
using APIREST.Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Web.Http.Cors;

namespace APIREST.Controllers
{
    [RoutePrefix("api/Usuario")]
    //[EnableCors(origins: "localhost:3000", headers: "*", methods: "GET")]
    public class ApiUsuarioController : ApiController
    {
        RepositorioUsuario repoUsu = new RepositorioUsuario();
        RepositorioCentro repoCentro = new RepositorioCentro();

        [HttpGet]
        [Route("LogIn")]
        public IHttpActionResult GetUsuario(string cedula, string pass, string centro)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
            //Esta funcionando      
            var unUsuario = repoUsu.LoginUsuario(cedula, pass, contexto);
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
        [Route("Buscar")]
        public IHttpActionResult GetUsuarioSoloCedula(string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var unUsuario = repoUsu.FindById(cedula, contexto);
                if (unUsuario != null)
                {
                    unUsuario = LimpiarUsuario(unUsuario);
                    unUsuario.Tipos = LimpiarTipos((List<TipoUsuario>)unUsuario.Tipos);
                    return Ok(unUsuario);
                }
                else
                {
                    return BadRequest("No existe un usuario registrado con esta cédula.");
                }
            }
            return BadRequest("Permisos denegados");
        }


        [HttpGet]
        [Route("BuscarParaRecuperar")]
        public IHttpActionResult GetUsuarioParaRecuperar(string cedula, string centro)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var unUsuario = repoUsu.FindById(cedula, contexto);
                if (unUsuario != null)
                {
                    unUsuario = LimpiarUsuario(unUsuario);
                    unUsuario.Tipos = LimpiarTipos((List<TipoUsuario>)unUsuario.Tipos);
                    return Ok(unUsuario);
                }
                else
                {
                    return BadRequest("No existe un usuario registrado con esta cédula.");
                }
            
        }

        [HttpPost]
        [Route("Borrar")]
        public IHttpActionResult PostBorrarUsuario([FromBody] string cedula, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var codigo = repoUsu.BorrarUsu(cedula, contexto);
                if (codigo >= 0)
                {
                    return Ok(codigo);
                }
                else
                {
                    return BadRequest();
                }
            }
            return BadRequest("Permisos denegados");


        }


        [HttpPost]
        [Route("Registro")]
        public IHttpActionResult PostUsuario([FromBody] ModeloUsuario usuModel, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var usu = ParseModeloAUsu(usuModel);
                if (usu != null)
                {
                    bool flag = repoUsu.Add(usu, contexto);
                    if (flag)
                    {
                        return Ok(usu);
                    }
                    else
                    {
                        return BadRequest("Ya existe un usuario registrado con esta cédula.");
                    }
                }
                return BadRequest("Algo salió mal");
            }
            return BadRequest("Permisos denegados");



        }

        [HttpPost]
        [Route("Actualizar")]
        public IHttpActionResult Actualizar([FromBody] Usuario usu, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {

                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;

                if (usu != null)
                {
                    usu = LimpiarUsuario(usu);
                    usu.Tipos = LimpiarTipos((List<TipoUsuario>)usu.Tipos);
                    var actualizado = repoUsu.Update(usu, contexto);
                    if (actualizado != null) return Ok(actualizado);
                }

                return BadRequest("No se ha podido realizar la acción.");
            }
            return BadRequest("Permisos denegados");

        }

        [HttpPost]
        [Route("Token")]
        public IHttpActionResult PostUsuarioToken([FromBody] ModeloUsuario usuModel, string centro)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
            var usu = ParseModeloAUsu(usuModel);
            if (usu != null)
            {

                bool flag = repoUsu.UpdateToken(usu, contexto);
                if (flag)
                {
                    return Ok(usu);
                }
                else
                {
                    return BadRequest("Ya existe un usuario registrado con esta cédula.");
                }

            }
            return BadRequest("Algo salió mal");

        }

        [HttpPost]
        [Route("UpdatePassword")]
        public IHttpActionResult PostUsuarioPass([FromBody] ModeloUsuario usuModel, string centro)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
            var usu = ParseModeloAUsu(usuModel);

            if (usu != null)
            {
                var usuActual = repoUsu.UpdatePassword(usu, contexto);
                if (usuActual != null)
                {
                    usuActual = LimpiarUsuario(usuActual);
                    usuActual.Tipos = LimpiarTipos((List<TipoUsuario>)usuActual.Tipos);
                    //var usuarioJson = JsonConvert.SerializeObject(usuActual);
                    return Ok(usuActual);
                }
                else
                {
                    return BadRequest("El usuario no fue encontrado");
                }
            }
            else
            {
                return BadRequest("Algo salió mal");
            }
        }


        #region buscar
        [HttpGet]
        [Route("Todos")]
        public IHttpActionResult BuscarTodos(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                var todos = repoUsu.FindAll(contexto);
                if (todos != null)
                {
                    var todosLimpios = new List<Usuario>();
                    foreach (Usuario usu in todos)
                    {
                        usu.Tipos = LimpiarTipos((List<TipoUsuario>)usu.Tipos);

                        todosLimpios.Add(LimpiarUsuario(usu));
                    }
                    return Ok(todosLimpios);
                }
                return BadRequest("Algo salió mal");
            }
            return BadRequest("Permisos denegados");

        }


        [HttpGet]
        [Route("TodosTipos")]
        public IHttpActionResult BuscarTipos(string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                List<TipoUsuario> todos = (List<TipoUsuario>)repoUsu.FindAllTipos(contexto);

                if (todos != null)
                {
                    todos = LimpiarTipos(todos);
                    var respuesta = JsonConvert.SerializeObject(todos);
                    return Ok(respuesta);
                }

                return BadRequest("Algo salió mal");
            }
            return BadRequest("Permisos denegados");

        }



        [HttpGet]
        [Route("BuscarTiposId")]
        public IHttpActionResult BuscarTiposPorId(string tipos, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }
            if (UsuToken(token, centro))
            {

                if (string.IsNullOrEmpty(tipos)) return BadRequest("Algo salió mal.");

                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                string[] separados = tipos.Split('+');
                List<TipoUsuario> seleccionados = new List<TipoUsuario>();
                for (int i = 0; i < separados.Length; i++)
                {
                    var rolJson = repoUsu.FindTipoId(separados[i], contexto);
                    seleccionados.Add(rolJson);
                }
                return Ok(seleccionados);
            }
            return BadRequest("Permisos denegados");

        }



        [HttpGet]
        [Route("UsuariosPorTipo")]
        public IHttpActionResult BuscarPorTipo(string tipo, string centro, string token)
        {
            if (!CentroActivo(centro))
            {
                return BadRequest("El centro no se encuentra operativo.");
            }

            if (UsuToken(token, centro))
            {
                var contexto = System.Configuration.ConfigurationManager.ConnectionStrings[$"{centro}"].ConnectionString;
                List<Usuario> usuarios = (List<Usuario>)repoUsu.FindAllByTipo(tipo, contexto);
                if (usuarios != null)
                {
                    for (int i = 0; i < usuarios.Count; i++)
                    {
                        usuarios[i] = LimpiarUsuario(usuarios[i]);
                        usuarios[i].Tipos = LimpiarTipos((List<TipoUsuario>)usuarios[i].Tipos);
                    }
                    return Ok(usuarios);
                }

                return BadRequest("Algo salió mal");
            }
            return BadRequest("Permisos denegados");

        }
        #endregion

        #region auxiliares
        public Usuario ParseModeloAUsu(ModeloUsuario modUsu)
        {
            if (modUsu == null) return null;

            Usuario usu = new Usuario
            {
                Nombre = modUsu.Nombre,
                Correo = modUsu.Correo,
                Password = modUsu.Password,
                Telefono = modUsu.Telefono,
                Tipos = modUsu.Tipos,
                Cedula = modUsu.Cedula,
                fechaCreacion = modUsu.fechaCreacion,
                Token = modUsu.Token,
                Centro = modUsu.Centro,
                Residentes = modUsu.Residentes
            };
            return usu;
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
