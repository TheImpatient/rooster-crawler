using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Day
    {
        private Les[] _lessen = new Les[15];
        public Day()
        {

        }

        public void Add(Les s)
        {
            for (int i = 0; i < _lessen.Length; i++ )
            {
                if (_lessen[i] == null)
                {
                    _lessen[i] = s != null ? s : new Les(); // <<dit is vragen om problemen thijs
                    break;
                }
            }
        }

        public void AddAt(Les s, int index)
        {
            _lessen[index] = s;
        }

        public bool Available(int index)
        {
            return _lessen[index] == null;
        }
    }
}
