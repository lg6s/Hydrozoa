using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WAMS.DataModels;

namespace WAMS.Services.PlanManagement
{
    public static class PlanContainer
    {
        private static bool FirstTime = true;
        private static ILogger _logger { set; get; }
        public static List<Plan> Container { get; set; }

        public static void Setup(ILoggerFactory loggerFactory)
        {
            if (FirstTime) {
                _logger = loggerFactory.CreateLogger("PlanContainer");
                Container = new List<Plan>();
                FirstTime = false;
            }
        }

        public static bool AddPlan(string Name, DateTime StartCondition, uint Duration, List<DataModels.Action> Elements = null)
        {
            if (Container.All(e => !(e.Name.Equals(Name)))) {
                Container.Add(new Plan() {
                    Name = Name,
                    StartCondition = StartCondition,
                    Duration = Duration,
                    Elements = Elements
                });
                return true;
            }
            return false;
        }

        public static bool AddAction(string Name, DataModels.Action Element)
        {
            if (Container.Any(e => e.Name.Equals(Name))) {
                if (Container.Where(e => e.Name.Equals(Name)).First().Elements.All(e => !(e.Name.Equals(Element.Name)))) {
                    Container.Where(e => e.Name.Equals(Name)).First().Elements.Add(Element);
                    return true;
                }
            }
            return false;
        }

        public static void RemovePlan(string Name)
        {
            Container.RemoveAll(e => e.Name.Equals(Name));
        }

        public static void RemoveAction(string PlanName, string ActionName)
        {
            if (Container.Any(e => e.Name.Equals(PlanName))) {
                Container.Where(e => e.Name.Equals(PlanName)).First().Elements.RemoveAll(e => e.Name.Equals(ActionName));
            }
        }
    }
}
