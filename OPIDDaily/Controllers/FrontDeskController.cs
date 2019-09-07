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
    [Authorize(Roles = "FrontDesk")]
    public class FrontDeskController : SharedController
    {
        public ActionResult Home()
        {
            ServiceTicketBackButtonHelper("Reset", null);
            SpecialReferralBackButtonHelper("Reset", null);
            return View();
        } 

        public ActionResult UpdateClientView()
        {
            string nowServing = SessionHelper.Get("NowServing");

            if (nowServing.Equals("0"))
            {
                ViewBag.Warning = "Please select a client from the Clients Table before selecting Update Client View.";
                return View("Warning");
            }

            return RedirectToAction("UpdateClientView", "Client", new { nowServing = Convert.ToInt32(nowServing) });
        }

        public ActionResult ClearClientView()
        {
            return RedirectToAction("ClearClientView", "Client");
        }
        
        public ActionResult SpecialReferral()
        {
            SpecialReferralViewModel srvm = new SpecialReferralViewModel();
        
            SpecialReferralBackButtonHelper("Get", srvm);

            return View("SpecialReferral", srvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareSpecialReferral(SpecialReferralViewModel srvm)
        {
            if (string.IsNullOrEmpty(srvm.Agency))
            {
                srvm.Agency = "_____________________";
            }
            else
            {
                srvm.NPP = true;
            }

            if (string.IsNullOrEmpty(srvm.AgencyContact))
            {
                srvm.AgencyContact = "_________________________";
            }

            SpecialReferralBackButtonHelper("Set", srvm);

            DateTime today = Extras.DateTimeToday();
            ViewBag.SpecialReferralDate = today.ToString("MMM d, yyyy");
            return View("PrintSpecialReferral", srvm);
        }
    }
}