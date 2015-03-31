using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class LesMutation
    {
        public enum Mutation { CREATE, UPDATE, DELETE }

        public Mutation Order{ get; set;}
        public Les NewLes{ get; set;}
        public LesMutation(Mutation m, Les l)
        {
            this.Order = m;
            this.NewLes = l;
        }
    }
}
