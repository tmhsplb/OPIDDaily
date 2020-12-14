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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareInterviewerExpressClient(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
         
            // This is the POST method of
            //   ~/Views/FrontDesk/ExpressClient.cshtml
            // If the NowServing client comes from the front desk, then the client will
            // have no supporting documents and the supporting documents section of the service
            // ticket will simply be a worksheet for the interviewer to fill in. If the NowServing
            // client comes from the Dashboard, then the client will have supporting documents.
            // So in either case, passing rsvm instead of null as the second argument of
            // GetClient is correct.
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
    }
}