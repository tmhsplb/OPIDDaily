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
    public class FrontDeskController : SharedController
    {
        public ActionResult Home()
        {
            BackButtonHelper("Reset", null);
            return View();
        } 
        
        public ActionResult SpecialReferral()
        {
            return View("SpecialReferral");
        }

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

            DateTime today = Extras.DateTimeToday();
            ViewBag.SpecialReferralDate = today.ToString("MMM d, yyyy");
            return View("PrintSpecialReferral", srvm);
        }
    }
}