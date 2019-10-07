using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "Interviewer")]
    public class InterviewerController : SharedController 
    {
        public ActionResult Home()
        {
            ServiceTicketBackButtonHelper("Reset", null);
            return View();
        }

        public ActionResult ServiceTicket()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (client.EXP == true)
            {
                return RedirectToAction("ExpressClient");
            }

            return RedirectToAction("ExistingClient");
        }
    }
}