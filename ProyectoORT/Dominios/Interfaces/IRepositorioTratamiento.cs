using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioTratamiento
    {
        bool Add(Tratamiento unTratamiento);

        bool Remove(Tratamiento unTratamiento);

        bool Update(Tratamiento unTratamiento);

        IEnumerable<Tratamiento> FindAll();

        Tratamiento FindById(string idTratamiento);
    }
}
