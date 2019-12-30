﻿using OpidDaily.Models;
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

            DailyHub.RefreshClient(Convert.ToInt32(nowServing));

            return View("Clients");
        }

        public ActionResult ClearClientView()
        {
            DailyHub.RefreshClient(0);

            return View("Clients");
        }

        public ActionResult GetClientHistory()
        {
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };

            string nowServing = SessionHelper.Get("NowServing");

            DailyHub.GetClientHistory(Convert.ToInt32(nowServing));

            return View("ExistingClient", rsvm);
        }

        public ActionResult ClearClientHistory()
        {
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };

            DailyHub.GetClientHistory(0);

            return View("ExistingClient", rsvm);
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

        public ActionResult OverflowVoucher()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }

            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };
            Client client = Clients.GetClient(nowServing);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (client.EXP == true)
            {
                return RedirectToAction("ExpressClientOverflowVoucher");
            }

            return RedirectToAction("ExistingClientOverflowVoucher");
        }

        public ActionResult ExpressClientOverflowVoucher()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };
            Client client = Clients.GetClient(nowServing);
             
            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            VoucherBackButtonHelper("Get", rsvm);
            
            return View(rsvm);
        }

        public ActionResult PrepareExpressClientOverflowVoucher(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);
            DateTime today = Extras.DateTimeToday();
            DateTime expiryDate = Clients.CalculateExpiry();

            Clients.UpdateOverflowExpiry(nowServing, expiryDate);

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency = Agencies.GetAgencyName(Convert.ToInt32(rsvm.Agency));  // rsvm.Agency will be the Id of an Agency as a string
            ViewBag.Expiry = Clients.CalculateExpiry().ToString("ddd MMM d, yyyy");
            ViewBag.VoucherDate = today.ToString("MM/dd/yyyy");  // for _OverflowSignatureBlock.cshtml

            VoucherBackButtonHelper("Set", rsvm);
            return View("PrintExpressCLientOverflowVoucher", rsvm);
        }

        public ActionResult ExistingClientOverflowVoucher()
        {
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            VoucherBackButtonHelper("Get", rsvm);

            return View(rsvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExistingClientOVerflowVoucher(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);
            DateTime today = Extras.DateTimeToday();
            DateTime expiryDate = Clients.CalculateExpiry();

            Clients.UpdateOverflowExpiry(nowServing, expiryDate);

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency = Agencies.GetAgencyName(Convert.ToInt32(rsvm.Agency));  // rsvm.Agency will be the Id of an Agency as a string
            ViewBag.Expiry = expiryDate.ToString("ddd MMM d, yyyy");
            ViewBag.VoucherDate = today.ToString("MM/dd/yyyy"); // for _OverflowSignatureBlock.cshtml
            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            rsvm.XBC = client.XBC == true ? "XBC" : string.Empty;
            rsvm.XID = client.XID == true ? "XID" : string.Empty;

            VoucherBackButtonHelper("Set", rsvm);
            var objTuple = new Tuple<List<VisitViewModel>, RequestedServicesViewModel>(visits, rsvm);
            return View("PrintExistingClientOverflowVoucher", objTuple);
        }
    }
}