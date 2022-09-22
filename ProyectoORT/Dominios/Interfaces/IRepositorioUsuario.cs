using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IRepositorioUsuario
    {

            bool Add(Usuario unUsuario, string contexto);

            bool Remove(Usuario unUsuario, string contexto);

            Usuario Update(Usuario unUsuario, string contexto);

           IEnumerable<Usuario> FindAll(string contexto);

            Usuario FindById(string cedula, string contexto);

        Usuario LoginUsuario(string cedula, string password, string contexto);
       
    }
}
