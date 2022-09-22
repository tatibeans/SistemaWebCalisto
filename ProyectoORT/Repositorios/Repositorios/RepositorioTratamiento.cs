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
    public class RepositorioTratamiento : IRepositorioTratamiento
    {
        public bool Add(Tratamiento unTratamiento)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tratamiento> FindAll()
        {
            throw new NotImplementedException();
        }

        public Tratamiento FindById(string idTratamiento)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Tratamiento unTratamiento)
        {
            throw new NotImplementedException();
        }

        public bool Update(Tratamiento unTratamiento)
        {
            throw new NotImplementedException();
        }
    }
}
