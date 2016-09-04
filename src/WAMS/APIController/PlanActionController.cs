using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WAMS.DataModels;
using WAMS.Services.PlanManagement;

namespace WAMS.APIController
{
    [Produces("application/json")]
    [System.Web.Http.Route("api/PlanAction")]
    public class PlanActionController : Controller
    {
        public static List<Tuple<string, DateTime>> Warnings = new List<Tuple<string, DateTime>>();
        protected ILogger _logger { get; }

        public PlanActionController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        // PUT api/AddPlan/AddPlanTemplate
        [HttpPut]
        [ActionName("AddPlan")]
        public IActionResult AddPlan(AddPlanTemplate NewPlan)
        {
            bool Result = PlanContainer.AddPlan(new Plan() {
                Name = NewPlan.Name,
                StartCondition = Convert.ToUInt16(NewPlan.StartCondition),
                Duration = new TimeSpan(NewPlan.Duration, 0, 0, 0),
                Elements = new List<DataModels.Action>()
            });
            if (Result) { return Ok(); } else { return BadRequest(); }
        }

        // POST api/PlanAction/TogglePlan/string
        [HttpPost]
        [ActionName("TogglePlan")]
        public IActionResult TogglePlan(string Name)
        {
            if (PlanWorker.TogglePlan(Name)) { return Ok(); } else { return BadRequest(); }
        }

        // POST api/PlanAction/AlterPlan/AlterPlanTemplate
        [HttpPost]
        public IActionResult AlterPlan(AlterPlanTemplate NewPlan)
        {
            if (PlanWorker.AlterPlan(NewPlan.OldPlanName, NewPlan.NewPlan)) { return Ok(); } else { return BadRequest(); }
        }

        // DELETE api/PlanAction/DeletePlan/string
        [HttpDelete]
        [ActionName("DeletePlan")]
        public IActionResult DeletePlan(string Name)
        {
            bool r1 = true;
            bool r2 = true;
            Parallel.Invoke(
                () => r1 = PlanContainer.RemovePlan(Name),
                () => r2 = PlanWorker.RemovePlan(Name)
            );

            if (r1 && r2) { return Ok(); } else { return BadRequest(); }
        }

        // GET api/PlanAction/GetAllPlans
        [HttpGet]
        [ActionName("GetAllPlans")]
        public string GetAllPlans()
        {
            List<PlanWrapper> Package = new List<PlanWrapper>(PlanContainer.Container.Count);
            foreach(Plan p in PlanContainer.Container) { Package.Add(new PlanWrapper() { Active = p.IsActive(), Content = p }); }
            return JsonConvert.SerializeObject(Package);
        }

        // Get api/PlanAction/GetPlanStatuses
        [HttpGet]
        [ActionName("GetPlanStatuses")]
        public string GetPlanStatuses()
        {
            Dictionary<string, bool> Statuses = new Dictionary<string, bool>(PlanContainer.Container.Count);
            foreach(Plan p in PlanContainer.Container) { Statuses.Add(p.Name, p.IsActive()); }
            return JsonConvert.SerializeObject(Statuses);
        }

        // PUT api/PlanAction/AddAction/ActionTemplate
        [HttpPut]
        [ActionName("AddAction")]
        public string AddAction(ActionTemplate NewAction)
        {
            return PlanContainer.AddAction(new DataModels.Action() {
                 Name = NewAction.Name,
                 PlanName = NewAction.PlanName
            });
        }

        // POST api/PlanAction/AlterAction/ActionTemplate
        [HttpPost]
        public IActionResult AlterAction(ActionTemplate EditedAction)
        {
            bool Result = PlanWorker.AlterAction(new DataModels.Action() {
                Name = EditedAction.Name,
                PlanName = EditedAction.PlanName,
                Duration = new TimeSpan(0, EditedAction.Duration, 0),
                PrimaryCondition = DateTime.ParseExact(EditedAction.PrimaryCondition, "ddd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                SecondaryCondition = Convert.ToUInt16(EditedAction.SecondaryCondition)
            });

            if (Result) { return Ok(); } else { return BadRequest(); }
        }

        // DELETE api/PlanAction/DeleteAction/DeleteActionTemplate
        [HttpDelete]
        [ActionName("DeleteAction")]
        public IActionResult DeleteAction(DeleteActionTemplate Action)
        {
            bool r1 = true;
            bool r2 = true;
            Parallel.Invoke(
                () => r1 = PlanContainer.RemoveAction(Action.PlanName, Action.Name),
                () => r2 = PlanWorker.RemoveAction(Action.PlanName, Action.Name)
            );

            if (r1 && r2) { return Ok(); } else { return BadRequest(); }
        }
    }

    public class AddPlanTemplate
    {
        public string Name { get; set; }
        public int StartCondition { get; set; }
        public int Duration { get; set; }
    }

    public class AlterPlanTemplate
    {
        public string OldPlanName { get; set; }
        public Plan NewPlan { get; set; }
    }

    public class ActionTemplate
    {
        public string Name { get; set; }
        public string PlanName { get; set; }
        public int Duration { get; set; }
        public string PrimaryCondition { get; set; }
        public int SecondaryCondition { get; set; }
    }

    public class DeleteActionTemplate
    {
        public string Name { get; set; }
        public string PlanName { get; set; }
    }

    public class PlanWrapper
    {
        public bool Active { get; set; }
        public Plan Content { get; set; }
    }
}
