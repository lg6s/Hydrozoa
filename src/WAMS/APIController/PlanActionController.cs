using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WAMS.APIController
{
    [Route("api/[controller]")]
    public class PlanActionController : Controller
    {
        public static List<Tuple<string, DateTime>> Warnings = new List<Tuple<string, DateTime>>();
        protected ILogger _logger { get; }

        public PlanActionController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        // GET api/values/5
        [HttpGet]
        public string GetWarnings()
        {
            string json

            return JsonConvert.SerializeObject(Warnings);
        }
    }
}
