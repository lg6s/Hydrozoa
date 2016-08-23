using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WAMS.DataModels
{
    public class Action
    {
        public string Name { get; set; }
        public DateTime PrimaryCondition { get; set; }
        public TimeSpan Duration { get; set; }
        public Int16 SecondaryCondition { get; set; }
    }
}
