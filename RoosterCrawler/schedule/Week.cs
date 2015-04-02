using System;
using System.Collections.Generic;
using System.Globalization;
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

        public Week GetTrimmedWeek()
        {
            Week w = this;
            Day[] ds = new Day[days.Length];
            for (int i = 0; i < ds.Length; i++)
            {
                ds[i] = new Day();
            }

            for (int j = 0; j < days.Length; j++)
            {
                ds[j].lessen = days[j].lessen.Where(x => x.Docent != "" && x.Vak != "" && x.VakCode != "" && x.VakId != 0).ToArray<Les>();
                int l = 0;
                for (int k = 0; k < days[j].lessen.Length; k++)
                {

                    if (days[j].lessen[k].Docent != "" && days[j].lessen[k].Vak != "" && days[j].lessen[k].VakCode != "" && days[j].lessen[k].VakId != 0)
                    {
                        if (ds[j].lessen[l].StartTijd == null)
                        {
                            ds[j].lessen[l].StartTijd = Util.FirstDateOfWeek(2015, WeekNummer, new TimeSpan(j, 0, Util.schoolHours[k % days[j].lessen.Length], 0));
                        }
                        l++;
                    }
                }
            }
            w.days = ds;
            return w;
        }

        public List<Les> getLessen()
        {
            List<Les> allLessen = new List<Les>();

            for (int i = 0; i < days.Length; i++)
            {
                allLessen.AddRange(days[i].lessen);
            }
            return allLessen;
        }

        public bool Equals(Week w)
        {
            bool allDaysEqual = true;
            for (int i = 0; i < days.Length; i++)
            {
                if (!days[i].Equals(w.days[i]))
                {
                    allDaysEqual = false;
                    break;
                }
            }
            return (this.WeekNummer == w.WeekNummer && allDaysEqual);
        }

        public List<LesMutation> GetMutations(Week w)
        {
            List<LesMutation> ls = new List<LesMutation>();

            List<Les> allExterLessen = getLessen();
            List<Les> allInterLessen = w.getLessen();

            List<Les> removeables = allInterLessen;

            bool contains;

            //Compare all external lessen with the internal ones
            for (int i = 0; i < allExterLessen.Count; i++)
            {
                contains = false;
                for (int j = 0; j < allInterLessen.Count; i++)
                {
                    if (allExterLessen[i].Equals(allInterLessen[j]))
                    {
                        contains = true;
                        removeables.RemoveAt(j);
                        break;
                    }
                    //ALTER ROW
                    if (allExterLessen[i].PartiallyEquals(allInterLessen[j]))
                    {
                        allExterLessen[i].InternalId = allInterLessen[i].InternalId;
                        ls.Add(new LesMutation(LesMutation.Mutation.UPDATE, allExterLessen[i]));
                        contains = true;
                        removeables.RemoveAt(j);
                        break;
                    }
                }
                //INSERT ROW
                if (!contains)
                {
                    ls.Add(new LesMutation(LesMutation.Mutation.CREATE, allExterLessen[i]));
                }
            }
            //DELETE ROW remaining
            foreach (Les l in removeables)
            {
                ls.Add(new LesMutation(LesMutation.Mutation.DELETE, l));
            }

            return ls;
        }
    }
}
