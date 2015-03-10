using System;
using System.Globalization;

namespace RoosterCrawler
{
    public class Schedule
    {
        public Week ExternalWeek;
        public Week InternalWeek;
        public String Klas;

        public Schedule(string weken, string klas)
        {
            Klas = klas;

            InternalWeek = DataParser.GetInternalWeekSchedule(weken, klas);
            ExternalWeek = DataParser.GetExternalWeekSchedule(weken, klas);
        }

        /// <summary>
        /// compares internal and external schedules
        /// </summary>
        /// <returns>true = the compared schedules are equal. false = schedules are not equal</returns>
        public bool Compare()
        {
            //Als het externe rooster anders is dan ons interne rooster over schrijven we gewoon de hele week

            //Trim de weken eerst voordat hij compared kan worden
            var internalWeekTrimmed = InternalWeek.GetTrimmedWeek();
            var externalWeekTrimmed = ExternalWeek.GetTrimmedWeek();

            return externalWeekTrimmed.Equals(internalWeekTrimmed);
        }

        public UpdateResult Synchronize()
        {
            string query = WeekToQuery(ExternalWeek, Klas);
            return DataParser.UpdateInternalData(query);
        }

        private string WeekToQuery(Week week, String klas)
        {
            /*
             INSERT INTO les
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas)
            VALUES
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas),
            (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas);
             */
            var schoolHours = new[] 
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

            String query = "INSERT INTO les (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas) VALUES";

            int dayIndex = 0;
            int lesIndex = 0;

            foreach (Day d in week.days)
            {
                foreach (Les l in d.lessen)
                {

                    if (l.Docent != "" && l.Vak != "" && l.VakCode != "" && l.VakId != 0)
                    {
                        query += "('" + l.Docent + "', '" + l.Vak + "', '" + l.VakCode + "', " + l.VakId + ", '" + FirstDateOfWeek(2015, week.WeekNummer, new TimeSpan(dayIndex, 0, schoolHours[lesIndex % 15], 0)) + "', '" + new TimeSpan(0, 0, l.Lengte, 0).ToString(@"hh\:mm\:ss") + "' , '" + l.Lokaal + "' , '" + Klas + "'),";
                    }

                    lesIndex++;
                }
                dayIndex++;
            }
            query = query.Remove(query.Length - 1) + ";";

            return query;
        }

        private string FirstDateOfWeek(int year, int weekOfYear, TimeSpan offSet)
        {
            var jan1 = new DateTime(year, 1, 1);
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
