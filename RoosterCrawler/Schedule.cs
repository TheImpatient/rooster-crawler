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
            //TODO: checksum version database vs crawler for newer version of week / pull all weeks and save them per class
            ExternalWeek = DataParser.GetExternalWeekSchedule(11);
            //InternalWeek = DataParser.GetInternalWeekSchedule();


        }
    }
}
