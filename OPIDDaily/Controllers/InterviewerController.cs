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
        // GET: Interviewer
        public ActionResult Home()
        {
            return View();
        }

        public JsonResult GetClients() //(int page, int rows)
        {
            List<ClientViewModel> clients = Clients.GetClients();

            var jsonData = new
            {
                total = 1,
                page = 1,
                records = clients.Count,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}