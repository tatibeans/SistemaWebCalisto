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
    public class RepositorioResidente : IRepositorioResidente
    {
        public bool Add(Residente unResidente, string centro)
        {
            try
            {
                if (unResidente == null) return false;
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var res = db.Residentes.Where(r => r.Cedula.Equals(unResidente.Cedula));
                    if (res.ToList().Count == 0)
                    {
                        db.Residentes.Add(unResidente);
                        db.Entry(unResidente.Centro).State = EntityState.Unchanged;
                        db.Entry(unResidente.unResponsable).State = EntityState.Unchanged;

                        db.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public IEnumerable<Residente> FindAll(string centro)
        {
            List<Residente> todos = new List<Residente>();
            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    todos = db.Residentes.Include(r => r.unResponsable)
                        .Where(r => r.FechaEgreso == null)
                        .Include(c => c.Centro)
                        .Include(r => r.unResponsable.Tipos).ToList();

                    //foreach(Residente u in todos)
                    //{
                    //    var usuParaTipos = db.Usuarios.Where(res => res.Cedula == u.unResponsable.Cedula).Include(t => t.Tipos).FirstOrDefault();
                    //    u.unResponsable.Tipos = usuParaTipos.Tipos;
                    //}
                    //var tiposResponsable = 

                }
                return todos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Residente FindById(string idResidente, string centro)
        {
            if (string.IsNullOrEmpty(idResidente)) return null;
            try
            {
                Residente res = null;
                using (SistemaContext db = new SistemaContext(centro))
                {
                    res = db.Residentes.Where(r => r.Cedula == idResidente).Include(r => r.unResponsable).Include(c => c.Centro).FirstOrDefault();

                }
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        public bool CrearAlertaRes(Alerta alerta, string centro)
        {
            if (alerta == null) return false;
            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var res = db.Residentes.Where(r => r.Cedula.Equals(alerta.CiResidente)).FirstOrDefault();
                    res.Alertas.Add(alerta);

                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }




        public bool Remove(Residente unResidente, string centro)
        {
            throw new NotImplementedException();
        }

        public bool BorrarUsu(string cedula, string centro)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var usuCentro = db.Usuarios.Where(u => u.Cedula == "11111111").Include(t => t.Tipos)
                  .Include(u => u.Residentes).FirstOrDefault();

                    var resi = db.Residentes.Where(r => r.Cedula == cedula).Include(res => res.unResponsable).Include(c => c.Centro).FirstOrDefault();
                    if (resi != null)
                    {

                        resi.FechaEgreso = DateTime.Now;
                        resi.unResponsable = usuCentro;
                        usuCentro.Residentes.Add(resi);
                        db.SaveChanges();
                        return true;


                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }


        public bool Update(Residente unResidente, string centro)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var existente = db.Residentes.Where(r => r.Cedula == unResidente.Cedula).FirstOrDefault();
                    if (existente == null) return false;

                    existente.Nombre = unResidente.Nombre;
                    existente.Genero = unResidente.Genero;
                    existente.Dieta = unResidente.Dieta;
                    existente.Alergias = unResidente.Alergias;
                    existente.Condiciones = unResidente.Condiciones;
                    existente.FechaNacimiento = unResidente.FechaNacimiento;
                    existente.Mutualista = unResidente.Mutualista;
                    existente.ServicioFunebre = unResidente.ServicioFunebre;
                    existente.unResponsable = db.Usuarios.Where(x => x.Cedula == unResidente.unResponsable.Cedula).FirstOrDefault();

                    db.SaveChanges();
                    return true;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region insumos
        public bool IngresarInsumo(Insumo ins, string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {

                    var residente = db.Residentes.Where(r => cedula.Equals(r.Cedula)).Include(s => s.Insumos).FirstOrDefault();
                    if (residente == null) return false;

                    residente.Insumos.Add(ins);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Insumo BuscarInsumo(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var insumo = db.Insumos.Where(i => i.IdInsumo == id).FirstOrDefault();
                    if (insumo == null) return null;


                    return insumo;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool BorrarInsumo(string contexto, Insumo insu)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var insumo = db.Insumos.Where(r => r.IdInsumo == insu.IdInsumo).FirstOrDefault();

                    if (insumo != null)
                    {
                        var re = db.Residentes.Include("Insumos").ToList();
                        re.ForEach(i => i.Insumos.Remove(insumo));
                        db.Insumos.Remove(insumo);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion 

        #region estudios
        public bool IngresarEstudio(Estudio est, string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {

                    var residente = db.Residentes.Where(r => cedula.Equals(r.Cedula)).Include(s => s.Estudios).FirstOrDefault();
                    if (residente == null) return false;

                    residente.Estudios.Add(est);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Estudio BuscarEstudio(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var estudio = db.Estudios.Where(i => i.IdEstudio == id).FirstOrDefault();
                    if (estudio == null) return null;


                    return estudio;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool BorrarEstudio(string contexto, Estudio est)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var estudio = db.Estudios.Where(r => r.IdEstudio == est.IdEstudio).FirstOrDefault();

                    if (estudio != null)
                    {
                        var re = db.Residentes.Include("Estudios").ToList();
                        re.ForEach(i => i.Estudios.Remove(estudio));
                        db.Estudios.Remove(estudio);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region consulta
        public bool IngresarConsulta(Consulta con, string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {

                    var residente = db.Residentes.Where(r => cedula.Equals(r.Cedula)).Include(s => s.Consultas).FirstOrDefault();
                    if (residente == null) return false;

                    residente.Consultas.Add(con);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Consulta BuscarConsulta(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var consulta = db.Consultas.Where(i => i.Id == id).FirstOrDefault();
                    if (consulta == null) return null;


                    return consulta;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool BorrarConsulta(string contexto, Consulta con)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var consulta = db.Consultas.Where(r => r.Id == con.Id).FirstOrDefault();

                    if (consulta != null)
                    {
                        var re = db.Residentes.Include("Consultas").ToList();
                        re.ForEach(i => i.Consultas.Remove(consulta));
                        db.Consultas.Remove(consulta);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion



        public bool IngresarSignos(SignoVital signos, string centro, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var funcionario = db.Usuarios.Where(r => signos.CiFuncionario.Equals(r.Cedula)).FirstOrDefault();
                    if (funcionario == null) return false;

                    var residente = db.Residentes.Where(r => cedula.Equals(r.Cedula)).Include(s => s.SignosVitales).FirstOrDefault();
                    if (residente == null) return false;
                    if (signos.Validar())
                    {
                        residente.SignosVitales.Add(signos);
                        db.SaveChanges();
                        return true;
                    }
                    return false;

                }
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public SignoVital FindSigno(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var signo = db.SignoVitales.Where(r => r.IdSignoVital == id).FirstOrDefault();

                    return signo;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<SignoVital> FindAllSignosRes(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var res = db.Residentes.Where(r => r.Cedula.Equals(cedula)).Include("SignosVitales").FirstOrDefault();
                    List<SignoVital> todos = res.SignosVitales.OrderByDescending(x => x.FechaRegistro).ToList();
                    return todos;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public bool EditarSigno(SignoVital signo, string centro)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var existente = db.SignoVitales.Where(r => r.IdSignoVital == signo.IdSignoVital).FirstOrDefault();
                    if (existente == null) return false;

                    existente.Temperatura = signo.Temperatura;
                    existente.Azucar = signo.Azucar;
                    existente.CiFuncionario = signo.CiFuncionario;
                    existente.PresionMaxima = signo.PresionMaxima;
                    existente.PresionMinima = signo.PresionMinima;
                    existente.Pulso = signo.Pulso;
                    existente.Comentario = signo.Comentario;
                    existente.Oxigeno = signo.Oxigeno;

                    db.SaveChanges();
                    return true;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool AgregarTratamiento(string contexto, Tratamiento trat/*, string ci*/)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var residente = db.Residentes.Where(r => r.Cedula.Equals(trat.CiResidente)).FirstOrDefault();
                    if (residente != null)
                    {
                        //db.Tratamientos.Add(trat);
                        // creo que no es necesario esto. lo dejo por si no anda solo con lo anterior
                        residente.Tratamientos.Add(trat);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AgregarMedicamento(string contexto, Medicamento nuevoMed, int idTrat )
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tratamiento = db.Tratamientos.Where(r => r.IdTratamiento.Equals(idTrat)).Include("Medicamentos").FirstOrDefault();
                    if (tratamiento != null)
                    {
                        tratamiento.Medicamentos.Add(nuevoMed);
                     
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }






        public Tratamiento FindTratById(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var trat = db.Tratamientos.Where(t => t.IdTratamiento == id).FirstOrDefault();
                    return trat;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public List<Receta> FindRecetasRes(string contexto, string cedula)
        //{
        //    try
        //    {
        //        using (SistemaContext db = new SistemaContext(contexto))
        //        {
        //            var res = db.Residentes.Where(r => r.Cedula.Equals(cedula)).Include("Tratamientos").FirstOrDefault();
                 
        //            List<Receta> recetas = new List<Receta>();
        //            foreach (Tratamiento t in res.Tratamientos)
        //            {
        //                var trat = db.Tratamientos.Where(x => x.IdTratamiento == t.IdTratamiento).Include("Recetas").FirstOrDefault();
        //                if (trat.Recetas != null) { 
        //                recetas.AddRange(trat.Recetas);
        //            }
        //            }

                    
        //            return recetas;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public List<Tratamiento> FindAllTratamientos(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var res = db.Residentes.Where(r => r.Cedula.Equals(cedula)).Include("Tratamientos").FirstOrDefault();
                    List<Tratamiento> listTratamiento = new List<Tratamiento>();
                    foreach (Tratamiento t in res.Tratamientos)
                    {
                        Tratamiento tratamiento =db.Tratamientos.Where(r => r.IdTratamiento == t.IdTratamiento).
                            Include("Recetas").Include("Medicamentos").FirstOrDefault();
                       
                        listTratamiento.Add(tratamiento);
                    }

                    listTratamiento = listTratamiento.OrderByDescending(x => x.IdTratamiento).ToList();
                    return listTratamiento;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Tratamiento FindTratamiento(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tratamiento = db.Tratamientos.Where(r => r.IdTratamiento.Equals(id)).FirstOrDefault();

                    return tratamiento;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Receta FindReceta(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var receta = db.Recetas.Where(r => r.IdReceta.Equals(id)).FirstOrDefault();

                    return receta;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Medicamento FindMedicamento(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var medicamento = db.Medicamentos.Where(m => m.IdMedicamento.Equals(id)).FirstOrDefault();

                    return medicamento;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool EliminarTrat(string contexto, int id)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tratamiento = db.Tratamientos.Where(t => t.IdTratamiento == id).Include("Recetas").FirstOrDefault();
                    List<Receta> recetasABorrar = new List<Receta>();
                    List<Medicamento> medicamentosABorrar = new List<Medicamento>();
                    if (tratamiento != null)
                    {
                        foreach (Receta r in tratamiento.Recetas)
                        {
                            var receta = db.Recetas.Where(re => re.IdReceta == r.IdReceta).FirstOrDefault();
                            recetasABorrar.Add(receta);
                        
                        }
                    }
                   
                    if (recetasABorrar != null && recetasABorrar.Count() > 0)
                    {
                        db.Recetas.RemoveRange(recetasABorrar);
                    }
                    
                    db.Tratamientos.Remove(tratamiento);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EliminarMed(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var medicamento = db.Medicamentos.Where(m => m.IdMedicamento == id).FirstOrDefault();

                    if (medicamento == null) return false;

                    db.Medicamentos.Remove(medicamento);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Receta FindRecetaById(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var rec = db.Recetas.Where(r => r.IdReceta == id).FirstOrDefault();
                    return rec;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool AgregarReceta(string contexto, Receta rec)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {

                    var receta = db.Recetas.Where(re => re.IdReceta == rec.IdReceta).FirstOrDefault();
                    if (receta == null)
                    {
                        db.Recetas.Add(rec);
                        db.SaveChanges();
                        return true;

                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool EditarTratamiento(string contexto, Tratamiento trat)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {

                    var tratamiento = db.Tratamientos.Where(t => t.IdTratamiento == trat.IdTratamiento).FirstOrDefault();
                    if (tratamiento != null)
                    {
                        tratamiento.Medico = trat.Medico;
                        tratamiento.Descripcion = trat.Descripcion;
                        tratamiento.FechaComienzo = trat.FechaComienzo;
                        tratamiento.FechaFin = trat.FechaFin;
                        tratamiento.FechaIngreso = DateTime.Now;

                        db.SaveChanges();
                        return true;

                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Alerta> FindAlertas(string contexto)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var alertas = db.Alertas.OrderByDescending(x=> x.Fecha).ToList();
                    
                    return alertas;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public List<Alerta> FindAlertasRes(string cedula, string centro)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(centro))
                {
                    var alertas = db.Alertas.Where(x=>x.CiResidente.Equals(cedula)).ToList();
                    return alertas;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public List<Consulta> FindAllConsultas(string contexto)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var consultas = db.Consultas.ToList();
                    return consultas;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public List<Tratamiento> FindAllTratamientosGenerico(string contexto)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tratamientos = db.Tratamientos.ToList();
                    return tratamientos;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }


        public List<Estudio> FindAllEstudios(string contexto)
        {

            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var estudios = db.Estudios.ToList();
                    return estudios;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        
        /// <summary>
        /// busca los medicamentos de ese residente que tengan stock para 10 o 3 días
        /// </summary>
        /// <param name="contexto"></param>
        /// <param name="cedula"></param>
        /// <returns>
        /// objeto con {medicamento, 10 o 3 dias, tratamiento}
        /// </returns>
        public ICollection<List<Object>> FaltaStockMedicamentos(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {
                    var tratamientos = db.Tratamientos.Where(r => r.CiResidente.Equals(cedula)).Include("Medicamentos").ToList();
                    if (tratamientos != null)
                    {
                        List<Object> medsBajoStock = new List<Object>();
                        List<Object> cantDias = new List<Object>();
                        List<Object> trats = new List<Object>();
                        List < Object > PRUEBAS = new List<Object>();
                        foreach (Tratamiento t in tratamientos)
                        {
                            if (t.Medicamentos != null && t.Medicamentos.Count() != 0)
                            {
                                var meds = t.Medicamentos.OrderByDescending(m => m.FechaFinStock).GroupBy(x => x.Sustancia).ToList();

                                
                                List<string> susts = new List<string>();
                                foreach (Medicamento m in meds)
                                {
                                    PRUEBAS.Add(m);
                                    if (!susts.Contains(m.Sustancia))
                                    {
                                        susts.Add(m.Sustancia);
                                        //Medicamento ultimoMed = grupo.FirstOrDefault();

                                        List<object> medsRet = BajoStock(m);
                                        if (medsRet != null)
                                        {
                                            medsBajoStock.Add(medsRet[0]);
                                            cantDias.Add(medsRet[1]);
                                            trats.Add(t.Descripcion);
                                        }
                                    }

                                }
                            }
                        }
                        List<List<Object>> retorno = new List<List<Object>>();
                        retorno.Add(PRUEBAS);
                        if (medsBajoStock.Count() != 0)
                        {
                            
                            retorno.Add(medsBajoStock);
                            retorno.Add(cantDias);
                            retorno.Add(trats);
                            return retorno;
                        }
                        return retorno;
                    }
                }
                return null;
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ICollection<List<Object>> RecetasVencen(string contexto, string cedula)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto)) {
                    var recetas = db.Recetas.Where(r => r.Tratamiento.CiResidente.Equals(cedula)).ToList();
                    List<List<Object>> retorno = new List<List<object>>();
                    List<Object> recsVencen = new List<object>();
                    List<Object> diasQuedan = new List<object>();
                    List<Object> trats = new List<object>();
                    List<Object> PRUEBAS = new List<object>();
                    if (recetas != null)
                    {
                        foreach (Receta r in recetas)
                        {
                            int dias = (r.FechaVencimiento - DateTime.Today).Days;
                            PRUEBAS.Add(r.IdReceta + " " + dias);
                            if (dias == 10)
                            {
                                recsVencen.Add(r);
                                diasQuedan.Add(10);
                                trats.Add(r.Tratamiento.Descripcion);
                            }
                            else if (dias == 3)
                            {
                                recsVencen.Add(r);
                                diasQuedan.Add(3);
                                trats.Add(r.Tratamiento.Descripcion);
                            }
                        }
                        retorno.Add(PRUEBAS);
                        if (recsVencen.Count() != 0)
                        {
                            retorno.Add(recsVencen);
                            retorno.Add(diasQuedan);
                            retorno.Add(trats);
                            return retorno;
                        }
                        return retorno;
                    }
                    
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }


        public bool EliminarReceta(string contexto, int id)
        {
            try
            {
                using (SistemaContext db = new SistemaContext(contexto))
                {

                    var receta = db.Recetas.Where(re => re.IdReceta == id).Include("Tratamiento").FirstOrDefault();
                    receta.Tratamiento = null;

                    db.Recetas.Remove(receta);

                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// se fija si el medicamento tiene poco stock, exactamente 10 o 3 dias
        /// </summary>
        /// <param name="m"></param>
        /// <returns>
        /// devuelve una lista, si tiene bajo stock guarda en la pos 0 el medicamento, en la pos 1 la cant dias
        /// si no tiene bajo stock, devuelve null
        /// </returns>
        public List<object> BajoStock(Medicamento m)
        {
            int dias = (m.FechaFinStock - DateTime.Today).Days;
            if (dias == 10)
            {
                List<object> retorno = new List<object>();
                retorno.Add(m);
                retorno.Add(10);
                return retorno;
            } else if (dias == 3)
            {
                List<object> retorno = new List<object>();
                retorno.Add(m);
                retorno.Add(3);
                return retorno;
            }
            return null;
        }

    }
}
