using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;

namespace RoosterCrawler
{
    public class Program
    {
        private Schedule _schedule;

        public void Main()
        {
            //_schedule = new Schedule();


            const string connectionString = @"Server=195.8.208.128;Port=3351;Database=rooster;Uid=crawler;Pwd=g!MqEFCXbVK0P3hv^Jy5;";
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string deelnemers = String.Empty;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                const string query = "SELECT * FROM les";
                var sqlCommand = new MySqlCommand(query, connection);
                reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    deelnemers = (string)reader.GetValue(1);
                    
                }
            }
            catch (MySqlException err)
            {
                Console.WriteLine("Error: " + err.ToString());
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (connection != null)
                {
                    connection.Close();
                }
            }


            //Week foo = DataParser();
            //Console.WriteLine(foo);
            Console.Read();
        }

    }
}
