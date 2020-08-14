using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UpcomingGames.Controllers
{
    public class HealthCheckController : Controller
    {
        // GET: HealthCheck
        public ActionResult HealthCheck()
        {
            return View();
        }
    }
}