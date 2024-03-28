using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace crowd_management.classes
{
    public class DbRepository
    {
        const string constring = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
        MySqlConnection conn;
        MySqlCommand cmd = new MySqlCommand();

        public DbRepository()
        {
            conn = new MySqlConnection(constring);
            conn.Open();
        }

        public void Dispose()
        {
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public DataTable SQLExecuteReader(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = query;

                MySqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public void SQLExecute(string query)
        {
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}