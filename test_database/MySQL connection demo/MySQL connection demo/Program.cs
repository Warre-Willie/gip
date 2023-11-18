//Tutorial: https://www.youtube.com/watch?v=n1QarlZj3lM&ab_channel=ProgrammingGuru
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQL_connection_demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string server = "localhost";
            string database = "demo school";
            string username = "root";
            string password = "";
            string constring = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";PASSWORD=" + password + ";";

            MySqlConnection conn = new MySqlConnection(constring);
            conn.Open();

            string query = "SELECT * FROM persoongegevens";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader["voornaam"]);
                Console.WriteLine(reader["achternaam"]);
                Console.WriteLine("------------------");
            }
        }
    }
}
