using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace AgendaAgent
{
    internal static class Repository
    {
        private static readonly string _serviceAccountEmail = "177600524325-6sbfabk2ldv0ickbpjtvfjqbieagg289@developer.gserviceaccount.com";
        private static readonly string _privateKeyPassword = "notasecret";
        internal static readonly string _agendaId = "roosterify@gmail.com";
        private static readonly string ApplicationName = "Roosterify";
        private static CalendarService _service;
        private static Events _agenda;

        private static CalendarService GoogleCalendar()
        {
            var certificate = new X509Certificate2(Properties.Resources.Roosterify_b75ac5863d29, _privateKeyPassword, X509KeyStorageFlags.Exportable);

            var credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(_serviceAccountEmail)
                {
                    Scopes = new[] { CalendarService.Scope.Calendar }
                }.FromCertificate(certificate));

            return new CalendarService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
        }

        public static List<LesTaak> GetAgendaTasks()
        {
            const string connectionString = "Server=195.8.208.128;Port=3351;Database=rooster;Uid=crawler;Pwd=g!MqEFCXbVK0P3hv^Jy5;";
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string log = String.Empty;
            var list = new List<LesTaak>();

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = String.Format("SELECT * FROM agenda_tasks");

                var sqlCommand = new MySqlCommand(query, connection);
                reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    var task = new LesTaak
                    {
                        Id = reader.GetInt32(0),
                        Les = String.IsNullOrEmpty(reader.GetString(2)) ? new Les() : JsonConvert.DeserializeObject<Les>(reader.GetString(2)),
                        Action = reader.GetInt32(1) == 0 ? LesTaak.TaakAction.Create : reader.GetInt32(1) == 1 ? LesTaak.TaakAction.Update : LesTaak.TaakAction.Delete
                    };

                    list.Add(task);
                }

                connection.Close();
            }
            catch (MySqlException err)
            {
                log = err.ToString();
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

            return list;
        }

        public static void UpdateInternalData(string query)
        {
            const string connectionString = "Server=195.8.208.128;Port=3351;Database=rooster;Uid=crawler;Pwd=g!MqEFCXbVK0P3hv^Jy5;";
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();


                var sqlCommand = new MySqlCommand(query, connection);

                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException err)
            {
                Console.WriteLine(err.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        public static bool DeleteAgendaTask(int id)
        {
            const string connectionString = "Server=195.8.208.128;Port=3351;Database=rooster;Uid=crawler;Pwd=g!MqEFCXbVK0P3hv^Jy5;";
            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = String.Format("DELETE FROM agenda_tasks WHERE id = {0}",id);

                var sqlCommand = new MySqlCommand(query, connection);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();

                connection.Close();
            }
            catch (MySqlException err)
            {
                Console.WriteLine(err.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
            return true;
        }

        public static bool GetData()
        {
            _service = GoogleCalendar();
            _agenda = _service.Events.List(_agendaId).Execute();
            return true;
        }

        public static Events GetAgenda()
        {
            return _agenda;
        }

        public static CalendarService GetCalendar()
        {
            return _service;
        }
    }
}
