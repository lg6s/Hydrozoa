using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WAMS.DataModels
{
    public class Action
    {
        public string Name { get; set; }
        public string PlanName { get; set; }
        public TimeSpan Duration { get; set; } // min
        public DateTime PrimaryCondition { get; set; } // Weekday && Time
        public UInt16 SecondaryCondition { get; set; } // humidity
    }
}
