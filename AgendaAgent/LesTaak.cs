using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgendaAgent
{
    public class LesTaak
    {
        public enum TaakAction
        {
            Create = 0,
            Update = 1,
            Delete = 2
        }
        public int Id { get; set; }
        public string LesGUID { get; set; }
        public Les Les { get; set; }
        public TaakAction Action { get; set; }
    }
}
