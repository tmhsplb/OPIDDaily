using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperadminController : SharedController 
    {
        public ActionResult Home()
        {
            string workingConnectionString = string.Empty;

            ViewBag.Release = Config.GetRelease();  /* was Config.Release */
             
            switch (Config.GetRelease())  /* was Config.Release */
            {
                case "Desktop":
                    workingConnectionString = Config.WorkingDesktopConnectionString;
                    break;
                case "Staging":
                    workingConnectionString = Config.WorkingStagingConnectionString;
                    break;
                case "Production":
                    workingConnectionString = Config.WorkingProductionConnectionString;
                    break;
            }

            ViewBag.ConnectionString = Config.ConnectionString;
            ViewBag.ChangedConnectionString = (Config.ConnectionString.Equals(workingConnectionString) ? "False" : "True");

            // Log.Info("Goto Superadmin home page");
            return View();
        }

        public ActionResult ManageServiceDateClients()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            if (serviceDate.Equals("0"))
            {
                DateTime today = Extras.DateTimeToday();
                serviceDate = today.ToString("ddd MMM d");
            }
            ViewBag.ServiceDate = DateTime.Parse(serviceDate).ToString("ddd MMM d");
            SessionHelper.Set("ServiceDate", serviceDate);
            return View("Clients");
        }

        public JsonResult GetServiceDateClients(int page, int rows)
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            DateTime selectedDate = DateTime.Parse(serviceDate);
            List<ClientViewModel> clients = Clients.GetClients(selectedDate, false);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

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
         
        public ActionResult ServiceDateReview()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            if (serviceDate.Equals("0"))
            {
                DateTime today = Extras.DateTimeToday();
                serviceDate = today.ToString("ddd MMM d");
            }

            ViewBag.ServiceDate = DateTime.Parse(serviceDate).ToString("ddd MMM d");
            SessionHelper.Set("ServiceDate", serviceDate);
            return View("Review");
        }

        public JsonResult GetServiceDateReviewClients(int page, int rows)
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            DateTime selectedDate = DateTime.Parse(serviceDate);
            
            List<ClientReviewViewModel> clients = Clients.GetReviewClients(selectedDate);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            clients = clients.OrderBy(c => c.CheckedIn).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeServiceDate(DatePickerViewModel dpvm)
        {
            SessionHelper.Set("ServiceDate", dpvm.datepicker);
            return RedirectToAction("ManageServiceDateClients");
        }

        public ActionResult PrepareServiceDateTable()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            ViewBag.ServiceDate = DateTime.Parse(serviceDate).ToString("ddd MMM d, yyyy");

            DateTime specifiedDate = DateTime.Parse(serviceDate);

            List<ClientServedViewModel> clientsServed = Clients.ClientsServed(specifiedDate);
            return View("ClientsServed", clientsServed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveServiceDateClients()
        {
            string serviceDate = SessionHelper.Get("ServiceDate");
            DateTime date = DateTime.Parse(serviceDate);
            Clients.RemoveClients(date);
            return RedirectToAction("ManageServiceDateClients");
        }

        public ActionResult Version()
        {
            return View("Version");
        }
    }
}