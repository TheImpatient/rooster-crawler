using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace RoosterCrawler
{
    public class Program
    {
        private Schedule _schedule;

        public void Main()
        {
            //start task schedular
            // > task schedular 
            // | haal informatie op welke rooster onderdelen gecrawlt moet worden en om de hoeveel tijd dit moet gebeuren , crawl info van db table 'crawl_schedule'
            // | op gedicteerde tijden start de task schedular de rooster crawler en geeft mee welke rooster onderdelen gecrawlt moeten worden
            // > rooster crawler
            // | haalt extern rooster op (voegtoe: html pagina nummer dat opgeroepen is)
            // | haalt intern rooster op (haalop: where html pagina nummer zie boventstaand)
            // | vergelijkt extern met intern en update het interne rooster indien nodig (zorg ervoor dat je de juiste week planningen met elkaar vergelijkt)


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
            int[] schoolHours = new int[15] 
            {
                510,  // 8:30
                560,  // 9:20
                630,  //10:30
                680,  //11:20
                730,  //12:10
                780,  //13:00
                830,  //13:50
                900,  //15:00
                950,  //15:50
                1020, //17:00
                1070, //17:50
                1120, //18:40
                1170, //19:30
                1220, //20:20
                1270  //21:10
            };

            String query = "INSERT INTO les (docent, vak, vak_code, vak_id, start_tijd, lengte) VALUES";

            int dayIndex = 0;
            int lesIndex = 0;

            foreach (Day d in week.Days)
            {
                foreach (Les l in d.lessen)
                {

                    if (l.Docent != "" && l.Vak != "" && l.VakCode != "" && l.VakId != 0)
                    {
                        query += "('" + l.Docent + "', '" + l.Vak + "', '" + l.VakCode + "', " + l.VakId + ", '" + FirstDateOfWeek(2015, week.WeekNummer, new TimeSpan(dayIndex, 0, schoolHours[lesIndex%15], 0)) + "', '" + new TimeSpan(0, 0, l.Lengte, 0).ToString(@"hh\:mm\:ss") + "'),";
                    }

                    lesIndex++;
                }
                dayIndex++;
            }
            query = query.Remove(query.Length - 1) + ";";

            return query;
        }

        public String FirstDateOfWeek(int year, int weekOfYear, TimeSpan offSet)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int weekNum = weekOfYear;
            weekNum -= firstWeek <= 1 ? 1 : 0;
            return firstThursday.AddDays(weekNum * 7).AddDays(-3).Add(offSet).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
