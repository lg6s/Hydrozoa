using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace WAMS.Services.PlanManagement
{
    using DataModels;
    using GPIOAccess;
    using static PlanContainer;
    public class PlanWorker
    {
        private static Timer PlanPeriod;

        private static Timer ActionPeriod;

        private static List<Plan> ActivePlans = new List<Plan>();
        private static ILogger _logger { set; get; }
        private static IValve _Valve { get; set; }

        public PlanWorker(ILoggerFactory loggerFactory, IValve Valve)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
            _Valve = Valve;

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
            DateTime Now = DateTime.Now;
            Action PlannedAction = ActivePlans.SelectMany(e => e.Elements).Where(e => !(e.Equals(ActiveAction))
                && e.PrimaryCondition.DayOfWeek <= Now.DayOfWeek && e.PrimaryCondition.DayOfWeek + e.Duration.Days >= Now.DayOfWeek
                && e.PrimaryCondition.Hour <= Now.Hour && e.PrimaryCondition.Hour + e.Duration.TotalHours >= Now.Hour
                && e.PrimaryCondition.Minute <= Now.Minute && e.PrimaryCondition.Minute + e.Duration.TotalMinutes >= Now.Minute).FirstOrDefault();

            if(PlannedAction != null) {
                _Valve.OpenFor(PlannedAction.Duration);
                ActiveAction = PlannedAction;
            }
        }

        private static void PlanWorkerEvent(Object source, ElapsedEventArgs k)
        {
            ActivePlans.AddRange(Container.Where(e =>
                e.StartCondition <= DateTime.Now.DayOfYear &&
                (e.StartCondition + e.Duration.Days) >= DateTime.Now.DayOfYear));

            ActivePlans.RemoveAll(e => (e.StartCondition + e.Duration.Days) < DateTime.Now.DayOfYear);

            if (ActionPeriod.Enabled == true && ActivePlans.Count == 0) { ActionPeriod.Enabled = false; }
            else if (ActionPeriod.Enabled == false && ActivePlans.Count > 0) { ActionPeriod.Enabled = true; }

        }
    }
}
