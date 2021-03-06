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

        /*
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
                    rsvm.SDTDCJ = (SessionHelper.Get("SDTDCJ").Equals("Requested") ? true : false);
                    rsvm.SDVREG = (SessionHelper.Get("SDVREG").Equals("Requested") ? true : false);
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
                    SessionHelper.Set("SDTDCJ", (rsvm.SDTDCJ ? "Requested" : string.Empty));
                    SessionHelper.Set("SDVREG", (rsvm.SDVREG ? "Requested" : string.Empty));
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
                    SessionHelper.Set("SDTDCJ", string.Empty);
                    SessionHelper.Set("SDVREG", string.Empty);
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
           // ServiceTicketBackButtonHelper(mode, rsvm);
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
        */

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