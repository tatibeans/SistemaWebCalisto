using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioCentro
    {
        bool Add(Centro unCentro);

        bool Remove(Centro unCentro);

        bool Update(Centro unCentro);

        //IEnumerable<Centro> FindAll();

        Centro Find(string contexto);
    }
}
