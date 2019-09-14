using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        public ActionResult InitNowServing()
        {
            return View("NowServing");
        }

        public ActionResult InitNowServingHistory()
        {
            return View("History");
        }

        public ActionResult ClearClientView()
        {
            return View("NowServing");
        }
               
        public JsonResult GetEmptyGrid(int page, int rows)
        {
            List<ClientViewModel> clients = new List<ClientViewModel>();

            var jsonData = new
            {
                total = 1,  
                page = 1,  
                records = 0,  
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNowServing(int nowServing)
        {
            DateTime today = Extras.DateTimeToday();
            List<ClientViewModel> clients = Clients.GetClients(today);

            clients = clients.Where(c => c.Id == nowServing).ToList();
 
            var jsonData = new
            {
                total = 1,  
                page = 1,
                records = 1,  
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
 
        public JsonResult GetClientHistory(int nowServing)
        {
            if (nowServing == 0)
            {
                return GetEmptyGrid(1, 20);
            }

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            int pageIndex = 0;
            int pageSize = 20;
            int totalRecords = visits.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)20);

            visits = visits.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            visits = visits.OrderBy(v => v.Date).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = 1,
                records = totalRecords,
                rows = visits
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}