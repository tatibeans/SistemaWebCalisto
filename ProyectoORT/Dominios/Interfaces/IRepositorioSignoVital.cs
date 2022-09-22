using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioSignoVital
    {
        bool Add(SignoVital unSignoVital);

        bool Remove(SignoVital unSignoVital);

        bool Update(SignoVital unSignoVital);

        IEnumerable<SignoVital> FindAll();

        SignoVital FindById(string idSignoVital);
    }
}
