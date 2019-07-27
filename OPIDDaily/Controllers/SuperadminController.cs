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
    public class SuperadminController : UsersController
    {
        public ActionResult Home()
        {
            string workingConnectionString = string.Empty;

            ViewBag.Release = Config.Release;

            switch (Config.Release)
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

        public ActionResult ManageRoles()
        {
            return RedirectToAction("Index", "Role");
        }

        
        public ActionResult ManageUsers()
        {
            return View("Users");
        }

        public string ExtendInvitation(InvitationViewModel invite)
        {
            if (InUse(invite.UserName))
            {
                string status = string.Format("The user name {0} is already in use. Please use a different user name.", invite.UserName);
                return status;
            }

            Identity.ExtendInvitation(invite);

            return "Success";
        }

        public string EditUser(InvitationViewModel invite)
        {
            string status = Users.EditUser(invite);
            return status;
        }
        
        public JsonResult GetUsers(int page, int rows)
        {
            List<InvitationViewModel> invitations = Identity.GetUsers();

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = invitations.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            invitations = invitations.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = invitations
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageClients()
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

        public JsonResult GetClients(int page, int rows)
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
 
        private static int NowServing()
        {
            return Convert.ToInt32(SessionHelper.Get("NowServing"));
        }

        public void NowServing(int? nowServing = 0)
        {
            SessionHelper.Set("NowServing", nowServing.ToString());
        }

        public ActionResult History()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please select a client from the Clients Table before viewing History.";
                return View("Warning");
            }

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            return View();
        }

        public JsonResult GetHistory(int page, int rows)
        {
            int nowServing = NowServing();

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = visits.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            visits = visits.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            visits = visits.OrderBy(v => v.Date).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = visits
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Review()
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

        public JsonResult GetReviewClients(int page, int rows)
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
        public ActionResult ChangeServiceDate(DatePickerViewModel dpvm)
        {
            SessionHelper.Set("ServiceDate", dpvm.datepicker);
            return RedirectToAction("ManageClients");
        }
    }
}