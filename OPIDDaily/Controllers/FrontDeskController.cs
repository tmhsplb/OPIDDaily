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
    }
}