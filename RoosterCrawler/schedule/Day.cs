using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Day
    {
        private Les[] _hours = new Les[15];
        public Day()
        {

        }

        public void Add(Les s)
        {
            for (int i = 0; i < _hours.Length; i++ )
            {
                if (_hours[i] == null)
                {
                    _hours[i] = s != null ? s : new Les(); // <<dit is vragen op problemen thijs
                    break;
                }

            }
        }

        public void AddAt(Les s, int index)
        {
            _hours[index] = s;
        }

        public bool Available(int index)
        {
            return _hours[index] == null;
        }
    }
}
