using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WAMS.Services.GPIOAccess;

namespace WAMS.APIController
{
    [Produces("application/json")]
    [System.Web.Http.Route("api/SystemInformation")]
    public class SystemInformationController : Controller
    {
        public static List<Tuple<string, DateTime>> Warnings = new List<Tuple<string, DateTime>>();
        protected ILogger _logger { get; }

        public SystemInformationController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        // GET api/GetWarnings/
        [HttpGet]
        [ActionName("GetWarnings")]
        public string GetWarnings()
        {
            string json = JsonConvert.SerializeObject(Warnings);
            Warnings.RemoveAll(e => e.Item2.CompareTo(DateTime.Now) == 1);
            return json;
        }

        // GET api/GetValveStatus/
        [HttpGet]
        [ActionName("GetValveStatus")]
        public string GetValveStatus()
        {
            if (Valve.IsOpen) { return "offen"; } else { return "geschlossen"; }
        }

        // GET api/GetSystemStatus/
        [HttpGet]
        [ActionName("GetSystemStatus")]
        public string GetSystemStatus()
        {
            return new SystemStatus(GetValveStatus(), Warnings).ToString();
        }
    }

    public struct SystemStatus
    {
        public string Date { get; private set; }
        public string ValveStatus { get; private set; }
        public List<Tuple<string, DateTime>> Warnings { get; private set; }

        public SystemStatus(string ValveStatus, List<Tuple<string, DateTime>> Warnings)
        {
            this.ValveStatus = ValveStatus;
            this.Warnings = Warnings;
            Date = DateTime.Now.DayOfYear.ToString();
        }

        public override string ToString() { return JsonConvert.SerializeObject(this); }
    }
}
