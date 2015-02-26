using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Schedule
    {
        public Week ExternalWeek;
        public Week InternalWeek;
        
        public Schedule()
        {
            ExternalWeek = DataParser.GetExternalWeekSchedule();
            //InternalWeek = DataParser.GetInternalWeekSchedule();
        }
    }
}
