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
                ModelState.AddModelError("ServiceRequestError",  serviceRequestError);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExpressClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageMyClients", "CaseManager");
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
                ModelState.AddModelError("ServiceRequesstError", serviceRequestError);
                return View("ExistingClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageMyClients", "CaseManager");
        }
    }
}