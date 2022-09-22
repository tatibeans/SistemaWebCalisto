using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioAlerta
    {
        bool Add(Alerta unCentro);

        bool Remove(Centro unCentro);

        bool Update(Centro unCentro);

        IEnumerable<Centro> FindAll();

        Centro FindById(string rut);

    }
}
