using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Net;
using MySql.Data.MySqlClient;

namespace RoosterCrawler
{
    public static class DataParser
    {
        //TODO: catch webclient errors / sanitise input more / divide parts into functions for readability
        public static Week GetExternalWeekSchedule(int weekNummer, string klas)
        {
            WebClient webClient = new WebClient();

            //TODO: catch and handle 404exception, and other webclient exceptions
            String page = new WebClient().DownloadString("http://misc.hro.nl/roosterdienst/webroosters/cmi/kw3/" + weekNummer + "/c/" + klas + ".htm").Trim();

            //String page = System.IO.File.ReadAllText("c00084.htm").Trim();

            page = page.Replace("\r", String.Empty).Replace("\n", String.Empty);

            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(page);

            List<HtmlNode> table = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr/td").ToList();


            //Contains all data per week, day, hour
            Week week = new Week();
            week.WeekNummer = weekNummer;


            //Lists all elements innerText
            List<String> innerTextList = new List<String>();
            List<int> rowspanList = new List<int>();


            int rowCount = 0;
            int columnCount = 0;
            List<HtmlNode> tableRows = htmldoc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr").Skip(1).Where((x, i) => i % 2 == 0).ToList();
            IEnumerable<HtmlNode> tableDatas = new List<HtmlNode>();
            HtmlAttribute rowspan;
            foreach (HtmlNode tr in tableRows)
            {
                columnCount = 0;

                tableDatas = tr.ChildNodes.Skip(1);
                foreach (HtmlNode td in tableDatas)
                {
                    for (int i = 0; i < week.days.Length - columnCount; i++)
                    {
                        if (week.days[columnCount + i].Available(rowCount))
                        {


                            //Bepaal de lengte van de les
                            rowspan = td.Attributes.Where(x => x.Name.Equals("rowspan")).SingleOrDefault();
                            int rowvalue;
                            try
                            {
                                rowvalue = int.Parse(rowspan != null ? rowspan.Value : "EMPTY");
                            }
                            catch (FormatException)
                            {
                                rowvalue = 0;
                            }

                            //Check for bold text
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(td.InnerHtml);

                            if (td.InnerText != "" && doc.DocumentNode.SelectNodes("//tr").First().SelectNodes("td/font/b") != null) //er is een lokaal bekend
                            {
                                string[] a = td.InnerText.Split(')');
                                string[] b = a[0].Split(' ');

                                week.days[columnCount + i].Add(
                                    new Les()
                                    {
                                        Lokaal = b[0],
                                        Docent = b[1],
                                        Vak = a[1].Trim(),
                                        VakCode = b[2],
                                        VakId = int.Parse(b[3]),
                                        Lengte = rowvalue * 25
                                    });
                            }
                            else if (td.InnerText != "") //er is geen lokaal bekend
                            {
                                string[] a = td.InnerText.Split(')');
                                string[] b = a[0].Split(' ');

                                week.days[columnCount + i].Add(
                                    new Les()
                                    {
                                        Lokaal = String.Empty,
                                        Docent = b[0],
                                        Vak = a[1].Trim(),
                                        VakCode = b[1],
                                        VakId = int.Parse(b[2]),
                                        Lengte = rowvalue * 25
                                    });
                            }
                            else// geen les
                            {
                                week.days[columnCount + i].Add(
                                new Les()
                                {
                                    Lokaal = String.Empty,
                                    Docent = String.Empty,
                                    Vak = String.Empty,
                                    VakCode = String.Empty,
                                    VakId = 0,
                                    Lengte = 0
                                });
                            }

                            //Als de les langer dan 1 uur duurt, hou deze plaatsen bezet
                            if (rowvalue >= 4)
                            {
                                for (int j = 1; j < rowvalue / 2; j++)
                                {
                                    //Check for bold text
                                    week.days[columnCount + i].Add(
                                        new Les()
                                        {
                                            Lokaal = String.Empty,
                                            Docent = String.Empty,
                                            Vak = String.Empty,
                                            VakCode = String.Empty,
                                            VakId = 0,
                                            Lengte = 0
                                        });
                                }
                            }
                            break;
                        }
                    }
                    columnCount++;
                }
                rowCount++;
            }


            return week;
        }

        public static Week GetInternalWeekSchedule(int weekNummer, string klas)
        {
            const string connectionString = @Credentials.ConnectionString;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;

            List<Les> values = new List<Les>();
            List<DateTime> starts = new List<DateTime>();

            Week w = new Week();

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                //Gets all row within the current week within the same klas
                string query = "SELECT * FROM les WHERE start_tijd >= DATE_ADD(start_tijd , INTERVAL(1-DAYOFWEEK(start_tijd )) +1 DAY) AND start_tijd < ADDDATE(DATE_ADD(start_tijd, INTERVAL(1-DAYOFWEEK(start_tijd)) +1 DAY), INTERVAL 7 DAY)AND klas = '" + klas + "' ORDER BY start_tijd ASC;";
                var sqlCommand = new MySqlCommand(query, connection);
                reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    values.Add(new Les(reader.GetString(7), reader.GetString(1), reader.GetString(3), reader.GetInt32(4), reader.GetString(2), (int)(reader.GetTimeSpan(6).TotalMinutes)));
                    starts.Add(reader.GetDateTime(5));
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


            Day[] d = new Day[5];
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = new Day();
            }

            for(int i = 0; i < values.Count; i++){
                d[(int)((starts[i].DayOfWeek)-1)].Add(values[i]);
            }
            for (int i = 0; i < d.Length; i++)
            {
                for (int j = 0; j < d[i].lessen.Count(); j++)
                {
                    d[i].lessen[j] = (d[i].lessen[j] == null) ? new Les(): d[i].lessen[j];
                }
            }

            w.days = d;
            w.WeekNummer = weekNummer;

            return w;
        }

        public static List<TaskSchedular.CrawlTask> GetCrawlSchedule()
        {
            const string connectionString = @Credentials.ConnectionString;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            string log = String.Empty;
            List<TaskSchedular.CrawlTask> list = new List<TaskSchedular.CrawlTask>();
            var jss = new JavaScriptSerializer();

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = String.Format("SELECT * FROM crawl_schedule WHERE date = '{0}'", DateTime.Now.ToString("yyyy-MM-dd"));

                var sqlCommand = new MySqlCommand(query, connection);
                reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    var task = jss.Deserialize<TaskSchedular.CrawlTask>((string)reader.GetValue(1));
                    task.Id = (int)reader.GetValue(0);
                    task.Datetime = (DateTime)reader.GetValue(4);
                    task.Datetime = task.Datetime.Add((TimeSpan) reader.GetValue(5));
                    task.Interval = (int)reader.GetValue(2);
                    task.Weken = (int)reader.GetValue(3);

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
        
        public static UpdateResult UpdateInternalData(string query)
        {
            const string connectionString = @Credentials.ConnectionString;
            MySqlConnection connection = null;
            MySqlDataReader reader = null;
            bool completed = true;
            string log = String.Empty;

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
                log = err.ToString();
                completed = false;
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

            return new UpdateResult() { Completed = completed, Log = log };
        }
    }
}
