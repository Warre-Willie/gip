using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace crowd_management.classes
{
    public class DbRepository
    {
			private const string ConnString = "SERVER=localhost;DATABASE=crowd_management;UID=root;PASSWORD=gip-WJ;";
			private readonly MySqlConnection _conn;
			private readonly MySqlCommand _cmd = new MySqlCommand();

        public DbRepository()
        {
            _conn = new MySqlConnection(ConnString);
            _conn.Open();
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }
        }

        public DataTable SqlExecuteReader(string query)
        {
            DataTable dt = new DataTable();

            try
            {
                _cmd.Connection = _conn;
                _cmd.CommandText = query;

                MySqlDataReader reader = _cmd.ExecuteReader();
                dt.Load(reader);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return dt;
        }

        public void SqlExecute(string query)
        {
            try
            {
                _cmd.Connection = _conn;
                _cmd.CommandText = query;
                _cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}