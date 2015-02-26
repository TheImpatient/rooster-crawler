using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Day
    {
        private String[] _hours = new String[24];
        public Day()
        {

        }

        public void Add(String s)
        {
            for (int i = 0; i < _hours.Length; i++ )
            {
                if (_hours[i] == null)
                {
                    _hours[i] = s != null ? s : "";
                    break;
                }

            }
        }

        public void AddAt(String s, int index)
        {
            _hours[index] = s;
        }

        public bool Available(int index)
        {
            return _hours[index] == null;
        }
    }
}
