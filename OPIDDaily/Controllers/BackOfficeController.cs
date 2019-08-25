using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.Utils;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "BackOffice")]
    public class BackOfficeController : SharedController
    {
        public ActionResult Home()
        {
            return View();
        }
    }
}