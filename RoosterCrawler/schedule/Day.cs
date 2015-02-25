using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    class Day
    {
        private String[] hours = new String[24];
        public Day()
        {

        }

        public void Add(String s)
        {
            for (int i = 0; i < hours.Length; i++ )
            {
                if (hours[i] == null)
                {
                    hours[i] = s != null ? s : "";
                    break;
                }

            }
        }

        public void AddAt(String s, int index)
        {
            hours[index] = s;
        }

        public bool Available(int index)
        {
            return hours[index] == null;
        }
    }
}
