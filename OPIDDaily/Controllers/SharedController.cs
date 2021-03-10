using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using static OPIDDaily.DataContexts.IdentityDB;
using DataTables.Mvc;
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
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SharedController));

        public static int NowServing()
        {
            return Convert.ToInt32(SessionHelper.Get("NowServing"));
        }
 
        public void NowServing(int? nowServing = 0)
        {
            SessionHelper.Set("NowServing", nowServing.ToString());
        }

        public JsonResult NowConversing(SearchParameters sps, int page, int frontdesk = 0, int? nowConversing = 0, int? rows = 15)
        {
          // Log.Debug(string.Format("Enter NowConversing: nowConversing = {0}", nowConversing));

            int agencyId = ReferringAgency();
            int msgCnt = Convert.ToInt32(SessionHelper.Get("MsgCnt"));

            NowServing(nowConversing);
                                  
            List<ClientViewModel> clients;

            if (agencyId == 0)
            {
                if (frontdesk == 1)
                {
                    clients = Clients.GetClients(sps, Extras.DateTimeToday());
                }
                else
                {
                    clients = Clients.GetDashboardClients(sps);
                }
            }
            else
            {
                clients = Clients.GetMyClients(agencyId);
            }

            int pageIndex = page - 1;
            int pageSize = (int)rows;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();
           
            //  Log.Debug(string.Format("NowConversing: page = {0}, rows = {1}, totalPages = {2}", page, rows, totalPages));

            DailyHub.RefreshConversation((int)nowConversing, msgCnt);

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        // See https://stackoverflow.com/questions/18448637/how-to-get-current-user-and-how-to-use-user-class-in-mvc5
        protected int ReferringAgency()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            string userName = User.Identity.GetUserName();
            ApplicationUser user = userManager.FindByNameAsync(userName).Result;

            return user.AgencyId;
        }

        private int AgencyOfUser()
        {
            return ReferringAgency();
        }

        public string GetClientAgencyName(Client client)
        {
            if (!string.IsNullOrEmpty(client.AgencyName))
            {
                return client.AgencyName;
            }
            else
            {
                return Agencies.GetAgencyName(client.AgencyId);
            }
        }

        public ActionResult ManageClients()
        {
            DateTime today = Extras.DateTimeToday();
            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            return View("Clients");
        }

        public JsonResult GetClients(SearchParameters sps, int page, int? rows = 25)
        {
            DateTime today = Extras.DateTimeToday();
            List<ClientViewModel> clients = Clients.GetClients(sps, today);

           // ServiceTicketBackButtonHelper("Reset", null);
           // SpecialReferralBackButtonHelper("Reset", null);

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

        public ActionResult ManageDashboard()
        {
            DateTime today = Extras.DateTimeToday();
            int msgCnt = Convert.ToInt32(SessionHelper.Get("MsgCnt"));

           // Log.Info(string.Format("msgCnt = {0}", msgCnt));

            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            ViewBag.MsgCnt = msgCnt;
            
            return View("Dashboard");
        }

        public JsonResult GetDashboard(SearchParameters sps, int page, int? rows = 15)
        {
            List<ClientViewModel> clients = Clients.GetDashboardClients(sps);
 
            int pageIndex = page - 1;
            int pageSize = (int)rows;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

           // Log.Debug(string.Format("GetDashboard: page = {0}, rows = {1}, totalPages = {2}", page, rows, totalPages));

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDemographicInfo()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }

            DemographicInfoViewModel civm = Clients.GetDemographicInfoViewModel(nowServing);

            return View("DemographicInfo", civm);
        }

        public ActionResult StoreDemographicInfo(DemographicInfoViewModel civm)
        {
            int nowServing = NowServing();
            Clients.StoreDemographicInfo(nowServing, civm);
            return RedirectToAction("ManageClients");
        }

        public string AddClient(ClientViewModel cvm)
        {
            int id = Clients.AddClient(cvm);

            if (id == -1)
            {
                return "Failure";
            }

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
            NowServing(id);

            DailyHub.Refresh(); 
            
            if (!string.IsNullOrEmpty(cvm.Conversation) && cvm.Conversation.Equals("Y"))
            {
                return "OpenConversation";
            }

            return "Success";
        }

        public string DeleteClient(int id)
        {
            Clients.DeleteClient(id);
            DailyHub.Refresh();
            return "Success";
        }

        public JsonResult GetDependents(int id, int page)
        {
            List<ClientViewModel> dependents = Clients.GetDependents(id);

            var jsonData = new
            {
                total = 1,
                page = page,
                records = dependents.Count,
                rows = dependents
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddDependentClient(int household, ClientViewModel cvm)
        {
            int referringAgency = ReferringAgency();
            int id = Clients.AddDependentClient(referringAgency, household, cvm);

            if (id == -1)
            {
                return "Failure";
            }

            DailyHub.Refresh();
            SessionHelper.Set("NowServing", id.ToString());
            return "Success";
        }

        public string EditDependentClient(ClientViewModel cvm)
        {
            Clients.EditDependentClient(cvm);
            return "Success";
        }

        public string DeleteDependentClient(int id)
        {
            Clients.DeleteMyClient(id);
            DailyHub.Refresh();
            return "Success";
        }

        public JsonResult GetVisitHistory(int page, int rows)
        {
            int nowServing = NowServing();
           
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

        public string EditVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.EditVisit(nowServing, vvm);
            DailyHub.Refresh();
            return "Success";
        }

        public JsonResult GetClientPocketChecks(int page, int rows)
        {
            int nowServing = NowServing();

            List<VisitViewModel> visits = Visits.GetPocketChecks(nowServing);
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

        public string AddPocketCheck(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.AddPocketCheck(nowServing, vvm);
            DailyHub.Refresh();
            return "Success";
        }

        public string AddDatedPocketCheck(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            vvm.Date = Extras.DateTimeToday();
            Visits.AddPocketCheck(nowServing, vvm);
            DailyHub.Refresh();
            return "Success";
        }

        public string EditPocketCheck(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.EditVisit(nowServing, vvm);
            DailyHub.Refresh();
            return "Success";
        }

        /// <summary>
        ///  PLB: 6/15/2020 Disallowed this through jqGrid in 
        ///     FrontDeskClientHistory.js
        ///     FrontDeskPocketChecks.js
        ///     BackOfficeClientHistory.js 
        ///     BackOfficePocketChecks.js
        ///     InterviewerClientHistory.js
        ///     InterviewerPocketChecks.js
        ///  May revisit this decision later.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string DeletePocketCheck(int id)
        { 
            int nowServing = NowServing();
            Visits.DeletePocketCheck(nowServing, id);
            DailyHub.Refresh();
            return "Success";
        }

        public JsonResult GetVisitNotes(int id, int page, int rows)
        {
            int nowServing = NowServing();
            List<VisitNoteModel> visitNotes = Visits.GetVisitNotes(nowServing, id);

            var jsonData = new
            {
                total = 1,
                page = page,
                records = visitNotes.Count,
                rows = visitNotes
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);  
        }

        public string AddVisitNote(int vid, string sender, VisitNoteModel vnm)
        {
            int nowServing = NowServing();
                      
          //  Log.Debug(string.Format("NowServing = {0}, vid = {1}, side = {2}", nowServing, vid, side));

            Visits.AddVisitNote(nowServing, vid, sender, vnm);
            DailyHub.Refresh();
            return "Success";
        }

        public string EditVisitNote(VisitNoteModel vnm)
        {
            Visits.EditVisitNote(vnm);
            DailyHub.Refresh();
            return "Success";
        }

        public string DeleteVisitNote(int id)
        {
            Visits.DeleteVisitNote(id);
            DailyHub.Refresh();
            return "Success";
        }

        public ActionResult ExpressClientServiceRequest()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);
            rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
            rsvm.MBVDS = MBVDS.GetMBVDSelectList();

            ViewBag.Agency = GetClientAgencyName(client);

            if (!string.IsNullOrEmpty(client.AgencyName))
            {
                rsvm.OtherAgency = true;
                rsvm.OtherAgencyName = client.AgencyName;
            }

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
            rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
            rsvm.MBVDS = MBVDS.GetMBVDSelectList();

            ViewBag.Agency = GetClientAgencyName(client);

            if (!string.IsNullOrEmpty(client.AgencyName))
            {
                rsvm.OtherAgency = true;
                rsvm.OtherAgencyName = client.AgencyName;
            }

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;

            // VoucherBackButtonHelper("Get", rsvm);
            return View("ExistingClientServiceRequest", rsvm);
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

          //  notes.Append(string.Format(" {0}", rsvm.State));

            notes.Append(string.Format(" {0}", MBVDS.GetMBVDName(Convert.ToInt32(rsvm.State))));

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

        protected void PrepareClientNotes(Client client, RequestedServicesViewModel rsvm)
        {
            if (!rsvm.TrackingOnly)
            {
                PrepareBCNotes(client, rsvm);
                PrepareMBVDNotes(client, rsvm);
                PrepareTIDNotes(client, rsvm);
                PrepareTDLNotes(client, rsvm);
            }
            else
            {
                rsvm.Notes = client.Notes;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExpressClient(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Clients.StoreRequestedServicesAndSupportingDocuments(nowServing, rsvm);

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

            if (rsvm.OtherAgency && !string.IsNullOrEmpty(client.AgencyName))
            {
                ViewBag.Agency = client.AgencyName;
            }
            else
            {
                ViewBag.Agency = Agencies.GetAgencyName(Convert.ToInt32(rsvm.AgencyId));  // rsvm.Agency will be the Id of an Agency as a string 
            }

            // ServiceTicketBackButtonHelper("Set", rsvm);

            // May have added a pocket check. In that case, this express client becomes
            // an existing client.
            if (CheckManager.HasHistory(nowServing))
            {
                List<VisitViewModel> visits = Visits.GetVisits(nowServing);

                var objTuple = new Tuple<List<VisitViewModel>, RequestedServicesViewModel>(visits, rsvm);
                return View("PrintExistingClient", objTuple);
            }

            DailyHub.Refresh();

            return View("PrintExpressClient", rsvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExistingClient(RequestedServicesViewModel rsvm)
        {
            int nowServing = NowServing();
            Clients.StoreRequestedServicesAndSupportingDocuments(nowServing, rsvm);

            // This is the POST method of
            //   ~/Views/FrontDesk/ExistingClient.cshtml
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

            if (rsvm.OtherAgency && !string.IsNullOrEmpty(client.AgencyName))
            {
                ViewBag.Agency = client.AgencyName;
            }
            else
            {
                ViewBag.Agency = Agencies.GetAgencyName(Convert.ToInt32(rsvm.AgencyId));  // rsvm.Agency will be the Id of an Agency as a string
            }

            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            DailyHub.Refresh();

            var objTuple = new Tuple<List<VisitViewModel>, RequestedServicesViewModel>(visits, rsvm);
            return View("PrintExistingClient", objTuple);
        }

        public ActionResult History()
        {
            return RedirectToAction("ExistingClient");
        }

        public ActionResult ExpressClient()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);
            rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
            rsvm.MBVDS = MBVDS.GetMBVDSelectList();

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency = GetClientAgencyName(client);  // needed only for Interviewer role

            if (!string.IsNullOrEmpty(client.AgencyName))
            {
                rsvm.OtherAgency = true;
                rsvm.OtherAgencyName = client.AgencyName;
            }
          
            // ServiceTicketBackButtonHelper("Get", rsvm);

            return View("ExpressClient", rsvm);
        }

        public ActionResult ExistingClient()
        {
            int nowServing = NowServing();
            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);
            rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
            rsvm.MBVDS = MBVDS.GetMBVDSelectList();

            if (!string.IsNullOrEmpty(client.AgencyName))
            {
                rsvm.OtherAgency = true;
                rsvm.OtherAgencyName = client.AgencyName;
            }

            //   rsvm.Agency = Agencies.GetAgencyName(client.AgencyId);  

            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            ViewBag.Agency = GetClientAgencyName(client);  // needed only for Interviewer role

            // ServiceTicketBackButtonHelper("Get", rsvm);

            return View("ExistingClient", rsvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExistingClientVisits()
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing, null);

            DateTime today = Extras.DateTimeToday();
            ViewBag.TicketDate = today.ToString("MM/dd/yyyy");

            ViewBag.ServiceTicket = client.ServiceTicket;
            ViewBag.ClientName = Clients.ClientBeingServed(client);
            ViewBag.BirthName = client.BirthName;
            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            ViewBag.Age = client.Age;
            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            return View("PrintExistingClientVisits", visits);
        }

        public string AddTextMsg(string sender, TextMsgViewModel textMsg)
        {
            int nowServing = NowServing();
            int msgCnt = Convert.ToInt32(SessionHelper.Get("MsgCnt"));

            if (nowServing != 0)
            {
                bool stopConversation = Clients.AddTextMsg(nowServing, sender, textMsg);

                if (stopConversation)
                {
                    msgCnt -= 1;
                }
                else
                {
                    msgCnt += 1;
                }

                SessionHelper.Set("MsgCnt", msgCnt.ToString());

                DailyHub.Refresh();
                DailyHub.RefreshConversation(nowServing, msgCnt);
                return "Success";
            }

            return "Failure";
        }

        public string EditTextMsg(TextMsgViewModel textMsg)
        {
            int nowServing = NowServing();
            int msgCnt = Convert.ToInt32(SessionHelper.Get("MsgCnt"));

            if (nowServing != 0)
            {
                Clients.EditTextMsg(nowServing, textMsg);
                DailyHub.Refresh();
                DailyHub.RefreshConversation(nowServing, msgCnt);
                return "Success";
            }

            return "Failure";
        }

        public ActionResult GetConversation(int page, int? nowConversing = 0, int? rows = 20)
        {
            // Log.Debug(string.Format("Enter GetConversation: nowConversing = {0}", nowConversing));
            int nowServing = NowServing(); ; 
             
            if (nowConversing == 0 || nowServing != nowConversing)
            {
              //  Log.Debug("No conversation!");
                return null;
            }

            int agencyId = AgencyOfUser();

            List <TextMsgViewModel> texts = Clients.GetConversation((int)nowConversing, agencyId);

            int pageIndex = page - 1;
            int pageSize = (int)rows;
            int totalTexts = texts.Count;
            int totalPages = (int)Math.Ceiling((float)totalTexts / (float)rows);

            texts = texts.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            texts = texts.OrderBy(t => t.Date).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = texts.Count,
                rows = texts
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecentChecks()
        {
            ViewBag.RecentYears = Config.RecentYears;
            return View();
        }

        public ActionResult AncientChecks()
        {
            ViewBag.AncientYears = Config.AncientYears;
            return View();
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

        protected string ServiceRequestError(RequestedServicesViewModel rsvm)
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
                return RedirectToAction("ExistingClientServiceRequest");
            }

            return RedirectToAction("ExpressClientServiceRequest");
        }

        public ActionResult ExpressClientServiceTicket()
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

            // ServiceTicketBackButtonHelper("Set", rsvm);
            return View("PrintExpressClient", rsvm);
        }

        public ActionResult ExistingClientServiceTicket()
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

        public JsonResult GetPocketChecks(int page, int rows)
        {
            List<PocketCheckViewModel> pchecks = PocketChecks.GetPocketChecks();
            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = pchecks.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            pchecks = pchecks.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = pchecks
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
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
    
        public ActionResult ServiceTicket()
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
               // client.EXP = false;
                return RedirectToAction("ExistingClient");
            }

           // client.EXP = true;
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

          //  ServiceTicketBackButtonHelper("Reset", null);
          //  SpecialReferralBackButtonHelper("Reset", null);

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