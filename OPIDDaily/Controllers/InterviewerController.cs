using OpidDaily.Models;
using OPIDDaily.DAL;
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
            return View("Clients");
        }

        public JsonResult GetClients(int? page = 1, int? rows = 20)
        {
            List<ClientViewModel> clients = Clients.GetClients();

            var jsonData = new
            {
                total = 1,
                page,  
                records = clients.Count,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}