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
    [Authorize(Roles = "CaseManager")]
    public class CaseManagerController : SharedController
    {
        public ActionResult Home()
        {
            // VoucherBackButtonHelper("Reset", null);
            return View();
        }

        public JsonResult GetMyClients(int page, int? rows = 25)
        {
            int referringAgency = ReferringAgency();
            List<ClientViewModel> clients = Clients.GetMyUnexpiredClients(referringAgency);

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

            // Newly added client becomes the client being served.
            // Entity Framework will set client.Id to the Id of the inserted client.
            // See: https://stackoverflow.com/questions/5212751/how-can-i-get-id-of-inserted-entity-in-entity-framework
            SessionHelper.Set("NowServing", id.ToString());

            DailyHub.Refresh();
            return "Success";
        }
 
        public ActionResult CaseManagerServiceTicket()
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

            if (CheckManager.HasHistory(client.Id))
            {
                client.EXP = false;
                return RedirectToAction("ExistingClientServiceTicket");
            }

            client.EXP = true;
            return RedirectToAction("ExpressClientServiceTicket");
        }

        public ActionResult ExpressClientServiceTicket()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

          //  VoucherBackButtonHelper("Get", rsvm);

            return View("ExpressClientServiceTicket", rsvm);
        }

        public ActionResult StoreContactInfo(ContactInfoViewModel civm)
        {
            int nowServing = NowServing();
            Clients.StoreContactInfo(nowServing, civm);
            return RedirectToAction("ManageClients");
        }

        public ActionResult ExistingClientServiceTicket()
        { 
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            // VoucherBackButtonHelper("Get", rsvm);

            return View("ExistingClientServiceTicket", rsvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareVoucher(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing, null);
            Clients.StoreRequestedServices(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
             
            DateTime today = Extras.DateTimeToday();
            ViewBag.VoucherDate = today.ToString("MM/dd/yyyy");
            ViewBag.Expiry = client.Expiry.ToString("ddd MMM d, yyyy");
                         
            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.BirthName = client.BirthName;
            ViewBag.AKA = client.AKA;

            ViewBag.CurrentAddress = Clients.ClientAddress(client);
            ViewBag.Phone = (!string.IsNullOrEmpty(client.Phone) ? client.Phone : "none given");
            ViewBag.Email = (!string.IsNullOrEmpty(client.Email) ? client.Email : "none available");

            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.BirthPlace = Clients.GetBirthplace(client);
            ViewBag.Age = client.Age;
            ViewBag.Agency = Agencies.GetAgencyName(client.AgencyId);  // rsvm.Agency will be the Id of an Agency as a string

            List<ClientViewModel> dependents = Clients.GetDependents(nowServing);

            var objTuple = new Tuple<List<ClientViewModel>, RequestedServicesViewModel>(dependents, rsvm);

            //  VoucherBackButtonHelper("Set", rsvm);
            return View("PrintVoucher", objTuple);
        }
    }
}