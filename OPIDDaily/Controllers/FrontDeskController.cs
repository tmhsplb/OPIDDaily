using OpidDaily.Models;
using OPIDDaily.DAL;
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

            var jsonData = new
            {
                total = 1,
                page,
                records = clients.Count,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddClient(ClientViewModel cvm)
        {
            Clients.AddClient(cvm);
            return "Success";
        }

        public string EditClient(ClientViewModel cvm)
        {
            string status = Clients.EditClient(cvm);
            return status;
        }
    }
}