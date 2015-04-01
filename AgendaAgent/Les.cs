using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgendaAgent
{
    public class Les
    {
        public string Lokaal { get; set; }
        public string Docent { get; set; }
        public string VakCode { get; set; }
        public int VakId { get; set; }
        public string Vak { get; set; }
        public TimeSpan Lengte { get; set; }
        public DateTime StartTijd { get; set; }
        public string Klas { get; set; }
        public string Guid { get; set; }
    }
}
