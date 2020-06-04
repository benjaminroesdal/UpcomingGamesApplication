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

        [OutputCache(Duration = 86400)]
        public ActionResult ListGames()
        {
            GameCollection gameCollection = new GameCollection();
            //API_Handler getRequest = new API_Handler();
            //var rowdata = await getRequest.GetThisMonthGames();
            System.Threading.Thread.Sleep(200);;
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            var rowdata = gameCollection.GetGameList().Result;
            return View(rowdata);
        }


        [OutputCache(Duration = 86400)]
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

        [OutputCache(Duration = 86400)]
        public ActionResult InfiniteScroll(int pageoffset)
        {
            GameCollection gameCollection = new GameCollection();
            System.Threading.Thread.Sleep(200);
            int pagesize = 5;
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);
            var data = gameCollection.GameList().Result;
            List<GameModel> tbrow = gameCollection.InfiniteScrollList(pageoffset);
            JsonModel jsonmodel = new JsonModel();
            jsonmodel.NoMoreData = tbrow.Count < pagesize;
            jsonmodel.HTMLString = renderPartialViewToString("ListGamesScroll", tbrow);
            return Json(jsonmodel);
        }


        [OutputCache(Duration = 86400)]
        public ActionResult ListGamesScroll(List<GameModel> Model)
        {
            return PartialView(Model);
        }
    }
}