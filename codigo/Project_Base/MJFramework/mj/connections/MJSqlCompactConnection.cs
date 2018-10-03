using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;

namespace MJFramework.mj.connections
{
    public class MJSqlCompactConnection
    {
        private SqlCeConnection SQLConnection;
        protected string ConnectionStringLocal;

        /// <summary>
        /// Constructor of the class 
        /// </summary>
        public MJSqlCompactConnection()
        {
            try
            {
                ConnectionStringLocal = ConfigurationManager.ConnectionStrings["ServerDb"].ConnectionString;
                //MySqlConnection SQLConexion = default(MySqlConnection);                
            }
            catch (Exception e)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringLocal = config.ConnectionStrings.ConnectionStrings["ServerDb"].ConnectionString.ToString();
                Console.WriteLine(e.ToString());
            }
            SQLConnection = new SqlCeConnection(ConnectionStringLocal); 
        }

        

        /// <summary>
        /// Method to return the connection instance for to use it in the controllers 
        /// </summary>
        private void openConexion()
        {
            try
            {
                if (SQLConnection.State == ConnectionState.Closed)
                    SQLConnection.Open();
            }
            catch (SqlCeException ex)
            {
                Console.WriteLine("Ocurrio un error  " + ex);
                closeConnection();
            }
        }


        /// <summary>
        /// Method to close connection 
        /// </summary>
        protected void closeConnection()
        {
            if (SQLConnection.State == ConnectionState.Open)
            {
                SQLConnection.Close();
                SQLConnection.Dispose();
            }
        }

        /// <summary>
        /// Method to execute SQL sentences, mainly, the sentences to changes information in the database
        /// </summary>
        /// <param name="sentencia">String with the Sql Sentence</param>
        /// <param name="parametros">List of paramater neccesary for execution </param>
        /// <returns>True if the sentence is sucess, on the contrary, return false if it presents an execption</returns>
        protected bool executeQueryUpdate(string sentencia, List<SqlCeParameter> parametros)
        {
            bool accept = true;
            openConexion();
            try
            {
                SqlCeCommand comando = new SqlCeCommand();
                comando.CommandText = sentencia;
                comando.CommandType = CommandType.Text;
                comando.Connection = SQLConnection;
                foreach (SqlCeParameter parametro in parametros)
                    comando.Parameters.Add(parametro);
                comando.ExecuteNonQuery();
                accept = true;
            }
            catch (SqlCeException e)
            {
                accept = false;
                Console.WriteLine("Error al tratar de ingresar al archivo");
                Console.WriteLine("Codigo error: " + e.ToString());
            }
            finally
            {
               closeConnection();
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
        protected bool executeStoredProcedureUpdate(string nombreProcedimiento, List<SqlCeParameter> parametros)
        {
            bool accept = true;
            openConexion();
            try
            {
                SqlCeCommand comando = new SqlCeCommand();
                comando.CommandText = nombreProcedimiento;
                comando.CommandType = CommandType.StoredProcedure;
                comando.Connection = SQLConnection;
                foreach (SqlCeParameter parametro in parametros)
                    comando.Parameters.Add(parametro);
                comando.ExecuteNonQuery();
                accept = true;
            }
            catch (SqlCeException e)
            {
                accept = false;
                Console.WriteLine("Error al tratar de ingresar al archivo");
                Console.WriteLine("Codigo error: " + e.ToString());
            }
            finally
            {
                closeConnection();
            }
            return accept;
        }

        /// <summary>
        /// Metodo principal para acceso a la base de datos retorna un reader mediante un procedimiento almacenado
        /// </summary>
        /// <param name="sentencia"></param>
        /// <param name="parametros"></param>
        /// <returns>SqlCeDataReader</returns>
        protected SqlCeDataReader buscaRegistroStoredProcedure(string nombreProcedimiento, List<SqlCeParameter> parametros)
        {
            SqlCeDataReader reader;
            try
            {
                openConexion();
                SqlCeCommand sqlComando = new SqlCeCommand();
                sqlComando.CommandText = nombreProcedimiento;
                sqlComando.CommandType = CommandType.StoredProcedure;
                sqlComando.Connection = SQLConnection;
                foreach (SqlCeParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                reader = sqlComando.ExecuteReader();
            }
            catch (SqlCeException ex)
            {
                Console.Write("ocurrio un error al leer " + ex);
                reader = null;
                closeConnection();
            }
            return reader;
        }

        /// <summary>
        /// Permite tener acceso a la base de datos mediante una sentencia SLQ y regresa un Reader con los datos
        /// </summary>
        /// <param name="sentencia"></param>
        /// <param name="parametros"></param>
        /// <returns>SqlCeDataReader</returns>

        protected SqlCeDataReader buscaRegistroSQL(string sentencia, List<SqlCeParameter> parametros)
        {
            SqlCeDataReader reader;
            try
            {
                openConexion();
                SqlCeCommand sqlComando = new SqlCeCommand();
                sqlComando.CommandText = sentencia;
                sqlComando.CommandType = CommandType.Text;
                sqlComando.Connection = SQLConnection;
                foreach (SqlCeParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                reader = sqlComando.ExecuteReader();
            }
            catch (SqlCeException ex)
            {
                Console.Write("ocurrio un error al leer " + ex);
                reader = null;
                closeConnection();
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
        protected DataTable getListSentenciaSQL(String sentencia, List<SqlCeParameter> parametros)
        {
            DataSet miDataSet = new DataSet();
            bool ban = false;
            try
            {
                openConexion();
                SqlCeCommand sqlComando = new SqlCeCommand();
                sqlComando.CommandText = sentencia;
                sqlComando.CommandType = CommandType.Text;
                sqlComando.Connection = SQLConnection;
                foreach (SqlCeParameter parametro in parametros)
                    sqlComando.Parameters.Add(parametro);
                SqlCeDataAdapter myAdap = new SqlCeDataAdapter(sqlComando);
                myAdap.Fill(miDataSet, "permitidos");
                sqlComando.Dispose();
                ban = true;
            }
            catch (SqlCeException ex)
            { Console.Write(ex); }
            finally
            { closeConnection(); }
            if (ban)
                return miDataSet.Tables[0];
            else
                return null;
        }
    }
}
