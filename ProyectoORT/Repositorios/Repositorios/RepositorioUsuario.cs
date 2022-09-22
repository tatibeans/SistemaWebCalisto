using Dominio.Entidades;
using Dominio.Interfaces;
using Repositorios.Contexto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Mapping;
using System.Diagnostics;

namespace Repositorios.Repositorios
{
    public class RepositorioUsuario : IRepositorioUsuario
    {


        public bool TokenUsu(string token, string contexto)
        {

            if (token == null )
                return false;

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var usuarioEncontrado =
                    db.Usuarios.Where(u => u.TokenApi == token).FirstOrDefault();
                    if (usuarioEncontrado != null && usuarioEncontrado.FechaApi == DateTime.Today)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public bool TokenPWAUsu(string token, string contexto)
        {

            if (token == null)
                return false;

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var usuarioEncontrado =
                    db.Usuarios.Where(u => u.TokenPWA == token).FirstOrDefault();
                    if (usuarioEncontrado != null )
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public bool Add(Usuario unUsuario, string contexto)
        {
            //Esto anda bien
            if (unUsuario == null || !unUsuario.ValidarDatos())
                return false;
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var usuarioEncontrado =
                     db.Usuarios.Where(u => u.Cedula == unUsuario.Cedula);

                    if (usuarioEncontrado.Count() == 0)
                    {
                        string passEncriptada = Encriptar.GetSHA256(unUsuario.Password);
                        unUsuario.Password = passEncriptada;
                        unUsuario.fechaCreacion = DateTime.Now;
                        unUsuario.Centro = db.Centro.FirstOrDefault();

                        db.Usuarios.Add(unUsuario);

                        foreach (TipoUsuario c in unUsuario.Tipos)
                        {
                            db.Entry(c).State = EntityState.Unchanged;
                        }

                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }


        }


        public Usuario FindById(string cedula, string contexto)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var usuarioEncontrado =
                    db.Usuarios.Where(u => u.Cedula == cedula && u.Cedula!= "11111111").Include(u => u.Centro).Include(u => u.Residentes).Include(t => t.Tipos).FirstOrDefault();

                    if (usuarioEncontrado != null)
                    {
                        return usuarioEncontrado;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int BorrarUsu(string cedula, string contexto)
        {
            if (String.IsNullOrEmpty(cedula) || cedula == "11111111") return -1;
            int codigo = 0;
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                   
                    var usu = db.Usuarios.Where(u => u.Cedula == cedula).Include(t => t.Tipos)
                   .Include(u => u.Residentes).FirstOrDefault();

                    var directores = db.Tipos.Where(t => t.Tipo.Trim().ToUpper().
                     Equals("DIRECTOR")).Include("Usuarios").FirstOrDefault();

                    if (usu != null)
                    {
                        foreach (TipoUsuario t in usu.Tipos)
                        {
                            if (t.Tipo == "DIRECTOR" && directores.Usuarios.Count == 1)
                            {
                                codigo = 1;
                                return codigo;

                            }

                        }
                    }

                    //Aca no se borra por residente activo
                    if (usu != null && usu.Residentes != null)
                    {
                        foreach (Residente u in usu.Residentes)
                        {
                            if (u.FechaEgreso == null)
                            {
                                codigo = 2;
                                return codigo;
                            }
                           
                        }
                    }
                    
                    //var usuarios = db.Usuarios.ToList();
                    //usuarios.Remove(usu);
                    db.Usuarios.Remove(usu);
                    db.SaveChanges();

                    return codigo;
                }
            }
            catch (Exception ex)
            {
                return 6;
            }
        }

        public IEnumerable<Usuario> FindAllByTipo(string tipo, string contexto)
        {
            if (String.IsNullOrEmpty(tipo)) return null;

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tipoElegido = db.Tipos.Where(t => t.Tipo.Trim().ToUpper().
                     Equals(tipo.Trim().ToUpper())).Include("Usuarios").FirstOrDefault();
                    var lista = tipoElegido.Usuarios.Where(x=>x.Cedula!="11111111").ToList();
                    return lista.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TipoUsuario FindTipoId(string id, string contexto)
        {
            if (string.IsNullOrEmpty(id)) return null;

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tipo = db.Tipos.Where(t => t.Id.Equals(id)).FirstOrDefault();
                    return tipo;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public IEnumerable<Usuario> FindAll(string contexto)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    List<Usuario> listaADevolver = new List<Usuario>();

                    var lista = db.Usuarios.Where(uc=>uc.Cedula!="11111111").Include("Centro").Include("Tipos").ToList();

                    foreach (Usuario u in lista)
                    {
                        var res = db.Residentes.Where(x => x.unResponsable.Cedula == u.Cedula).ToList();
                        var usuario = u;
                        usuario.Password = "";
                        usuario.Residentes = (ICollection<Residente>)res;
                        listaADevolver.Add(usuario);
                    }

                    return listaADevolver;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Usuario LoginUsuario(string cedula, string password, string contexto)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    string passEncriptada = Encriptar.GetSHA256(password);
                    passEncriptada = passEncriptada.Substring(0, 30);

                    var usuarioEncontrado =
                     db.Usuarios.Where(u => u.Cedula.Equals(cedula) && u.Cedula != "11111111" && u.Password.Equals(passEncriptada))
                     .Include(u => u.Tipos).Include("Residentes").FirstOrDefault();

                   
                    var seed = Environment.TickCount;
                    Random rnd = new Random(seed);
                    string Token = Encriptar.GetSHA256(rnd.Next()+passEncriptada);
                    Token = Token.Substring(0, 30);

                    if (usuarioEncontrado != null)
                    {
                        usuarioEncontrado.TokenApi = Token;
                        usuarioEncontrado.FechaApi = DateTime.Today;

                        db.SaveChanges();
                    }

                    return usuarioEncontrado;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Usuario LoginUsuarioPWA(string cedula, string password, string contexto)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    string passEncriptada = Encriptar.GetSHA256(password);
                    passEncriptada = passEncriptada.Substring(0, 30);

                    var usuarioEncontrado =
                     db.Usuarios.Where(u => u.Cedula.Equals(cedula) && u.Cedula != "11111111" && u.Password.Equals(passEncriptada))
                     .Include(u => u.Tipos).Include("Residentes").FirstOrDefault();

                    bool esResponsable = false;

                    if (usuarioEncontrado == null || usuarioEncontrado.Tipos == null)
                    {
                        return null;
                    }
                    
                    foreach (TipoUsuario tipo in usuarioEncontrado.Tipos)
                    {
                        if (tipo.Tipo.Equals("RESPONSABLE"))
                        {
                            esResponsable = true;
                        }
                    }

                    if (!esResponsable)
                    {
                        return null;
                    }
                    var seed = Environment.TickCount;
                    Random rnd = new Random(seed);
                    string Token = Encriptar.GetSHA256(rnd.Next() + passEncriptada);
                    Token = Token.Substring(0, 30);

                    if (usuarioEncontrado != null)
                    {
                        usuarioEncontrado.TokenPWA = Token;
                        db.SaveChanges();
                    }

                    return usuarioEncontrado;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool Remove(Usuario unUsuario, string contexto)
        {
            throw new NotImplementedException();
        }

        public Usuario Update(Usuario unUsuario, string contexto)
        {
            if (unUsuario == null || !ValidarUpdate(unUsuario))
                return null;

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var existente = db.Usuarios.Where(u => u.Cedula == unUsuario.Cedula && u.Cedula != "11111111").Include(t => t.Tipos).FirstOrDefault();
                    List<TipoUsuario> tipos = new List<TipoUsuario>();
                    TipoUsuario tu = new TipoUsuario();
                    foreach (TipoUsuario c in unUsuario.Tipos)
                    {
                        tu = db.Tipos.Where(x => x.Tipo == c.Tipo).FirstOrDefault();
                        tipos.Add(tu);
                    }

                    if (existente == null) return null;

                    existente.Correo = unUsuario.Correo;
                    existente.Nombre = unUsuario.Nombre;
                    existente.Telefono = unUsuario.Telefono;
                    existente.Tipos = tipos;


                    db.SaveChanges();
                    return existente;
                }
            }
            catch (Exception ex) { return null; }
        }

        public bool ValidarUpdate(Usuario usu)
        {
            int a;
            Regex regNom = new Regex("[^A-Za-z]");
            Regex regMail = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            bool ci = usu.ValidarCedula();
            bool tel1 = !String.IsNullOrEmpty(usu.Telefono);
            bool tel2 = int.TryParse(usu.Telefono, out a);
            bool tel3 = usu.Telefono.Length > 7;
            bool tel4 = usu.Telefono.Length < 16;
            bool nom1 = !String.IsNullOrEmpty(usu.Nombre);
            //bool nom2 = regNom.IsMatch(usu.Nombre);
            bool mail1 = !String.IsNullOrEmpty(usu.Correo);
            bool mail2 = regMail.IsMatch(usu.Correo);


            return ci && tel1 && tel2 && tel3 && tel4 && nom1 && mail1 && mail2;
        }


        public Usuario UpdatePassword(Usuario unUsuario, string contexto)
        {
            //No esta probado
            if (unUsuario == null)
            {
                return null;
            }
            using (SistemaContext db = new SistemaContext(contexto))
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                   
                    string passEncriptada = Encriptar.GetSHA256(unUsuario.Password);
                    passEncriptada = passEncriptada.Substring(0, 30);

                    //  var usu = db.Usuarios.Where(d => d.Token == unUsuario.Token).Include(u => u.Centros).Include(u => u.Residentes);

                    var usuarioEncontrado =
                    db.Usuarios.Where(d => d.Token == unUsuario.Token)
                   .Include("Centro").Include(u => u.Residentes).Include(u => u.Tipos)
                   .FirstOrDefault();

                    if (usuarioEncontrado == null)
                    {
                        return null;
                    }
                    if (usuarioEncontrado != null)
                    {
                        usuarioEncontrado.Password = passEncriptada;
                        usuarioEncontrado.Token = null;
                        db.Usuarios.Attach(usuarioEncontrado);
                        db.Entry(usuarioEncontrado).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    dbTransaction.Commit();
                    usuarioEncontrado.Password = "";

                    ; return usuarioEncontrado;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    return null;
                }
            }
        }


        public bool UpdateToken(Usuario usu, string contexto)
        {
            try
            {
                if (usu != null)
                {
                    using (SistemaContext db = new SistemaContext(contexto))
                    {
                        db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public IEnumerable<TipoUsuario> FindAllTipos(string contexto)
        {
            try
            {
                if (!String.IsNullOrEmpty(contexto))
                {
                    using (SistemaContext db = new SistemaContext(contexto))
                    {
                        var todos = db.Tipos.ToList();
                        return todos;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }


}

