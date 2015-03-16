using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RoosterCrawler
{
    public class Schedule
    {
        public Week ExternalWeek;
        public Week InternalWeek;
        public String klas;

        public Schedule(int weken, string _klas)
        {
            klas = _klas;

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
            Week InternalWeekTrimmed = InternalWeek.GetTrimmedWeek();
            Week ExternalWeekTrimmed = ExternalWeek.GetTrimmedWeek();

            return ExternalWeekTrimmed.Equals(InternalWeekTrimmed);
        }

        public UpdateResult Synchronize()
        {
            string query = WeekToQuery(ExternalWeek, klas);
            return DataParser.UpdateInternalData(query);
        }

        private string WeekToQuery(Week week, String _klas)
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

            String query = "INSERT INTO les (docent, vak, vak_code, vak_id, start_tijd, lengte, lokaal, klas) VALUES";

            int dayIndex = 0;
            int lesIndex = 0;

            foreach (Day d in week.days)
            {
                foreach (Les l in d.lessen)
                {

                    if (l.Docent != "" && l.Vak != "" && l.VakCode != "" && l.VakId != 0)
                    {
                        query += "('" + l.Docent + "', '" + l.Vak + "', '" + l.VakCode + "', " + l.VakId + ", '" + Util.FirstDateOfWeek(2015, week.WeekNummer, new TimeSpan(dayIndex, 0, schoolHours[lesIndex % 15], 0)) + "', '" + new TimeSpan(0, 0, l.Lengte, 0).ToString(@"hh\:mm\:ss") + "' , '" + l.Lokaal + "' , '" + klas + "'),";
                    }

                    lesIndex++;
                }
                dayIndex++;
            }
            query = query.Remove(query.Length - 1) + ";";

            return Regex.Replace(query, @"[\r\n\x00\x1a\\'""]", @"\$0");
        }
    }
}
