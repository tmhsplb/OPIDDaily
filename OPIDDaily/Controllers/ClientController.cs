using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult UpdateClientView(int nowServing)
        {
            return View("NowServing");
        }

        public ActionResult ClearClientView()
        {
            return View("NowServing");
        }
    }
}