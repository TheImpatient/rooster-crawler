using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class Les
    {
        public int InternalId { get; set; }
        public string Lokaal { get; set; }
        public string Docent { get; set; }
        public string VakCode { get; set; }
        public int VakId { get; set; }
        public string Vak { get; set; }
        public int Lengte { get; set; }
        public string StartTijd { get; set; }
        public string CalendarGuid { get; set; }
        public string LesGuid { get; set; }

        public Les(string _Lokaal, string _Docent, string _VakCode, int _VakId, string _Vak, int _Lengte, int _InternalId,string les_guid ,string calendar_guid = "")
        {
            InternalId = _InternalId;
            Lokaal = _Lokaal;
            Docent = _Docent;
            VakCode = _VakCode;
            VakId = _VakId;
            Vak = _Vak;
            Lengte = _Lengte;
            CalendarGuid = calendar_guid;
            LesGuid = les_guid;
        }

        public Les()
        {
        }

        public bool Equals(Les l)
        {
            return (this.Lokaal == l.Lokaal && this.Docent == l.Docent && this.VakCode == l.VakCode && this.VakId == l.VakId && this.Vak == l.Vak && this.Lengte == l.Lengte);
        }

        public bool PartiallyEquals(Les l)
        {
            return (this.VakCode == l.VakCode && this.VakId == l.VakId);
        }
    }
}
