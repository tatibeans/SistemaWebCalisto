using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Interfaces;
using Repositorios.Contexto;

namespace Repositorios.Repositorios
{
    public class RepositorioInsumo : IRepositorioInsumo
    {
        public bool Add(Insumo unInsumo)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Insumo> FindAll()
        {
            throw new NotImplementedException();
        }

        public Insumo FindById(string idInsumo)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Insumo unInsumo)
        {
            throw new NotImplementedException();
        }

        public bool Update(Insumo unInsumo)
        {
            throw new NotImplementedException();
        }

        public List<Insumo> FindAllInsumosRes(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var residente = db.Residentes.Where(c => c.Cedula == cedula).Include("Insumos").FirstOrDefault();
                    var insumos = residente.Insumos.ToList();

                    return insumos;
                }
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
