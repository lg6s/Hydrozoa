using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WAMS.Controllers
{
    public class PrimaryController : Controller
    {
        protected ILogger _logger { get; }

        public PrimaryController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        public IActionResult ClientInterface()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
