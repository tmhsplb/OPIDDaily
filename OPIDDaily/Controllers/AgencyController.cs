using OPIDDaily.DAL;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class AgencyController : Controller
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AgencyController));

        public ActionResult ManageAgencies()
        {
            return View("Agencies");
        }

        public string AddAgency(AgencyViewModel avm)
        {
            Agencies.AddAgency(avm);
            return "Success";
        }

        public string EditAgency(AgencyViewModel avm)
        {
            Agencies.EditAgency(avm);
            return "Success";
        }

        public string DeleteAgency(AgencyViewModel avm)
        {
            Agencies.DeleteAgency(avm);
            return "Success";
        }
        public JsonResult GetAgencies(int page, int rows)
        {
            List<AgencyViewModel> agencies = Agencies.GetAgencies("AgencyName", "ASC");
            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = agencies.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            agencies = agencies.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            agencies = agencies.OrderBy(a => a.AgencyId).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = agencies
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}