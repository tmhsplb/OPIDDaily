using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class FrontDeskController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ManageClients()
        {
            return View("Clients");
        }

        public JsonResult GetClients(int page, int rows)
        {
            List<ClientViewModel> clients = Clients.GetClients();

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddClient(ClientViewModel cvm)
        {
            Clients.AddClient(cvm);

            CheckinHub.Refresh();
            return "Success";
        }

        public string EditClient(ClientViewModel cvm)
        {
            string status = Clients.EditClient(cvm);

            if (status.Equals("Success"))
            {
                CheckinHub.Refresh();
            }
            return status;
        }

        public void NowServing(int? nowServing = 0)
        {
            SessionHelper.Set("NowServing", nowServing.ToString());    
        }

        public ActionResult History()
        {
            int nowServing = Convert.ToInt32(SessionHelper.Get("NowServing"));

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please select a client from the Clients Table before viewing History.";
                return View("Warning");
            }

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            return View();
        }

        private static int NowServing()
        {
            return Convert.ToInt32(SessionHelper.Get("NowServing"));
        }

        public JsonResult GetHistory(int page, int rows)
        {
            int nowServing = NowServing();

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            List<VisitViewModel> clients = Visits.GetVisits(nowServing);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.AddVisit(nowServing, vvm);

            return "Success";
        }

        public string EditVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.EditVisit(nowServing, vvm);

            return "Success";
        }

        public string DeleteVisit(int id)
        {
            int nowServing = NowServing();
            Visits.DeleteVisit(nowServing, id);

            return "Success";
        }
    }
}