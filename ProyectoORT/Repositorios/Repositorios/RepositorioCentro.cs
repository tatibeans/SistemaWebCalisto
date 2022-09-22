using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Interfaces;
using Newtonsoft.Json;
using Repositorios.Contexto;

namespace Repositorios.Repositorios
{
    public class RepositorioCentro : IRepositorioCentro
    {
        public bool CentroActivo(string centro,string contexto) 
        {
            if (string.IsNullOrEmpty(centro) || string.IsNullOrEmpty(contexto)) return false;
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var cen = db.Centro.Where(x => x.Nombre.Trim().ToUpper().Equals(centro.Trim().ToUpper())).FirstOrDefault();

                    if (cen == null || cen.OperativoHasta != null) 
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Add(Centro unCentro)
        {
            if (unCentro == null || !unCentro.Validar()) return false;
            try
            {
                using (SistemaContext db = new SistemaContext(""))
                {
                    var centroEncontrado = db.Centro.Where(c => string.Equals(c.Rut, unCentro.Rut));

                    if (centroEncontrado.Count() != 0) return false;

                    db.Centro.Add(unCentro);
                    db.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public Centro Find(string contexto)
        {
            if (string.IsNullOrEmpty(contexto)) return null;
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var centroEncontrado = db.Centro.FirstOrDefault();
                        
                    if (centroEncontrado != null)
                    {
                        return centroEncontrado;
                    }
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public bool Remove(Centro unCentro)
        {
            throw new NotImplementedException();
        }

        public bool Update(Centro unCentro)
        {
            // se puede actualizar cuando se agregan o quitan usuarios o residentes,
            // cambiar el tel, la direccion o el correo, y operativo hasta
            if (unCentro == null || !unCentro.Validar()) return false;
            try
            {
                using (SistemaContext db = new SistemaContext(""))
                {
                    var centroEncontrado = db.Centro.Where(c => string.Equals(c.Rut, unCentro.Rut));
                    if (centroEncontrado.Count() != 1) return false;
                    Centro centroActualizado = centroEncontrado.ToList()[0];

                    if (centroActualizado.Correo != unCentro.Correo) centroActualizado.Correo = unCentro.Correo;
                    if (centroActualizado.Direccion != unCentro.Direccion) centroActualizado.Direccion = unCentro.Direccion;
                    if (centroActualizado.Nombre != unCentro.Nombre) centroActualizado.Nombre = unCentro.Nombre;
                    if (centroActualizado.OperativoHasta != unCentro.OperativoHasta)
                        centroActualizado.OperativoHasta = unCentro.OperativoHasta;

                    if (!Enumerable.SequenceEqual(centroActualizado.Residentes, unCentro.Residentes))
                    {
                        centroActualizado.Residentes.Clear();
                        centroActualizado.Residentes = unCentro.Residentes;
                    }

                    if (!Enumerable.SequenceEqual(centroActualizado.Usuarios, unCentro.Usuarios))
                    {
                        centroActualizado.Usuarios.Clear();
                        centroActualizado.Usuarios = unCentro.Usuarios;
                    }

                    db.Entry(centroActualizado).State = EntityState.Modified;
                    db.Centro.Attach(centroActualizado);


                    //if (!Enumerable.SequenceEqual(centroActualizado.Directores, unCentro.Directores))
                    //{
                    //    centroActualizado.Directores.Clear();
                    //    foreach (Usuario u in unCentro.Directores)
                    //    {
                    //        centroActualizado.Directores.Add(u);
                    //    }
                    //}

                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
