using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioReceta
    {
        bool Add(Receta unReceta);

        bool Remove(Receta unReceta);

        bool Update(Receta unReceta);

        IEnumerable<Receta> FindAll();

        Receta FindById(string idReceta);
    }
}
