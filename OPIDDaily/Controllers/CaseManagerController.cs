using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
 

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "CaseManager")]
    public class CaseManagerController : SharedController
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CaseManagerController));

        public ActionResult Home()
        {
            // VoucherBackButtonHelper("Reset", null);
            return View();
        }

        public ActionResult ManageMyClients()
        {
            DateTime today = Extras.DateTimeToday();
            int msgCnt = Convert.ToInt32(SessionHelper.Get("MsgCnt"));

            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            ViewBag.MsgCnt = msgCnt;

            return View("Clients");
        }

        public JsonResult GetMyClients(int page, int? rows = 15)
        {
            int referringAgency = ReferringAgency();
            List<ClientViewModel> clients = Clients.GetMyClients(referringAgency);

           // VoucherBackButtonHelper("Reset", null);

            int pageIndex = page - 1;
            int pageSize = (int)rows;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
 
        public string AddMyClient(ClientViewModel cvm)
        {
            int referringAgency = ReferringAgency();
            int id = Clients.AddMyClient(cvm, referringAgency);

            if (id == -1)
            {
                return "Failure";
            }

            // Newly added client becomes the client being served.
            // Entity Framework will set client.Id to the Id of the inserted client.
            // See: https://stackoverflow.com/questions/5212751/how-can-i-get-id-of-inserted-entity-in-entity-framework
            NowServing(id);

            DailyHub.Refresh();
            return "Success";
        }

        public string EditMyClient(ClientViewModel cvm)
        {
            int id = Clients.EditMyClient(cvm);

            // Edited client becomes the client being served.
            NowServing(id);

            DailyHub.Refresh();

            if (!string.IsNullOrEmpty(cvm.Conversation) && cvm.Conversation.Equals("Y"))
            {
                return "OpenConversation";
            }

            return "Success";
        }

        public string DeleteMyClient(int id)
        {
            string trainingClients = Config.TrainingClients;

            if (!string.IsNullOrEmpty(trainingClients) && trainingClients.Contains(id.ToString()))
            {
                return "Failure";
            }

            Clients.DeleteMyClient(id);
            DailyHub.Refresh();
            return "Success";
        }
 
        public ActionResult ServiceRequest()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }
                      
            Client client = Clients.GetClient(nowServing, null);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (client.LCK)
            {
                ViewBag.Warning = "Operation ID has currently locked Service Requests for this client.";
                return View("Warning");
            }

            if (CheckManager.HasHistory(client.Id))
            {
                //client.EXP = false;
                return RedirectToAction("ExistingClientServiceRequest");
            }

            //client.EXP = true;
            return RedirectToAction("ExpressClientServiceRequest");
        }

        public ActionResult ExpressClientServiceRequest()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);
            rsvm.MBVDS = MBVDS.GetMBVDSelectList();

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            //  VoucherBackButtonHelper("Get", rsvm);
            return View("ExpressClientServiceRequest", rsvm);
        }

        public ActionResult ExistingClientServiceRequest()
        { 
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);
            rsvm.MBVDS = MBVDS.GetMBVDSelectList();

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            // VoucherBackButtonHelper("Get", rsvm);
            return View("ExistingClientServiceRequest", rsvm);
        }

        private bool RequestingBothTIDandTDL(RequestedServicesViewModel rsvm)
        {
            bool tid = rsvm.NewTID || rsvm.ReplacementTID;
            bool tdl = rsvm.NewTDL || rsvm.ReplacementTDL;

            return tid && tdl;
        }

        private bool RequestingBothBCandMBVD(RequestedServicesViewModel rsvm)
        {
            return rsvm.BC & rsvm.MBVD;
        }

        private string ServiceRequestError(RequestedServicesViewModel rsvm)
        {
            string error = string.Empty;

            if (RequestingBothTIDandTDL(rsvm))
            {
                error = "By Texas State Law no resident may possess both an ID and a DL.";
            }
            else if (RequestingBothBCandMBVD(rsvm))
            {
                error = "Cannot request both in-state and out-of-state birth certificates.";
            }
            /*
            else if (rsvm.MBVD)
            {
                error = "Sorry, Operation ID cannot currently process a request for an out-of-state birth certificate.";
            }
            */
            return error;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExpressServiceRequest(RequestedServicesViewModel rsvm)
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
                ModelState.AddModelError("ServiceRequestError",  serviceRequestError);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExpressClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageClients", "CaseManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExistingServiceRequest(RequestedServicesViewModel rsvm)
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
                ModelState.AddModelError("ServiceRequesstError", serviceRequestError);
                return View("ExistingClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageClients", "CaseManager");
        }

        public ActionResult PrepareServiceTicket()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }

            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);

            if (client.HH != 0)
            {
                ViewBag.Warning = "Cannot prepare a Service Ticket for a dependent of another client.";
                return View("Warning");
            }

            PrepareClientNotes(client, rsvm);
            
            DateTime today = Extras.DateTimeToday();
            ViewBag.VoucherDate = today.ToString("MM/dd/yyyy");
            ViewBag.Expiry = client.Expiry.ToString("ddd MMM d, yyyy");
                         
            ViewBag.ClientName = Clients.ClientBeingServed(client, false);
            ViewBag.BirthName = client.BirthName;
            

           // ViewBag.CurrentAddress = Clients.ClientAddress(client);
            ViewBag.Phone = (!string.IsNullOrEmpty(client.Phone) ? client.Phone : "N/A");
            ViewBag.Email = (!string.IsNullOrEmpty(client.Email) ? client.Email : "N/A");

            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
           // ViewBag.BirthPlace = Clients.GetBirthplace(client);
            ViewBag.Age = client.Age;
            ViewBag.Agency = Agencies.GetAgencyName(client.AgencyId);  // rsvm.Agency will be the Id of an Agency as a string

            List<ClientViewModel> dependents = Clients.GetDependents(nowServing);

            var objTuple = new Tuple<List<ClientViewModel>, RequestedServicesViewModel>(dependents, rsvm);

            //  VoucherBackButtonHelper("Set", rsvm);
           // return View("PrintVoucher", objTuple);
            return View("PrintServiceTicket", objTuple);
        }
    }
}