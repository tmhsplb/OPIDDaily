using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "Interviewer")]
    public class InterviewerController : SharedController 
    {
        public ActionResult Home()
        {
           // ServiceTicketBackButtonHelper("Reset", null);
            return View();
        }

        /*
        public ActionResult InterviewerServiceTicket()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Dashboard.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (CheckManager.HasHistory(client.Id))
            {
                //  client.EXP = false;
                return RedirectToAction("PrepareInterviewerExistingClient");
            }

            // client.EXP = true;
            return RedirectToAction("PrepareInterviewerExpressClient");
        }
        */

        public ActionResult PrepareInterviewerExpressClient(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
         
            Client client = Clients.GetClient(nowServing, rsvm);
            PrepareClientNotes(client, rsvm);

            DateTime today = Extras.DateTimeToday();
            ViewBag.TicketDate = today.ToString("MM/dd/yyyy");
            ViewBag.ServiceTicket = client.ServiceTicket;
            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency = GetClientAgencyName(client);

            // ServiceTicketBackButtonHelper("Set", rsvm);

            return View("PrintExpressClient", rsvm);
        }

        public ActionResult PrepareInterviewerExistingClient()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);
            PrepareClientNotes(client, rsvm);

            DateTime today = Extras.DateTimeToday();
            ViewBag.TicketDate = today.ToString("MM/dd/yyyy");
            ViewBag.ServiceTicket = client.ServiceTicket;
            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency = GetClientAgencyName(client); 
            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            rsvm.XBC = client.XBC == true ? "XBC" : string.Empty;
            rsvm.XID = client.XID == true ? "XID" : string.Empty;

            // ServiceTicketBackButtonHelper("Set", rsvm);
            var objTuple = new Tuple<List<VisitViewModel>, RequestedServicesViewModel>(visits, rsvm);
            return View("PrintExistingClient", objTuple);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExpressClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when 
            //   ~/Views/CaseManager/ExpressClientServiceRequest.cshtml
            // posts to server. rsvm will contain both requested services
            // and supporting documents.
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing, null);  // pass null so the supporting documents won't be erased
            string serviceRequestError = ServiceRequestError(rsvm);

            if (!string.IsNullOrEmpty(serviceRequestError))
            {
                ViewBag.ClientName = Clients.ClientBeingServed(client);
                ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
                ViewBag.Age = client.Age;
                ModelState.AddModelError("ServiceRequestError", serviceRequestError);
                rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExpressClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageDashboard", "Interviewer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExistingClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when          
            //    ~/Views/CaseManager/ExistingClientServiceRequest.cshtml
            // posts to server. rsvm will contain both requested services
            // and supporting documents.
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing, null);  // pass null so the supporting documents won't be erased
            string serviceRequestError = ServiceRequestError(rsvm);

            if (!string.IsNullOrEmpty(serviceRequestError))
            {
                ViewBag.ClientName = Clients.ClientBeingServed(client);
                ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
                ViewBag.Age = client.Age;
                ModelState.AddModelError("ServiceRequestError", serviceRequestError);
                rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExistingClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageDashboard", "Interviewer");
        }

        public ActionResult InterviewerServiceTicket()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Dashboard.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (CheckManager.HasHistory(client.Id))
            {
                return RedirectToAction("ExistingClientServiceTicket");
            }

            return RedirectToAction("ExpressClientServiceTicket");
        }
    }
}