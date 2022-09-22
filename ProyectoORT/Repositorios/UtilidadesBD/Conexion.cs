using System;
using System.Data.SqlClient;

namespace Repositorios.UtilidadesBD
{
    public class Conexion
    {
        /// <summary>
        /// Maneja la conexión a la base de datos
        /// </summary>
        private string CadenaConexion { get; set; }
        /// <summary>
        /// Retorna el objeto con la conexión seteada.
        /// </summary>
        /// <returns>SqlConnection con la conexión</returns>
        public SqlConnection CrearConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
        /// <summary>
        /// Constructor. Lee la cadena de conexión  desde el config.
        /// </summary>
        public Conexion()
        {
            CadenaConexion = System.Configuration.ConfigurationManager.
                ConnectionStrings["miConexion"].ConnectionString;
        }
        /// <summary>
        /// Encapsula la apertura de la conexión realizando los controles necesarios
        /// </summary>
        /// <param name="cn">Objeto con la cadena de conexión seteada</param>
        /// <returns>true si la pudo abrir, false en caso contrario</returns>
        public bool AbrirConexion(SqlConnection cn)
        {
            if (cn == null) return false;
            try
            {
                if (cn.State != System.Data.ConnectionState.Open)
                {
                    cn.Open();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert
                    (false, "Error al abrir la conexión." + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Encapsula el cierre de la conexión realizando los controles necesarios
        /// </summary>
        /// <param name="cn">Objeto con la cadena de conexión a cerrar, supuestamente abierta</param>
        /// <returns>true si la pudo cerrar, false en caso contrario</returns>
        public bool CerrarConexion(SqlConnection cn)
        {
            if (cn == null) return false;
            try
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert
                    (false, "Error al cerrar la conexión." + ex.Message);
                return false;
            }
        }
    }
}

