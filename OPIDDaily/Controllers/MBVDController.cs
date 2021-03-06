using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.DAL;
using OPIDDaily.Models;

namespace OPIDDaily.Controllers
{
    public class MBVDController : Controller
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MBVDController));

        public ActionResult ManageMBVDS()
        {
            return View("MBVDS");
        }

        public string AddMBVD(MBVDViewModel mbvdvm)
        {
            MBVDS.AddMBVD(mbvdvm);
            return "Success";
        }

        public string EditMBVD(MBVDViewModel mbvdvm)
        {
            MBVDS.EditMBVD(mbvdvm);
            return "Success";
        }

        public string DeleteMBVD(MBVDViewModel mbvdvm)
        {
            MBVDS.DeleteMBVD(mbvdvm);
            return "Success";
        }
        public JsonResult GetMBVDS(int page, int rows)
        {
            List<MBVDViewModel> mbvds = MBVDS.GetMBVDS();
            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = mbvds.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            mbvds = mbvds.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            mbvds = mbvds.OrderBy(m => m.MBVDId).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = mbvds
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}
