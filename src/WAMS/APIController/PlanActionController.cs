using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WAMS.DataModels;
using WAMS.Services.PlanManagement;

namespace WAMS.APIController
{
    [Route("api/PlanAction")]
    public class PlanActionController : ApiController
    {
        public static List<Tuple<string, DateTime>> Warnings = new List<Tuple<string, DateTime>>();
        protected ILogger _logger { get; }

        public PlanActionController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        // PUT api/AddPlan/PlanTemplate
        [HttpPut]
        public IActionResult AddPlan(PlanTemplate NewPlan)
        {
            bool Result = PlanContainer.AddPlan(new Plan() {
                Name = NewPlan.Name,
                StartCondition = Convert.ToUInt16(NewPlan.StartCondition),
                Duration = new TimeSpan(NewPlan.Duration, 0, 0, 0)
            });
            if (Result) { return Ok(); } else { return BadRequest(); }
        }

        // POST api/TogglePlan/string
        [HttpPost]
        public IActionResult TogglePlan(string Name)
        {
            if (PlanWorker.TogglePlan(Name)) { return Ok(); } else { return BadRequest(); }
        }

        // DELETE api/DeletePlan/string
        [HttpDelete]
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

        // GET api/GetAllPlans
        [HttpGet]
        [Route("api/SystemInformation/GetAllPlans")]
        public string GetAllPlans()
        {
            List<PlanWrapper> Package = new List<PlanWrapper>(PlanContainer.Container.Count);
            foreach(Plan p in PlanContainer.Container) { Package.Add(new PlanWrapper() { Active = p.IsActive(), Content = p }); }
            return JsonConvert.SerializeObject(Package);
        }

        // PUT api/AddAction/ActionTemplate
        [HttpPut]
        public IActionResult AddAction(ActionTemplate NewAction)
        {
            bool Result = PlanContainer.AddAction(new DataModels.Action() {
                 Name = NewAction.Name,
                 PlanName = NewAction.PlanName,
                 Duration = new TimeSpan(0, NewAction.Duration, 0),
                 PrimaryCondition = DateTime.ParseExact(NewAction.PrimaryCondition, "ddd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                 SecondaryCondition = Convert.ToUInt16(NewAction.SecondaryCondition)
            });

            if (Result) { return Ok(); } else { return BadRequest(); }
        }

        // DELETE api/DeleteAction/DeleteActionTemplate
        [HttpDelete]
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

    public class PlanTemplate
    {
        public string Name { get; set; }
        public int StartCondition { get; set; }
        public int Duration { get; set; }
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
