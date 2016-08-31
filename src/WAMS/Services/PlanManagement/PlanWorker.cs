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
    public static class PlanWorker
    {
        private static Timer PlanPeriod;

        private static Timer ActionPeriod;

        private static bool SetupRun = true;

        private static List<Plan> ActivePlans = new List<Plan>();
        private static ILogger _logger { set; get; }
        private static IValve _Valve { get; set; }

        public static void Setup(ILoggerFactory loggerFactory, IValve Valve)
        {
            _logger = loggerFactory.CreateLogger("WAMS.PlanWorker");
            _Valve = Valve;

            PlanPeriod = new Timer(120000);
            PlanPeriod.AutoReset = true;
            PlanPeriod.Elapsed += PlanWorkerEvent;
            PlanPeriod.Enabled = true;

            ActionPeriod = new Timer(60000);
            ActionPeriod.AutoReset = true;
            ActionPeriod.Elapsed += ActionWorkerEvent;
            ActionPeriod.Enabled = ActivePlans.Count > 0;
        }

        private static void ActionWorkerEvent(Object sender, ElapsedEventArgs k)
        {
            DateTime Now = DateTime.Now;
            Action PlannedAction = ActivePlans.SelectMany(e => e.Elements).Where(e => !(e.Equals(ActiveAction))
                && e.PrimaryCondition.DayOfWeek <= Now.DayOfWeek && e.PrimaryCondition.DayOfWeek + e.Duration.Days >= Now.DayOfWeek
                && e.PrimaryCondition.Hour <= Now.Hour && e.PrimaryCondition.Hour + e.Duration.TotalHours >= Now.Hour
                && e.PrimaryCondition.Minute <= Now.Minute && e.PrimaryCondition.Minute + e.Duration.TotalMinutes >= Now.Minute).FirstOrDefault();

            if (PlannedAction != null && ActiveAction == null) {
                _Valve.OpenFor(PlannedAction.Duration);
                ActiveAction = PlannedAction;
            } else {
                _logger.LogWarning("Collision of two actions emerged, going to notice the user !");
                APIController.PlanActionController.Warnings.Add(
                    new Tuple<string, DateTime>("Warnung es gab einen Resourcenkonflikt zweier von ihnen geplanten Aktionen: " + ActiveAction.Name
                    + ", " + PlannedAction.Name + " | " + DateTime.Now.ToString(), DateTime.Now));
            }

            _logger.LogInformation("ActionWorker Elapsed successfully !");
        }

        private static void PauseTimers()
        {
            PlanPeriod.Stop();
            ActionPeriod.Stop();
            ToggleBackupPeriod();
            Task.Delay(30000);
            ToggleBackupPeriod();
            ActionPeriod.Start();
            PlanPeriod.Start();
        }

        internal static bool AlterPlan(string OldPlanName, Plan NewPlan)
        {
            if (Container.Any(e => e.Name.Equals(OldPlanName))) {
                Plan OldPlan = ActivePlans.Where(e => e.Name.Equals(OldPlanName)).First();
                PauseTimers();
                lock (Container) {
                    if (OldPlan.IsActive()) {
                        lock (ActivePlans) {
                            if (ActiveAction.PlanName.Equals(OldPlanName) && OldPlanName != NewPlan.Name) {
                                lock (ActiveAction) { ActiveAction.PlanName = NewPlan.Name; }
                            }
                            ActivePlans.Remove(OldPlan);
                            ActivePlans.Add(NewPlan);
                        }
                    }
                    Container.Remove(OldPlan);
                    Container.Add(NewPlan);
                }
                return true;
            }
            return false;
        }

        internal static bool RemovePlan(string Name)
        {
            if (ActivePlans.Any(e => !(e.Name.Equals(Name)))) { return false; } else {
                ActivePlans.RemoveAll(e => e.Name.Equals(Name));

                if (ActionPeriod.Enabled == true && ActivePlans.Count == 0) {
                    ActionPeriod.Enabled = false;
                    _logger.LogInformation("ActionPeriod was deactivated (again), due to the removal of the only active Plan !");
                }

                return true;
            }
        }

        internal static bool RemoveAction(string PlanName, string Name)
        {
            if (ActivePlans.Any(e => !(e.Name.Equals(PlanName)))) { return false; } else {
                if (!ActivePlans.Where(e => e.Name.Equals(PlanName)).First().Elements.Any(e => e.Name.Equals(Name)))
                { return false; }

                ActivePlans.Where(e => e.Name.Equals(PlanName)).First().Elements.RemoveAll(e => e.Name.Equals(Name));
                ActivePlans.RemoveAll(e => e.Elements.Count == 0);

                if (ActionPeriod.Enabled == true && ActivePlans.Count == 0) {
                    ActionPeriod.Enabled = false;
                    _logger.LogInformation("ActionPeriod was deactivated (again), due to the removal of the only active Plan !");
                }

                return true;
            }
        }

        internal static bool IsActive(this Plan p) { return ActivePlans.Any(e => e.Equals(p)); }

        internal static bool TogglePlan(string Name)
        {
            if (ActivePlans.Any(e => e.Name.Equals(Name))) {
                ActivePlans.RemoveAll(e => e.Name.Equals(Name));
            } else if (Container.Any(e => e.Name.Equals(Name))) {
                ActivePlans.Add(Container.Where(e => e.Name.Equals(Name)).First());
            } else { return false; }
            return true;
        }

        private static void PlanWorkerEvent(Object source, ElapsedEventArgs k)
        {
            ActivePlans.AddRange(Container.Where(e =>
                e.StartCondition <= DateTime.Now.DayOfYear
                && (e.StartCondition + e.Duration.Days) >= DateTime.Now.DayOfYear
                && e.Elements.Any())); // warn the user that plans don't get activated when they have no actions

            ActivePlans.RemoveAll(e => (e.StartCondition + e.Duration.Days) < DateTime.Now.DayOfYear);

            if (ActionPeriod.Enabled == true && ActivePlans.Count == 0) {
                ActionPeriod.Enabled = false;
                _logger.LogInformation("ActionPeriod was deactivated (again), because no active plans were found !");
            }else if (ActionPeriod.Enabled == false && ActivePlans.Count > 0) {
                ActionPeriod.Enabled = true;
                _logger.LogInformation("ActionPeriod was activated(again), due to a new plan being active !");
            }

            if (SetupRun) { PlanPeriod.Interval = 60000000; SetupRun = false; }
            _logger.LogInformation("PlanWorker Elapsed successfully !");
        }
    }
}
