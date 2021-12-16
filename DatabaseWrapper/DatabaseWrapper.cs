using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data;

namespace DataBaseWrapper
{
    public class DataBase_Secure
    {
        OdbcConnection connection;

        public DataBase_Secure(string connStrg)
        {
            connection = new OdbcConnection(connStrg);
        }

        public bool ConnectionIsOpen
        {
            get
            {
                return connection.State == ConnectionState.Open;
            }
        }

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public object RunQueryScalar(string sqlCmd, params string[] parameter)
        {
            object value;
            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = connection;
            cmd.CommandText = sqlCmd;
            foreach (string s in parameter)
            {
                cmd.Parameters.AddWithValue("?", s);
            }
            bool isConnectionInitiallyClosed = connection.State == ConnectionState.Closed;
            if (isConnectionInitiallyClosed)
            {
                Open();
            }
            try
            {
                value = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isConnectionInitiallyClosed)
                {
                    Close();
                }
            }
            return value;
        }

        public DataTable RunQuery(string sqlCmd, params string[] parameter)
        {
            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = connection;
            cmd.CommandText = sqlCmd;
            DataTable resultSet = new DataTable();

            foreach (string s in parameter)
            {
                cmd.Parameters.AddWithValue("?", s);
            }
            OdbcDataAdapter da = new OdbcDataAdapter(cmd);

            da.Fill(resultSet);
            return resultSet;
        }

        public int RunNonQuery(string sqlCmd, params string[] parameter)
        {
            int numRecs;
            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = connection;
            cmd.CommandText = sqlCmd;
            foreach (string s in parameter)
            {
                cmd.Parameters.AddWithValue("?", s);
            }
            bool isConnectionInitiallyClosed = connection.State == ConnectionState.Closed;
            if (isConnectionInitiallyClosed)
            {
                Open();
            }
            try
            {
                numRecs = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (isConnectionInitiallyClosed)
                {
                    Close();
                }
            }
            return numRecs;
        }

        public string TryToConnect()
        {
            try
            {
                Open();
                return "Verbindung OK.";
            }
            catch (Exception)
            {
                return "Kann nicht zur Datenbank verbinden.";
            }
            finally
            {
                Close();
            }
        }
    }
}

