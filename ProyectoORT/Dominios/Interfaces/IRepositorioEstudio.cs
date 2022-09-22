using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioEstudio
    {
        bool Add(Estudio unEstudio);

        bool Remove(Estudio unEstudio);

        bool Update(Estudio unEstudio);

        IEnumerable<Estudio> FindAll();

        Estudio FindById(string idEstudio);
    }
}
