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
    public class InterviewerController : UsersController
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ManageClients()
        {
            DateTime today = DateTime.Today;
            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            return View("Clients");
        }

        public JsonResult GetClients(int page, int rows)
        {
            DateTime today = DateTime.Today;
            ViewBag.ServiceDate = today.ToString("ddd  MMM d");

            List<ClientViewModel> clients = Clients.GetClients(DateTime.Today);

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

        public string EditClient(ClientViewModel cvm)
        {
            string status = Clients.EditClient(cvm);

            if (status.Equals("Success"))
            {
                CheckinHub.Refresh();
            }
            return status;
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