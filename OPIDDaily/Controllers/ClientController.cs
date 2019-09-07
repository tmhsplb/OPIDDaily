using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult Init()
        {
            return View("NowServing");
        }

        public ActionResult ClearClientView()
        {
            return View("NowServing");
        }

       
        public ActionResult GetNowServing(int page, int rows)
        {
            DateTime today = Extras.DateTimeToday();
            List<ClientViewModel> clients = Clients.GetClients(today);
           
            clients = clients.Where(c => c.Id == 0).ToList();

            int pageIndex = page - 1;
            int pageSize = (int)rows;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNowServingSignalR(int nowServing)
        {
            DateTime today = Extras.DateTimeToday();
            List<ClientViewModel> clients = Clients.GetClients(today);

            clients = clients.Where(c => c.Id == nowServing).ToList();

            int pageIndex = 0;
            int pageSize = 1;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = 1,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}