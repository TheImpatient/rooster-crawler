﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Week
    {
        public Day[] Days = new Day[5];
        public int WeekNummer;

        public Week()
        {
            for (int i = 0; i < Days.Length; i++)
            {
                Days[i] = new Day();
            }
        }
    }
}
