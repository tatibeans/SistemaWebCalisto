using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Interfaces;
using Repositorios.Contexto;

namespace Repositorios.Repositorios
{
    public class RepositorioConsulta : IRepositorioConsulta
    {
        public bool Add(Consulta unConsulta)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Consulta> FindAll()
        {
            throw new NotImplementedException();
        }

        public Consulta FindById(string idConsulta)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Consulta unConsulta)
        {
            throw new NotImplementedException();
        }

        public bool Update(Consulta unConsulta)
        {
            throw new NotImplementedException();
        }
        public List<Consulta> FindAllConsultasRes(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var residente = db.Residentes.Where(c => c.Cedula == cedula).Include("Consultas").FirstOrDefault();
                    var consultas = residente.Consultas.ToList();

                    return consultas;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
