using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Week
    {
        public Day[] days = new Day[5];
        public int WeekNummer;

        public Week()
        {
            for (int i = 0; i < days.Length; i++)
            {
                days[i] = new Day();
            }
        }

        public Week(Day[] d)
        {
            for (int i = 0; i < days.Length; i++)
            {
                days[i] = (d[i] == null) ? new Day() : d[i];
            }
        }

        public Week GetTrimmedWeek(){
            Week w = this;
            Day[] ds = new Day[days.Length];
            for (int i = 0; i < ds.Length; i++)
            {
                ds[i] = new Day();
            }

            for(int j = 0; j< days.Length; j++){
                ds[j].lessen = days[j].lessen.Where(x => x.Docent != "" && x.Vak != "" && x.VakCode != "" && x.VakId != 0).ToArray<Les>();
            }
            w.days = ds;
            return w;
        }

        public bool Equals(Week w)
        {
            bool allDaysEqual = true;
            for(int i = 0; i< days.Length; i++)
            {
                if(!days[i].Equals(w.days[i])){
                    allDaysEqual = false;
                    break;
                }
            }
            return (this.WeekNummer == w.WeekNummer && allDaysEqual);
        }
    }
}
