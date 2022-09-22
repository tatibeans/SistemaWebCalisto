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
    public class RepositorioAlerta : IRepositorioAlerta
    {
        public bool Add(Alerta unCentro)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Centro> FindAll()
        {
            throw new NotImplementedException();
        }

        public Centro FindById(string rut)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Centro unCentro)
        {
            throw new NotImplementedException();
        }

        public bool Update(Centro unCentro)
        {
            throw new NotImplementedException();
        }
    }
}
