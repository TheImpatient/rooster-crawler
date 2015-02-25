using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    class Week
    {
        public Day[] days = new Day[6];
        public Week()
        {
            for (int i = 0;i < days.Length ; i++ )
            {
                days[i] = new Day();
            }
        }
    }
}
