using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Day
    {
        public Les[] lessen = new Les[15];
        public Day()
        {

        }

        public void Add(Les s)
        {
            for (int i = 0; i < lessen.Length; i++ )
            {
                if (lessen[i] == null)
                {
                    lessen[i] = s != null ? s : new Les(); // <<dit is vragen om problemen thijs
                    break;
                }
            }
        }

        public void AddAt(Les s, int index)
        {
            lessen[index] = s;
        }

        public bool Available(int index)
        {
            return lessen[index] == null;
        }
    }
}
