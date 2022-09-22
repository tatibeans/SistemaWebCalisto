using Dominio.Entidades;
using System.Data.Entity;

namespace Repositorios.Contexto
{
    public class SistemaContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Centro> Centro { get; set; }
        public DbSet<Residente> Residentes { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<SignoVital> SignoVitales { get; set; }
        public DbSet<Estudio> Estudios { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<TipoUsuario> Tipos { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }


        public SistemaContext(string conexion): base(conexion) 
        {
            
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
           
        }


        
    }
}
