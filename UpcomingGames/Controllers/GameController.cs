using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using UpcomingGames.Models;
using System.Web.Mvc.Html;
using System.Threading;
using System.IO;
using System.Web.Helpers;
using Microsoft.SqlServer.Server;

namespace UpcomingGames.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> ListGames()
        {
            int pagesize = 5;
            API_Handler getRequest = new API_Handler();
            var rowdata = await getRequest.ThisYearsReleases(1, pagesize);
            return View(rowdata);
        }

        public async Task<ActionResult> ListSortedGames(string date1,string date2)
        {
            API_Handler getRequest = new API_Handler();
            await getRequest.SpecificGetRequest(date1,date2);
            IEnumerable<GameModel> query = getRequest.gamerequest.OrderBy(GameModel => GameModel.released);
            return View(getRequest.gamerequest);
        }

        //public ActionResult ListGamesScroll(int pageindex = 2)
        //{
        //    API_Handler getRequest = new API_Handler();
        //    var syncContext = SynchronizationContext.Current;
        //    SynchronizationContext.SetSynchronizationContext(null);

        //    var model = getRequest.ThisYearsReleases(pageindex).Result;

        //    return PartialView(model);
        //}




        ///



        protected string renderPartialViewToString(string Viewname, object model)
        {
            if (string.IsNullOrEmpty(Viewname))
                Viewname = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewresult = ViewEngines.Engines.FindPartialView(ControllerContext, Viewname);
                ViewContext viewcontext = new ViewContext(ControllerContext, viewresult.View, ViewData, TempData, sw);
                viewresult.View.Render(viewcontext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public class JsonModel
        {
            public string HTMLString { get; set; }
            public bool NoMoreData { get; set; }
        }

        [HttpPost]
        public ActionResult InfiniteScroll(int pageindex)
        {
            System.Threading.Thread.Sleep(200);
            int pagesize = 5;
            API_Handler getRequest = new API_Handler();
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            List<GameModel> tbrow = getRequest.ThisYearsReleases(pageindex,pagesize).Result;
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoreData = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewToString("ListGamesScroll", tbrow);
            return Json(jsonmodel);
        }

        [ChildActionOnly]
        public ActionResult ListGamesScroll(List<GameModel> Model)
        {
            return PartialView(Model);
        }
    }
}