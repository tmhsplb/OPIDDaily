using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class FrontDeskController : Controller
    {
        // GET: FrontDesk
        public ActionResult Home()
        {
            return View();
        }
    }
}