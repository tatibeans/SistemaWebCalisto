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
    public class RepositorioEstudio : IRepositorioEstudio
    {
        public bool Add(Estudio unEstudio)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Estudio> FindAll()
        {
            throw new NotImplementedException();
        }

        public Estudio FindById(string idEstudio)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Estudio unEstudio)
        {
            throw new NotImplementedException();
        }

        public bool Update(Estudio unEstudio)
        {
            throw new NotImplementedException();
        }

        public List<Estudio> FindAllEstudiosRes(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var residente = db.Residentes.Where(c => c.Cedula == cedula).Include("Estudios").FirstOrDefault();
                    var estudios = residente.Estudios.ToList();

                    return estudios;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
