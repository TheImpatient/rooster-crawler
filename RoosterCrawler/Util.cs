using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public static class Util
    {
        public static int[] schoolHours = new int[15] 
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
        public static string FirstDateOfWeek(int year, int weekOfYear, TimeSpan offSet)
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

        public static string FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int weekNum = weekOfYear;
            weekNum -= firstWeek <= 1 ? 1 : 0;
            return firstThursday.AddDays(weekNum * 7).AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int GetWeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
