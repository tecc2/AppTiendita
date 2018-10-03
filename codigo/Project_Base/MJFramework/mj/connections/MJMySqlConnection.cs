using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;


// Version 1.0 

namespace MJFramework.mj.connections
{
    public class MJMySqlConnection
    {
         //variables de para la conexion 
        protected string ConnectionStringLocal;
        protected MySqlConnection SQLConexion;

        /// <summary>
        /// Permite inicializar de las configuraciones la cadena de conexion y genera una conexion nueva 
        /// </summary>         
        public MJMySqlConnection()
        {
            try
            {
                ConnectionStringLocal = ConfigurationManager.ConnectionStrings["LaboratorioClinico.Properties.Settings.ServerDb"].ConnectionString;
                //MySqlConnection SQLConexion = default(MySqlConnection);                
            }
            catch (Exception e)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringLocal = config.ConnectionStrings.ConnectionStrings["LaboratorioClinico.Properties.Settings.ServerDb"].ConnectionString.ToString();
                Console.WriteLine(e.ToString());
            }
            SQLConexion = new MySqlConnection(ConnectionStringLocal);
        }

        /// <summary>
        /// Valida que se abra la conexion y la mantiene abierta
        /// </summary>
        private void openConexion()
        {
            try
            {
                if (SQLConexion.State == ConnectionState.Closed)
                    SQLConexion.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Ocurrio un error  " + ex);
                closeConexion();
            }
        }

        /// <summary>
        /// Cierra la conexion que se vaya a generar. 
        /// </summary>
        protected void closeConexion()
        {
            if (SQLConexion.State == ConnectionState.Open)
            {
                SQLConexion.Close();
                SQLConexion.Dispose();
            }
        }

        /// <summary>
        /// Metodo utilizado para ejecutar sentencias de SQL que tienen que ver con ingresar, 
        /// modificar o borrar
        /// </summary>
        /// <param name="sentencia">Sentencia de SQL completa que se quiere ejecutar</param>
        /// <param name="parametros">Numero de parametros que se necesitan para utilizar</param>
        /// <returns>Regresa verdadero si no ocurre ninguna exception </returns>
        protected bool executeQueryUpdate(string sentencia, List<MySqlParameter> parametros)
        {
            bool accept = true;
            openConexion();
            try
            {
                MySqlCommand comando = new MySqlCommand();
                comando.CommandText = sentencia;
                comando.CommandType = CommandType.Text;
                comando.Connection = SQLConexion;
                foreach (MySqlParameter parametro in parametros)
                    comando.Parameters.Add(parametro);
                comando.ExecuteNonQuery();
                accept = true; 
            }
            catch (MySqlException e)
            {
                accept = false;
                Console.WriteLine("Error al tratar de ingresar al archivo");
                Console.WriteLine("Codigo error: " + e.ToString());
            }
            finally
            {
                closeConexion();
            }
            return accept; 
        }

        /// <summary>
        /// Metodo utilizado para ejecutar procedimientos almacenados de SQL que tienen que ver con ingresar, 
        /// modificar o borrar
        /// </summary>
        /// <param name="sentencia">Sentencia de SQL completa que se quiere ejecutar</param>
        /// <param name="parametros">Numero de parametros que se necesitan para utilizar</param>
        /// <returns>Regresa verdadero si no ocurre ninguna exception </returns>
        protected bool executeStoredProcedureUpdate(string nombreProcedimiento, List<MySqlParameter> parametros)
        {
            bool accept = true;
            openConexion();
            try
            {
                MySqlCommand comando = new MySqlCommand();
                comando.CommandText = nombreProcedimiento;
                comando.CommandType = CommandType.StoredProcedure;
                comando.Connection = SQLConexion;
                foreach (MySqlParameter parametro in parametros)
                    comando.Parameters.Add(parametro);
                comando.ExecuteNonQuery();
                accept = true;
            }
            catch (MySqlException e)
            {
                accept = false;
                Console.WriteLine("Error al tratar de ingresar al archivo");
                Console.WriteLine("Codigo error: " + e.ToString());
            }
            finally
            {
                closeConexion();
            }
            return accept;
        }

        /// <summary>
        /// Metodo principal para acceso a la base de datos retorna un reader mediante un procedimiento almacenado
        /// </summary>
        /// <param name="sentencia"></param>
        /// <param name="parametros"></param>
        /// <returns>MysqlDataReader</returns>
        protected MySqlDataReader findRecordsStoredProcedure(string nombreProcedimiento, List<MySqlParameter> parametros)
        {
            MySqlDataReader reader;
            try
            {
                openConexion();
                MySqlCommand sqlComando = new MySqlCommand();
                sqlComando.CommandText = nombreProcedimiento;
                sqlComando.CommandType = CommandType.StoredProcedure;
                sqlComando.Connection = SQLConexion;
                foreach (MySqlParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                reader = sqlComando.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.Write("ocurrio un error al leer " + ex);
                reader = null;
                closeConexion();
            }
            return reader;
        }

        /// <summary>
        /// Permite tener acceso a la base de datos mediante una sentencia SLQ y regresa un Reader con los datos
        /// </summary>
        /// <param name="sentencia"></param>
        /// <param name="parametros"></param>
        /// <returns>MysqlDataReader</returns>

        protected MySqlDataReader findRecordsSQL(string sentencia, List<MySqlParameter> parametros)
        {
            MySqlDataReader reader;
            try
            {
                openConexion();
                MySqlCommand sqlComando = new MySqlCommand();
                sqlComando.CommandText = sentencia;
                sqlComando.CommandType = CommandType.Text;
                sqlComando.Connection = SQLConexion;
                foreach (MySqlParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                reader = sqlComando.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.Write("ocurrio un error al leer " + ex);
                reader = null;
                closeConexion();
            }
          
            return reader;
        }

        /// <summary>
        /// metodo que toma una sentencia SQL y la ejecuta para que pueda traer una tabla del dataset 
        /// Lo utilizo para meterlo en los grid o en los combos       
        /// </summary>
        /// <param name="sentencia"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        protected DataTable getListQuerySQL(String sentencia, List<MySqlParameter> parametros)
        {
            DataSet miDataSet = new DataSet();
            bool ban = false;
            try
            {
                openConexion();
                MySqlCommand sqlComando = new MySqlCommand();
                sqlComando.CommandText = sentencia;                                
                sqlComando.CommandType = CommandType.Text;
                sqlComando.Connection = SQLConexion;
                foreach (MySqlParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                MySqlDataAdapter myAdap = new MySqlDataAdapter(sqlComando);
                myAdap.Fill(miDataSet, "permitidos");
                sqlComando.Dispose();
                ban = true;
            }
            catch (MySqlException ex)
            { Console.Write(ex); }
            finally
            { closeConexion(); }
            if (ban)
                return miDataSet.Tables[0];
            else
                return null;
        }

        /// <summary>
        /// metodo que toma una sentencia SQL y la ejecuta para que pueda traer una tabla del dataset 
        /// Lo utilizo para meterlo en los grid o en los combos       
        /// </summary>
        /// <param name="sentencia"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        protected DataTable getListQueryStoredProcedure(String sentencia, List<MySqlParameter> parametros)
        {
            DataSet miDataSet = new DataSet();
            bool ban = false;
            try
            {
                openConexion();
                MySqlCommand sqlComando = new MySqlCommand();
                sqlComando.CommandText = sentencia;
                sqlComando.CommandType = CommandType.StoredProcedure;
                sqlComando.Connection = SQLConexion;
                foreach (MySqlParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                MySqlDataAdapter myAdap = new MySqlDataAdapter(sqlComando);
                myAdap.Fill(miDataSet, "permitidos");
                sqlComando.Dispose();
                ban = true;
            }
            catch (MySqlException ex)
            { Console.Write(ex); }
            finally
            { closeConexion(); }
            if (ban)
                return miDataSet.Tables[0];
            else
                return null;
        }
    }
}
