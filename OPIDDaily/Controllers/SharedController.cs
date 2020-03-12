using DataTables.Mvc;
using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class SharedController : Controller
    {
        public static int NowServing()
        {
            return Convert.ToInt32(SessionHelper.Get("NowServing"));
        }

        public void NowServing(int? nowServing = 0)
        {
            SessionHelper.Set("NowServing", nowServing.ToString());
        }

        public ActionResult ManageClients()
        {
            DateTime today = Extras.DateTimeToday();
            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            return View("Clients");
        }

        public JsonResult GetClients(int page, int? rows = 25)
        {
            DateTime today = Extras.DateTimeToday();
            List<ClientViewModel> clients = Clients.GetClients(today);

            ServiceTicketBackButtonHelper("Reset", null);
            SpecialReferralBackButtonHelper("Reset", null);

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

        public string AddClient(ClientViewModel cvm)
        {
            int id = Clients.AddClient(cvm);

            // Newly added client becomes the client being served.
            // Entity Framework will set client.Id to the Id of the inserted client.
            // See: https://stackoverflow.com/questions/5212751/how-can-i-get-id-of-inserted-entity-in-entity-framework
            SessionHelper.Set("NowServing", id.ToString());

            DailyHub.Refresh();
            return "Success";
        }

        public string EditClient(ClientViewModel cvm)
        {
            int id = Clients.EditClient(cvm);

            // Edited client becomes the client being served.
            SessionHelper.Set("NowServing", id.ToString());

            DailyHub.Refresh();      
            return "Success";
        }

        public string DeleteClient(int id)
        {
            Clients.DeleteClient(id);
            DailyHub.Refresh();
            return "Success";
        }

        public JsonResult GetVisitHistory(int page, int rows)
        {
            int nowServing = NowServing();

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = visits.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            visits = visits.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            visits = visits.OrderBy(v => v.Date).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = visits
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.AddVisit(nowServing, vvm);
            DailyHub.Refresh();
            return "Success";
        }

        public string EditVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.EditVisit(nowServing, vvm);

            return "Success";
        }

        public string DeleteVisit(int id)
        {
            int nowServing = NowServing();
            Visits.DeleteVisit(nowServing, id);

            return "Success";
        }
        
        protected static void ServiceTicketBackButtonHelper(string mode, RequestedServicesViewModel rsvm)
        {
            switch (mode)
            {
                case "Get":
                    rsvm.NeedsDocs = SessionHelper.Get("NeedsDocs").Equals("Requested") ? true : false;
                    rsvm.HasDocs = SessionHelper.Get("HasDocs").Equals("Requested") ? true : false;
                    rsvm.Agency = SessionHelper.Get("Agency");
                  //  rsvm.UseBirthName = (SessionHelper.Get("UseBirthName").Equals("Requested") ? true : false);
                    rsvm.BC = (SessionHelper.Get("BC").Equals("Requested") ? true : false);
                    rsvm.PreApprovedBC = (SessionHelper.Get("PreApprovedBC").Equals("Requested") ? true : false);
                    rsvm.HCC = (SessionHelper.Get("HCC").Equals("Requested") ? true : false);
                    rsvm.MBVD = (SessionHelper.Get("MBVD").Equals("Requested") ? true : false);
                    rsvm.PreApprovedMBVD = (SessionHelper.Get("PreApprovedMBVD").Equals("Requested") ? true : false);
                    rsvm.State = (SessionHelper.Get("State").Equals("0") ? string.Empty : SessionHelper.Get("State"));
                    rsvm.NewTID = (SessionHelper.Get("NTID").Equals("Requested") ? true : false);
                    rsvm.PreApprovedNewTID = (SessionHelper.Get("PreApprovedNewTID").Equals("Requested") ? true : false);
                    rsvm.ReplacementTID = (SessionHelper.Get("RTID").Equals("Requested") ? true : false);
                    rsvm.PreApprovedReplacementTID = (SessionHelper.Get("PreApprovedReplacementTID").Equals("Requested") ? true : false);
                    rsvm.NewTDL = (SessionHelper.Get("NTDL").Equals("Requested") ? true : false);
                    rsvm.PreApprovedNewTDL = (SessionHelper.Get("PreApprovedNewTDL").Equals("Requested") ? true : false);
                    rsvm.ReplacementTDL = (SessionHelper.Get("RTDL").Equals("Requested") ? true : false);
                    rsvm.PreApprovedReplacementTDL = (SessionHelper.Get("PreApprovedReplacementTDL").Equals("Requested") ? true : false);
                    rsvm.Numident = (SessionHelper.Get("Numident").Equals("Requested") ? true : false);

                    // Supporting documents
                    rsvm.SDBC = (SessionHelper.Get("SDBC").Equals("Requested") ? true : false);
                    rsvm.SDSSC = (SessionHelper.Get("SDSSC").Equals("Requested") ? true : false);
                    rsvm.SDTID = (SessionHelper.Get("SDTID").Equals("Requested") ? true : false);
                    rsvm.SDTDL = (SessionHelper.Get("SDTDL").Equals("Requested") ? true : false);
                    rsvm.SDOSID = (SessionHelper.Get("SDOSID").Equals("Requested") ? true : false);
                    rsvm.SDOSDL = (SessionHelper.Get("SDOSDL").Equals("Requested") ? true : false);
                    rsvm.SDML = (SessionHelper.Get("SDML").Equals("Requested") ? true : false);
                    rsvm.SDDD = (SessionHelper.Get("SDDD").Equals("Requested") ? true : false);
                    rsvm.SDSL = (SessionHelper.Get("SDSL").Equals("Requested") ? true : false);
                    rsvm.SDDD214 = (SessionHelper.Get("SDDD214").Equals("Requested") ? true : false);
                    rsvm.SDGC = (SessionHelper.Get("SDGC").Equals("Requested") ? true : false);
                    rsvm.SDEBT = (SessionHelper.Get("SDEBT").Equals("Requested") ? true : false);
                    rsvm.SDHOTID = (SessionHelper.Get("SDHOTID").Equals("Requested") ? true : false);
                    rsvm.SDSchoolRecords = (SessionHelper.Get("SDSRECS").Equals("Requested") ? true : false);
                    rsvm.SDPassport = (SessionHelper.Get("SDPassport").Equals("Requested") ? true : false);
                    rsvm.SDJobOffer = (SessionHelper.Get("SDJobOffer").Equals("Requested") ? true : false);
                    rsvm.SDOther = (SessionHelper.Get("SDOTHER").Equals("Requested") ? true : false);
                    rsvm.SDOthersd = (SessionHelper.Get("OTHERSD").Equals("0") ? string.Empty : SessionHelper.Get("OTHERSD"));
                    break;
                    
                case "Set":
                    SessionHelper.Set("NeedsDocs", (rsvm.NeedsDocs ? "Requested" : string.Empty));
                    SessionHelper.Set("HasDocs", (rsvm.HasDocs ? "Requested" : string.Empty));
                    SessionHelper.Set("Agency", rsvm.Agency);
                  //  SessionHelper.Set("UseBirthName", (rsvm.UseBirthName ? "Requested" : string.Empty));
                    SessionHelper.Set("BC", (rsvm.BC ? "Requested" : string.Empty));
                    SessionHelper.Set("PreApprovedBC", (rsvm.PreApprovedBC ? "Requested" : string.Empty));
                    SessionHelper.Set("HCC", (rsvm.HCC ? "Requested" : string.Empty));
                    SessionHelper.Set("MBVD", (rsvm.MBVD ? "Requested" : string.Empty));
                    SessionHelper.Set("PreApprovedMBVD", (rsvm.PreApprovedMBVD ? "Requested" : string.Empty));
                    SessionHelper.Set("State", rsvm.State);
                    SessionHelper.Set("NTID", (rsvm.NewTID ? "Requested" : string.Empty));
                    SessionHelper.Set("PreApprovedNewTID", (rsvm.PreApprovedNewTID ? "Requested" : string.Empty));
                    SessionHelper.Set("RTID", (rsvm.ReplacementTID ? "Requested" : string.Empty));
                    SessionHelper.Set("PreApprovedReplacementTID", (rsvm.PreApprovedReplacementTID ? "Requested" : string.Empty));
                    SessionHelper.Set("NTDL", (rsvm.NewTDL ? "Requested" : string.Empty));
                    SessionHelper.Set("PreApprovedNewTDL", (rsvm.PreApprovedNewTDL ? "Requested" : string.Empty));
                    SessionHelper.Set("RTDL", (rsvm.ReplacementTDL ? "Requested" : string.Empty));
                    SessionHelper.Set("PreApprovedReplacementTDL", (rsvm.PreApprovedReplacementTDL ? "Requested" : string.Empty));
                    SessionHelper.Set("Numident", (rsvm.Numident ? "Requested" : string.Empty));
                 
                    // Supporting documents
                    SessionHelper.Set("SDBC", (rsvm.SDBC ? "Requested" : string.Empty));
                    SessionHelper.Set("SDSSC", (rsvm.SDSSC ? "Requested" : string.Empty));
                    SessionHelper.Set("SDTID", (rsvm.SDTID ? "Requested" : string.Empty));
                    SessionHelper.Set("SDTDL", (rsvm.SDTDL ? "Requested" : string.Empty));
                    SessionHelper.Set("SDOSID", (rsvm.SDOSID ? "Requested" : string.Empty));
                    SessionHelper.Set("SDOSDL", (rsvm.SDOSDL ? "Requested" : string.Empty));
                    SessionHelper.Set("SDML", (rsvm.SDML ? "Requested" : string.Empty));
                    SessionHelper.Set("SDDD", (rsvm.SDDD ? "Requested" : string.Empty));
                    SessionHelper.Set("SDSL", (rsvm.SDSL ? "Requested" : string.Empty));
                    SessionHelper.Set("SDDD214", (rsvm.SDDD214 ? "Requested" : string.Empty));
                    SessionHelper.Set("SDGC", (rsvm.SDGC ? "Requested" : string.Empty));
                    SessionHelper.Set("SDEBT", (rsvm.SDEBT ? "Requested" : string.Empty));
                    SessionHelper.Set("SDHOTID", (rsvm.SDHOTID ? "Requested" : string.Empty));
                    SessionHelper.Set("SDSRECS", (rsvm.SDSchoolRecords ? "Requested" : string.Empty));
                    SessionHelper.Set("SDPassport", (rsvm.SDPassport ? "Requested" : string.Empty));
                    SessionHelper.Set("SDJobOffer", (rsvm.SDJobOffer ? "Requested" : string.Empty));
                    SessionHelper.Set("SDOTHER", (rsvm.SDOther ? "Requested" : string.Empty));
                    SessionHelper.Set("OTHERSD", rsvm.SDOthersd);
                    break;

                case "Reset":
                    SessionHelper.Set("Agency", "0");
                    //   SessionHelper.Set("UseBirthName", string.Empty);
                    SessionHelper.Set("NeedsDocs", string.Empty);
                    SessionHelper.Set("HasDocs", string.Empty);
                    SessionHelper.Set("BC", string.Empty);
                    SessionHelper.Set("PreApprovedBC", string.Empty);
                    SessionHelper.Set("HCC", string.Empty);
                    SessionHelper.Set("MBVD", string.Empty);
                    SessionHelper.Set("PreApprovedMBVD", string.Empty);
                    SessionHelper.Set("State", string.Empty);
                    SessionHelper.Set("NTID", string.Empty);
                    SessionHelper.Set("PreApprovedNewTID", string.Empty);
                    SessionHelper.Set("RTID", string.Empty);
                    SessionHelper.Set("PreApprovedReplacementTID", string.Empty);
                    SessionHelper.Set("NTDL", string.Empty);
                    SessionHelper.Set("PreApprovedNewTDL", string.Empty);
                    SessionHelper.Set("RTDL", string.Empty);
                    SessionHelper.Set("PreApprovedReplacementTDL", string.Empty);
                    SessionHelper.Set("Numident", string.Empty);

                    // Supporting documents
                    SessionHelper.Set("SDBC", string.Empty);
                    SessionHelper.Set("SDSSC", string.Empty);
                    SessionHelper.Set("SDTID", string.Empty);
                    SessionHelper.Set("SDTDL", string.Empty);
                    SessionHelper.Set("SDOSID", string.Empty);
                    SessionHelper.Set("SDOSDL", string.Empty);
                    SessionHelper.Set("SDML", string.Empty);
                    SessionHelper.Set("SDDD", string.Empty);
                    SessionHelper.Set("SDSL", string.Empty);
                    SessionHelper.Set("SDDD214", string.Empty);
                    SessionHelper.Set("SDGC", string.Empty);
                    SessionHelper.Set("SDEBT",string.Empty);
                    SessionHelper.Set("SDHOTID", string.Empty);
                    SessionHelper.Set("SDSRECS", string.Empty);
                    SessionHelper.Set("SDPassport", string.Empty);
                    SessionHelper.Set("SDJobOffer", string.Empty);
                    SessionHelper.Set("SDOTHER", string.Empty);
                    SessionHelper.Set("OTHERSD", string.Empty);
                    break;
            }
        }

        protected void VoucherBackButtonHelper(string mode, RequestedServicesViewModel rsvm)
        {
            ServiceTicketBackButtonHelper(mode, rsvm);
        }

        protected static void SpecialReferralBackButtonHelper(string mode, SpecialReferralViewModel srvm)
        {
            switch (mode)
            {
                case "Get":
                    srvm.FirstName = (SessionHelper.Get("FirstName").Equals("0") ? string.Empty : SessionHelper.Get("FirstName"));
                    srvm.MiddleName = (SessionHelper.Get("MiddleName").Equals("0") ? string.Empty : SessionHelper.Get("MiddleName"));
                    srvm.LastName = (SessionHelper.Get("LastName").Equals("0") ? string.Empty : SessionHelper.Get("LastName"));
                    srvm.Agency = (SessionHelper.Get("Agency").StartsWith("_") ? string.Empty : SessionHelper.Get("Agency"));
                    srvm.AgencyContact = (SessionHelper.Get("AgencyContact").StartsWith("_") ? string.Empty : SessionHelper.Get("AgencyContact"));
                    srvm.Notes = (SessionHelper.Get("Notes").Equals("0") ? string.Empty : SessionHelper.Get("Notes"));
                    break;

                case "Set":
                    SessionHelper.Set("FirstName", srvm.FirstName);
                    SessionHelper.Set("MiddleName", srvm.MiddleName);
                    SessionHelper.Set("LastName", srvm.LastName);
                    SessionHelper.Set("Agency", srvm.Agency);
                    SessionHelper.Set("AgencyContact", srvm.AgencyContact);
                    SessionHelper.Set("Notes", srvm.Notes);
                    break;

                case "Reset":
                    SessionHelper.Set("FirstName", string.Empty);
                    SessionHelper.Set("MiddleName", string.Empty);
                    SessionHelper.Set("LastName", string.Empty);
                    SessionHelper.Set("Agency", string.Empty);
                    SessionHelper.Set("AgencyContact", string.Empty);
                    SessionHelper.Set("Notes", string.Empty);
                    break;
            }
        }

        public ActionResult ExpressClient()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };
            Client client = Clients.GetClient(nowServing);

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            ServiceTicketBackButtonHelper("Get", rsvm);

            return View("ExpressClient", rsvm);
        }

        protected static void PrepareBCNotes(Client client, RequestedServicesViewModel rsvm)
        {
            StringBuilder notes = new StringBuilder();

            if (client.XBC)
            {
                notes.Append(" XBC ");
            }

            if (rsvm.PreApprovedBC)
            {
                notes.Append(" pre-approved ");
            }

            if (rsvm.HCC)
            {
                notes.Append(" (Harris County Clerk) ");
            }
            
            rsvm.BCNotes = notes.ToString();
        }

        protected static void PrepareMBVDNotes(Client client, RequestedServicesViewModel rsvm)
        {
            StringBuilder notes = new StringBuilder();

            if (client.XBC)
            {
                notes.Append(" XBC ");
            }

            notes.Append(string.Format(" {0}", rsvm.State));

            if (rsvm.PreApprovedMBVD)
            {
                notes.Append(", pre-approved ");
            }

            rsvm.MBVDNotes = notes.ToString();
        }

        protected static void PrepareTIDNotes(Client client, RequestedServicesViewModel rsvm)
        {
            StringBuilder notes = new StringBuilder();

            if (client.XID)
            {
                notes.Append(" XID ");
            }

            if (rsvm.PreApprovedNewTID || rsvm.PreApprovedReplacementTID)
            {
                notes.Append(" pre-approved ");
            }

            rsvm.TIDNotes = notes.ToString();
        }
        protected static void PrepareTDLNotes(Client client, RequestedServicesViewModel rsvm)
        {
            StringBuilder notes = new StringBuilder();

            if (client.XID)
            {
                notes.Append(" XID ");
            }

            if (rsvm.PreApprovedNewTDL || rsvm.PreApprovedReplacementTDL)
            {
                notes.Append(" pre-approved ");
            }

            rsvm.TDLNotes = notes.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExpressClient(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);

            PrepareBCNotes(client, rsvm);
            PrepareMBVDNotes(client, rsvm);

            PrepareTIDNotes(client, rsvm);
            PrepareTDLNotes(client, rsvm);

            DateTime today = Extras.DateTimeToday();
            ViewBag.TicketDate = today.ToString("MM/dd/yyyy");

            ViewBag.ServiceTicket = client.ServiceTicket;
            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency =  Agencies.GetAgencyName(Convert.ToInt32(rsvm.Agency));  // rsvm.Agency will be the Id of an Agency as a string

            ServiceTicketBackButtonHelper("Set", rsvm);
            return View("PrintExpressClient", rsvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExistingClient(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);

            PrepareBCNotes(client, rsvm);
            PrepareMBVDNotes(client, rsvm);

            PrepareTIDNotes(client, rsvm);
            PrepareTDLNotes(client, rsvm);
            
            DateTime today = Extras.DateTimeToday();
            ViewBag.TicketDate = today.ToString("MM/dd/yyyy");

            ViewBag.ServiceTicket = client.ServiceTicket;
            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            //   ViewBag.Agency = Agencies.GetAgencyName(Convert.ToInt32(rsvm.Agency));  // rsvm.Agency will be the Id of an Agency as a string
            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            var objTuple = new Tuple<List<VisitViewModel>, RequestedServicesViewModel>(visits, rsvm);
            return View("PrintExistingClient", objTuple);
        }

        public ActionResult History()
        {
            return RedirectToAction("ExistingClient");
        }

        public ActionResult ExistingClient()
        {
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel { Agencies = Agencies.GetAgenciesSelectList() };
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            ServiceTicketBackButtonHelper("Get", rsvm);

            return View(rsvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExistingClientVisits()
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing);

            DateTime today = Extras.DateTimeToday();
            ViewBag.TicketDate = today.ToString("MM/dd/yyyy");

            ViewBag.ServiceTicket = client.ServiceTicket;
            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            List<VisitViewModel> visits = Visits.GetVisits(nowServing);
 
            return View("PrintExistingClientVisits", visits);
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
            
            if (CheckManager.HasHistory(client))
            {
                client.EXP = false;
                return RedirectToAction("ExistingClient");
            }

            client.EXP = true;
            return RedirectToAction("ExpressClient");
        }

        public ActionResult _RequestedServices()
        {
            return PartialView();
        }

        public ActionResult Review()
        {
            DateTime today = Extras.DateTimeToday();
            ViewBag.ServiceDate = today.ToString("ddd  MMM d, yyyy");
            return View("Review");
        }

        public JsonResult GetReviewClients(int page, int? rows = 25)
        {
            DateTime today = Extras.DateTimeToday();
            List<ClientReviewViewModel> clients = Clients.GetReviewClients(today);

            int pageIndex = page - 1;
            int pageSize = (int)rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            clients = clients.OrderBy(c => c.CheckedIn).ToList();

            ServiceTicketBackButtonHelper("Reset", null);
            SpecialReferralBackButtonHelper("Reset", null);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string EditReviewClient(ClientReviewViewModel crvm)
        {
            string status = Clients.EditReviewClient(crvm);

            if (status.Equals("Success"))
            {
                DailyHub.Refresh();
            }
            return status;
        }

        public ActionResult PrepareTable()
        {
            DateTime today = Extras.DateTimeToday();
            ViewBag.ServiceDate = today.ToString("ddd  MMM d, yyyy");

            List<ClientServedViewModel> clientsServed = Clients.ClientsServed(today);

            return View("ClientsServed", clientsServed);
        }

        public ActionResult _ClientsServed()
        {
            return PartialView();
        }
    }
}