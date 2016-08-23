using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace WAMS.Services.PlanManagement
{
    using DataModels;
    using static PlanContainer;
    public class PlanWorker
    {
        private static Timer PlanPeriod;

        private static Timer ActionPeriod;

        private static List<Plan> ActivePlans = new List<Plan>();
        private static ILogger _logger { set; get; }
        public PlanWorker(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);

            PlanPeriod = new Timer(6000000);
            PlanPeriod.AutoReset = true;
            PlanPeriod.Elapsed += PlanWorkerEvent;
            PlanPeriod.Enabled = true;

            ActionPeriod = new Timer(60000);
            ActionPeriod.AutoReset = true;
            ActionPeriod.Elapsed += ActionWorkerEvent;
            ActionPeriod.Enabled = ActivePlans.Count > 0;
        }

        private static void ActionWorkerEvent(object sender, ElapsedEventArgs k)
        {
            throw new NotImplementedException();
        }

        private static void PlanWorkerEvent(Object source, ElapsedEventArgs k)
        {
            ActivePlans.AddRange(Container.Where(e =>
                e.StartCondition.DayOfYear <= DateTime.Now.DayOfYear &&
                (e.StartCondition.DayOfYear + e.Duration) >= DateTime.Now.DayOfYear));

            if (ActionPeriod.Enabled == false && ActivePlans.Count > 0) { ActionPeriod.Enabled = true; }

            ActivePlans.RemoveAll(e => (e.StartCondition.DayOfYear + e.Duration) < DateTime.Now.DayOfYear);

        }
    }
}
