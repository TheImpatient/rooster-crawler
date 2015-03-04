﻿using HtmlAgilityPack;
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
            _schedule = new Schedule();


            const string connectionString = @Credentials.ConnectionString;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string deelnemers = String.Empty;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = WeekToQuery(_schedule.ExternalWeek);
                var sqlCommand = new MySqlCommand(query, connection);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
                /*
                reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    deelnemers = (string)reader.GetValue(1);
                    
                }
                */
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

            connection.Close();
            //Week foo = DataParser();
            //Console.WriteLine(foo);
            Console.Read();
        }
        private String WeekToQuery(Week week)
        {
            /*
             INSERT INTO les
            (docent, vak, vak_code, vak_id, start_tijd, lengte)
            VALUES
            (docent, vak, vak_code, vak_id, start_tijd, lengte),
            (docent, vak, vak_code, vak_id, start_tijd, lengte),
            (docent, vak, vak_code, vak_id, start_tijd, lengte),
            (docent, vak, vak_code, vak_id, start_tijd, lengte);
             */

            String query = "INSERT INTO les (docent, vak, vak_code, vak_id, start_tijd, lengte) VALUES";

            foreach (Day d in week.Days)
            {

                foreach (Les l in d.lessen)
                {
                    if (l.Docent != "" && l.Vak != "" && l.VakCode != "" && l.VakId != 0)
                    {
                        query += "('" + l.Docent + "', '" + l.Vak + "', '" + l.VakCode + "', " + l.VakId + ", '2000-12-31 23:59:59', '00:50:00'),";
                    }
                }
            }
            query = query.Remove(query.Length - 1) + ";";

            return query;
        }
    }
}
