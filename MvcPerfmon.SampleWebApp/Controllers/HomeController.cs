using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcPerfmon.SampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        [TotalActionCallsPerformanceTracker]
        [TotalExceptionsPerformanceTracker]
        [AverageCallTimePerformanceTracker]
        [TotalActionCallsPerSecondPerformanceTracker]
        public ActionResult Index()
        {
            return View();
        }

    }
}
