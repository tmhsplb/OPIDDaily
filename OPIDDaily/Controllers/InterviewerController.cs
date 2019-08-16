using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class InterviewerController : SharedController 
    {
        public ActionResult Home()
        {
            return View();
        }
    }
}