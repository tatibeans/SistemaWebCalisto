using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioConsulta
    {
        bool Add(Consulta unConsulta);

        bool Remove(Consulta unConsulta);

        bool Update(Consulta unConsulta);

        IEnumerable<Consulta> FindAll();

        Consulta FindById(string idConsulta);
    }
}
