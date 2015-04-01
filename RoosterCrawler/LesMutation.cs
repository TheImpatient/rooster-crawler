﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoosterCrawler
{
    public class LesMutation
    {
        public enum Mutation { CREATE = 0, UPDATE = 1, DELETE = 2 }

        public Mutation Type{ get; set;}
        public Les MLes{ get; set;}
        public LesMutation(Mutation m, Les l)
        {
            this.Type = m;
            this.MLes = l;
        }
    }
}
