﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WAMS.DataModels
{
    public class Plan
    {
        public string Name { get; set; }
        public DateTime StartCondition { get; set; } // day of year
        public uint Duration { get; set; } // days
        public List<Action> Elements { get; set; }
    }
}
