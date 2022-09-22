using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;

namespace Dominio.Interfaces
{
    public interface IRepositorioResidente
    {
        bool Add(Residente unResidente, string centro);

        bool Remove(Residente unResidente, string centro);

        bool Update(Residente unResidente, string centro);

        IEnumerable<Residente> FindAll(string centro);

        Residente FindById(string idResidente, string centro);
    }
}
