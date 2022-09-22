using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioInsumo
    {
        bool Add(Insumo unInsumo);

        bool Remove(Insumo unInsumo);

        bool Update(Insumo unInsumo);

        IEnumerable<Insumo> FindAll();

        Insumo FindById(string idInsumo);
    }
}
