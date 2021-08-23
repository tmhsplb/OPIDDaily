using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Text;

namespace OPIDDaily.DAL
{
    public class Clients
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Clients));

        public static int IdentifyClient(string lastName, string firstName, DateTime dob)
        {
            try
            {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    DateTime today = Extras.DateTimeToday();
                    string lname = Extras.StripSuffix(lastName.ToUpper());
                    Client client = opidcontext.Clients.Where(c => c.LastName.StartsWith(lname) && c.FirstName.StartsWith(firstName) && c.DOB == dob).SingleOrDefault();

                    if (client == null) return 0;

                    return client.Id;
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Could not identify client {0}, {1} DOB {2} Error: {3}", 
                    lastName, firstName, dob.ToString("MM/dd/yyyy"), e.Message));
                return 0;
            }
        }

        public static Client IdentifyClient(Check check)
        {
            DateTime dob = check.DOB;
            string lastName = Extras.StripSuffix(check.Name.ToUpper());

            // Heuristic match: if client has same DOB and last names match, then return client.
            // Else return null;
            try
            {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    Client client = opidcontext.Clients.Where(c => c.DOB == dob && c.LastName.StartsWith(lastName)).SingleOrDefault();
                    return client;
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Could not identify client {0} DOB {1} Error {2}", check.Name, dob.ToString("MM/dd/yyyy"), e.Message));
                return null;
            }
        }

        public static Client GetClient(int nowServing, RequestedServicesViewModel rsvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(nowServing);
               
                if (rsvm != null)
                {
                    rsvm.AgencyId = client.AgencyId.ToString();

                    if (!string.IsNullOrEmpty(client.AgencyName))
                    {
                        rsvm.AgencyName = client.AgencyName;
                    }

                    // Requested Services
                    rsvm.BC = client.BC;
                    rsvm.HCC = client.HCC;
                    rsvm.MBVD = client.MBVD;  // boolean
                    rsvm.MBVDId = client.State;

                    rsvm.State = client.State;
                    rsvm.NewTID = client.NewTID;
                    rsvm.ReplacementTID = client.ReplacementTID;
                    rsvm.NewTDL = client.NewTDL;
                    rsvm.ReplacementTDL = client.ReplacementTDL;
                    rsvm.Numident = client.Numident;
                    rsvm.RequestedDocument = client.RequestedDocument;
                    rsvm.Notes = client.Notes;

                    rsvm.TrackingOnly = NoServicesRequested(rsvm);

                    // Supporting documents
                    rsvm.SDBC = client.SDBC;
                    rsvm.SDSSC = client.SDSSC;
                    rsvm.SDTID = client.SDTID;
                    rsvm.SDTDL = client.SDTDL;
                    rsvm.SDTDCJ = client.SDTDCJ;
                    rsvm.SDVREG = client.SDVREG;
                    rsvm.SDML = client.SDML;
                    rsvm.SDDD = client.SDDD;
                    rsvm.SDSL = client.SDSL;
                    rsvm.SDDD214 = client.SDDD214;
                    rsvm.SDGC = client.SDGC;
                    rsvm.SDEBT = client.SDEBT;
                    rsvm.SDHOTID = client.SDHOTID;
                    rsvm.SDSchoolRecords = client.SDSchoolRecords;
                    rsvm.SDPassport = client.SDPassport;
                    rsvm.SDJobOffer = client.SDJobOffer;
                    rsvm.SDOther = client.SDOther;
                    rsvm.SDOthersd = client.SDOthersd;
                }

                DateTime today = Extras.DateTimeToday();

                if (client.Expiry < today)
                {
                    DateTime now = Extras.DateTimeNow();
                    // Retrieving a client who has rolled off the board.
                    // Make client appear as first in client list
                    client.ServiceDate = now;
                    client.Expiry = Clients.CalculateExpiry(now);
                    opidcontext.SaveChanges();
                }

                return client;
            }
        }

        private static ClientViewModel ClientEntityToClientViewModel(Client client)
        {
            string msg = string.Empty;

            if (!string.IsNullOrEmpty(client.Msgs))
            {
                int separatorIndex = client.Msgs.IndexOf(":");

                if (separatorIndex >= 0)
                {
                    // msg is the first message in the string client.Msgs
                    msg = client.Msgs.Substring(0, separatorIndex);
                }  
            }

            ClientViewModel cvm = new ClientViewModel
            {
                Id = client.Id,
                ServiceDate = client.ServiceDate.AddHours(12),
                Expiry = client.Expiry.AddHours(12),
                ServiceTicket = client.ServiceTicket,
                Stage = client.Stage,
                WaitTime = client.WaitTime,
                Conversation = (client.Conversation ? "Y" : string.Empty),
                HeadOfHousehold = (client.HeadOfHousehold ? "Y" : string.Empty),
                LastName = client.LastName,
                FirstName = client.FirstName,
                MiddleName = client.MiddleName,
                BirthName = client.BirthName,
                DOB = client.DOB.AddHours(12),
                sDOB = client.DOB.AddHours(12).ToString("MM/dd/yyyy"),
                Age = client.Age,
                AgencyName = (!string.IsNullOrEmpty(client.AgencyName) ? client.AgencyName : Agencies.GetAgencyName(client.AgencyId)),

                ACK = (client.ACK == true ? "Y" : string.Empty),
                LCK = (client.LCK == true ? "Y" : string.Empty),
                XID = (client.XID == true ? "Y" : string.Empty),
                XBC = (client.XBC == true ? "Y" : string.Empty),
            
                MSG = msg,
                Msgs = client.Msgs,
                Notes = client.Notes
            };

            return cvm;
        }

        private static void ClientViewModelToClientEntity(ClientViewModel cvm, Client client)
        {
            DateTime dob = (string.IsNullOrEmpty(cvm.sDOB) ? cvm.DOB : DateTime.Parse(cvm.sDOB));

            client.ServiceTicket = cvm.ServiceTicket;
            client.Stage = cvm.Stage;
            client.Conversation = (client.Conversation ? true : (!string.IsNullOrEmpty(cvm.Conversation) && cvm.Conversation.Equals("Y") ? true : false));
            // default(DateTime) : https://stackoverflow.com/questions/221732/datetime-null-value
            client.Expiry = (cvm.Expiry != default(DateTime) ? cvm.Expiry : client.Expiry);
            client.LastName = cvm.LastName;
            client.FirstName = cvm.FirstName;
            client.MiddleName = cvm.MiddleName;
            client.BirthName = cvm.BirthName;
            client.DOB = dob;   
            client.Age = CalculateAge(dob); 

            // client.EXP = (cvm.EXP.Equals("Y") ? true : false);
            client.ACK = (client.ACK ? true : (!string.IsNullOrEmpty(cvm.ACK) && cvm.ACK.Equals("Y")) ? true : false);
            client.LCK = (string.IsNullOrEmpty(cvm.LCK) || cvm.LCK.Equals("''")) ? false : true;
            client.XID = (cvm.XID.Equals("Y") ? true : false);
            client.XBC = (cvm.XBC.Equals("Y") ? true : false);
            client.Msgs = cvm.Msgs;
            client.Notes = cvm.Notes;
        }

        private static int GetUpdatedWaitTime(Client client)
        {
            if (client.Stage.Equals("CheckedIn"))
            {
                double waitTime = (Extras.DateTimeNow()).Subtract(client.CheckedIn).TotalMinutes;
                int waitMinutes = (int)Math.Round(waitTime);
                return waitMinutes;
            }
            else if (client.Stage.Equals("Interviewed") || client.Stage.Equals("BackOffice"))
            {
                double waitTime = (Extras.DateTimeNow()).Subtract(client.Interviewed).TotalMinutes;
                int waitMinutes = (int)Math.Round(waitTime);
                return waitMinutes;
            }

            return 0;
        }

        public static List<ClientViewModel> GetClients(SearchParameters sps, DateTime date, bool? superadmin = false, bool? updateWaittimes = true)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                DateTime nextDate = date.AddHours(24);
                List<Client> clients;
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                bool dobSearch = sps != null && sps._search == true && !string.IsNullOrEmpty(sps.sDOB);

                if (dobSearch)
                {
                    DateTime dob = DateTime.Parse(sps.sDOB);
                    DateTime dob12 = DateTime.Parse(sps.sDOB).AddHours(12);
                    // When searching by DOB, allow a dependent of the Head of Household to be returned, i.e. don't check for HH == 0.
                    // Return both active and inactive clients. Filter below.
                    clients = opiddailycontext.Clients.Where(c => c.DOB == dob || c.DOB == dob12).ToList();
                }
                else
                {
                    // When not searching by DOB, return only top level clients, i.e. clients for which HH == 0.
                    // Return both active and inactive clients. Filter below.
                    clients = opiddailycontext.Clients.Where(c => c.HHId == null && date <= c.ServiceDate && c.ServiceDate <= nextDate).ToList();
                }
               
                clients = clients.OrderByDescending(c => c.ServiceDate).ToList();

                foreach (Client client in clients)
                {
                    if (dobSearch)
                    {
                        // If DOB search returned an inactive client, then set cliet to active
                        client.IsActive = true;
                    }

                    if (!(client.IsActive == false && superadmin == false))
                    {
                        // If superadmin == true, then this call is coming from SuperadminController.
                        // In this case return both active and inactive clients. Otherwise return only active clients.

                        if (updateWaittimes == true)
                        {
                            // Disable WaitTime processing
                            // client.WaitTime = GetUpdatedWaitTime(client);
                        }

                        clientCVMS.Add(ClientEntityToClientViewModel(client));
                    }
                }

                opiddailycontext.SaveChanges();
                return clientCVMS;
            }
        }

        private static List<ClientViewModel> GetFilteredDashboardClients(SearchParameters sps, List<ClientViewModel> cvms)
        {
            List<ClientViewModel> filteredCVMS;

            if (!string.IsNullOrEmpty(sps.sDOB))
            {
                DateTime dob = DateTime.Parse(sps.sDOB);
                DateTime dob12 = DateTime.Parse(sps.sDOB).AddHours(12);
                filteredCVMS = cvms.Where(c => c.DOB == dob || c.DOB == dob12).ToList();
            }

            if (!string.IsNullOrEmpty(sps.AgencyName))
            {
                filteredCVMS = cvms.Where(c => c.AgencyName != null && c.AgencyName.ToUpper().StartsWith(sps.AgencyName.ToUpper())).ToList();
            }
            else if (!string.IsNullOrEmpty(sps.LastName))
            {
                filteredCVMS = cvms.Where(c => c.LastName != null && c.LastName.ToUpper().StartsWith(sps.LastName.ToUpper())).ToList();
            }
            else if (!string.IsNullOrEmpty(sps.FirstName))
            {
                filteredCVMS = cvms.Where(c => c.FirstName != null && c.FirstName.ToUpper().StartsWith(sps.FirstName.ToUpper())).ToList();
            }
            else if (!string.IsNullOrEmpty(sps.MiddleName))
            {
                filteredCVMS = cvms.Where(c => c.MiddleName != null && c.MiddleName.ToUpper().StartsWith(sps.MiddleName.ToUpper())).ToList();
            }
            else if (!string.IsNullOrEmpty(sps.BirthName))
            {
                filteredCVMS = cvms.Where(c => c.BirthName != null && c.BirthName.ToUpper().StartsWith(sps.BirthName.ToUpper())).ToList();
            }
            else
            {
                filteredCVMS = cvms;
            }

            return filteredCVMS;
        }

        public static List<ClientViewModel> GetDashboardClients(SearchParameters sps)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<Client> clients;
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
              
                // A dashboard client will 
                //    come from an agency: c.AgencyId != 0
                //    be a head of household: c.HH == 0
                //    be not a same day client: c.ServiceDate != c.Expiry
                //    be unexpired: today <= c.Expiry
                // List<Client> clients = opiddailycontext.Clients.Where(c => c.AgencyId != 0 && c.HH == 0 && c.ServiceDate != c.Expiry && today <= c.Expiry).ToList();

                if (sps != null && sps._search == true && !string.IsNullOrEmpty(sps.sDOB))
                {
                    // When searching by DOB, allow a dependent of the Head of Household to be returned, i.e. don't check for HH == 0.
                    DateTime dob = DateTime.Parse(sps.sDOB);
                    DateTime dob12 = DateTime.Parse(sps.sDOB).AddHours(12);
                    clients = opiddailycontext.Clients.Where(c => (dob == c.DOB || dob12 == c.DOB) && c.IsActive == true).ToList();
                }
                else
                {
                    // When not searching, return only unexpired top level clients, i.e. clients for which HH == 0.
                    DateTime today = Extras.DateTimeToday();
                    clients = opiddailycontext.Clients.Where(c => c.HHId == null && today <= c.Expiry && c.IsActive == true).ToList();
                }
               
                clients = clients.OrderByDescending(c => c.ServiceDate).ToList();

                foreach (Client client in clients)
                {
                    clientCVMS.Add(ClientEntityToClientViewModel(client));
                }

                if (sps != null && sps._search == false)
                {
                    // if not performing filtered search (example, when refreshing) then
                    // just return the required view models
                    return clientCVMS;
                }

                return GetFilteredDashboardClients(sps, clientCVMS);
            }
        }

        private static TextMsgViewModel TextMsgToTextMsgViewModel(TextMsg textMsg)
        {
            return new TextMsgViewModel
            {
                Id = textMsg.Id,
                Date = textMsg.Date,
                From = textMsg.From,
                To = textMsg.To,
                InHouse = textMsg.InHouse ? "Y" : string.Empty,
                Msg = textMsg.Msg
            };
        }

        private static TextMsg TextMsgViewModelToTextMsg(TextMsgViewModel textMsg)
        {
            return new TextMsg
            {
                Date = Extras.DateTimeToday().AddHours(12),
                From = textMsg.From,
                To = textMsg.To,
                InHouse = (string.IsNullOrEmpty(textMsg.InHouse) || textMsg.InHouse.Equals("''")) ? false : true,
                Msg = textMsg.Msg
            };
        }

        private static bool IsEndMsg(string textMsg)
        {
            string msg = textMsg.ToUpper();
            return (msg.Equals("END") || msg.Equals("DONE") || msg.Equals("OVER"));
        }

        private static void PrependMsg(Client client, string sender, string textMsg, bool inHouse)
        {
            string msg;

            if (IsEndMsg(textMsg))
            {
                msg = string.Format("END:0");
            }
            else
            {
                msg = string.Format("{0}From{1}:0", (inHouse ? "IH" : string.Empty), sender);
            }
            
            // client.Msgs will be a comma separated list of messages
            // Example: client.Msgs = "FromOPID:0,FromFrontDesk:0"
            if (string.IsNullOrEmpty(client.Msgs))
            {
                client.Msgs = msg;
            }
            else
            {
                client.Msgs = string.Format("{0},{1}", msg, client.Msgs);
            }
        }

        public static bool AddTextMsg(int nowServing, string sender, TextMsgViewModel tmvm)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    PrependMsg(client, sender, tmvm.Msg, (!string.IsNullOrEmpty(tmvm.InHouse) && tmvm.InHouse.Equals("Y")));
                    TextMsg textMsg = TextMsgViewModelToTextMsg(tmvm);
                    opiddailycontext.Entry(client).Collection(c => c.TextMsgs).Load();

                    client.TextMsgs.Add(textMsg);
                    opiddailycontext.SaveChanges();

                    // Return true if the message being added is an "end message", 
                    // i.e. one that is meant to stop the conversation.
                    return IsEndMsg(tmvm.Msg);
                }

                return false;
            }
        }

        public static void EditTextMsg(int nowServing, TextMsgViewModel tmvm)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.TextMsgs).Load();
                    TextMsg textMsg = client.TextMsgs.Where(t => t.Id == tmvm.Id).SingleOrDefault();

                    if (textMsg != null)
                    {
                        textMsg.From = tmvm.From;
                        textMsg.To = tmvm.To;
                        textMsg.InHouse = (string.IsNullOrEmpty(tmvm.InHouse) || tmvm.InHouse.Equals("''")) ? false : true;
                        textMsg.Msg = tmvm.Msg;
                        opiddailycontext.SaveChanges();
                    }                  
                }
            }
        }

        public static List<TextMsgViewModel> GetConversation(int nowConversing, int agencyId)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowConversing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.TextMsgs).Load();

                    List<TextMsgViewModel> texts = new List<TextMsgViewModel>();

                    foreach (TextMsg textMsg in client.TextMsgs)
                    {
                        if (textMsg.Vid == 0)
                        {
                            if (agencyId == 0 || (agencyId != 0 && !textMsg.InHouse))
                            {
                                texts.Add(TextMsgToTextMsgViewModel(textMsg));
                            }
                        }
                    }

                    return texts;
                }

                return null;
            }
        }

        public static List<ClientViewModel> GetDemoDashboardClients(SearchParameters sps)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                
                // A demo dashboard client will 
                //    come from an agency: c.AgencyId != 0
                //    be a head of household: c.HH == 0
                //    be not a same day client: c.ServiceDate != c.Expiry.
                // The expiry date of a demo dashboard client does not matter. It may lie in the past
                // or it may lie in the future.
                List<Client> clients = opiddailycontext.Clients.Where(c => c.AgencyId != 0 && c.HHId == null && c.ServiceDate != c.Expiry).ToList();

                foreach (Client client in clients)
                {
                    clientCVMS.Add(ClientEntityToClientViewModel(client));
                }

                if (!sps._search)
                {
                    // if not performing filtered search (example, when refreshing) then
                    // just return the required view models
                    return clientCVMS;
                }

                return GetFilteredDashboardClients(sps, clientCVMS);
            }
        }

        public static List<ClientViewModel> GetMyClients(int referringAgency)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                DateTime today = Extras.DateTimeToday();
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();

                // An unexpired remote client will have today <= c.Expiry
                List<Client> clients = opiddailycontext.Clients.Where(c => c.AgencyId == referringAgency && c.HHId == null && today <= c.Expiry && c.IsActive == true).ToList();
                clients = clients.OrderByDescending(c => c.ServiceDate).ToList();

                foreach (Client client in clients)
                {
                    clientCVMS.Add(ClientEntityToClientViewModel(client));
                }

                return clientCVMS;
            }
        }

        public static int AddClient(ClientViewModel cvm)
        {
            DateTime today = Extras.DateTimeToday();
            DateTime now = Extras.DateTimeNow();
            DateTime dob = DateTime.Parse(cvm.sDOB);
          
            try {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    Client client = new Client
                    {
                        ServiceDate = now,  // was "today", but using "now" allows inserting at top of dashboards
                        ServiceTicket = (string.IsNullOrEmpty(cvm.ServiceTicket) ? "?" : cvm.ServiceTicket),
                        Stage = cvm.Stage,
                        Conversation = (string.IsNullOrEmpty(cvm.Conversation) ? false : true),
                        FirstName = cvm.FirstName,
                        MiddleName = cvm.MiddleName,
                        LastName = cvm.LastName,
                        BirthName = cvm.BirthName,
                        DOB = dob, // cvm.DOB,
                        Age = CalculateAge(dob), // CalculateAge(cvm.DOB),
                        //   EXP = (cvm.EXP.Equals("Y") ? true : false),
                        ACK = (!string.IsNullOrEmpty(cvm.ACK) && cvm.ACK.Equals("Y") ? true : false),
                        XID = (cvm.XID.Equals("Y") ? true : false),
                        XBC = (cvm.XBC.Equals("Y") ? true : false),
                        Notes = cvm.Notes,
                        Screened = now,
                        CheckedIn = now,
                        Interviewing = now,
                        Interviewed = now,
                        BackOffice = now,
                        Done = now,
                        Expiry = CalculateExpiry(today), 
                        IsActive = true
                    };

                    if (String.IsNullOrEmpty(client.LastName))
                    {
                        return -1;
                    }

                    opidcontext.Clients.Add(client);
                    opidcontext.SaveChanges();

                    return client.Id;
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Failed to add new client {0}, {1}. Error: {2}", cvm.LastName, cvm.FirstName, e.Message));
                return -1;
            }
        }


        public static List<ClientRow> GetNewClients(string uploadedFileName)
        {
            List<ClientRow> newClients = MyExcelDataReader.GetClientRows(uploadedFileName);
            return newClients;
        }

        public static void AddNewClients(List<ClientRow> clientRows)
        {
            DateTime today = Extras.DateTimeToday();
            DateTime now = Extras.DateTimeNow();
            List<Client> newClients = new List<Client>();

            try
            {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    // Using context.AddRange as described in: https://entityframework.net/improve-ef-add-performance
                    opidcontext.Configuration.AutoDetectChangesEnabled = false;

                    foreach (ClientRow cr in clientRows)
                    {
                        Client existingClient = opidcontext.Clients.Where(c => c.LastName == cr.LastName && c.DOB == cr.DOB).SingleOrDefault();

                        if (existingClient == null)
                        {
                            Client client = new Client
                            {
                                ServiceDate = today,
                                ServiceTicket = "PB",
                                Stage = "Screened",
                                Conversation = false,
                                FirstName = cr.FirstName,
                                MiddleName = cr.MiddleName,
                                LastName = cr.LastName,
                                BirthName = cr.BirthName,
                                DOB = cr.DOB,
                                Age = CalculateAge(cr.DOB),
                                ACK = false,
                                XID = false,
                                XBC = false,
                                Notes = string.Empty,
                                Screened = now,
                                CheckedIn = now,
                                Interviewing = now,
                                Interviewed = now,
                                BackOffice = now,
                                Done = now,
                                Expiry = CalculateExpiry(today),  // was just today
                                IsActive = true
                            };

                            newClients.Add(client);
                        }
                    }

                    if (newClients.Count > 0)
                    {
                        opidcontext.Clients.AddRange(newClients);
                    }

                    opidcontext.ChangeTracker.DetectChanges();
                    opidcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Error processing client: {0}",  e.Message));
            }

        }

        private static DateTime ExtendedExpiry(DateTime expiry)
        {
            string weekday = expiry.ToString("ddd");
            TimeSpan extension = new TimeSpan(0, 0, 0, 0);

            switch (weekday)
            {
                case "Mon":
                    // extend until Friday
                    extension = new TimeSpan(4, 0, 0, 0);
                    break;
                case "Tue":
                    // extend until Friday
                    extension = new TimeSpan(3, 0, 0, 0);
                    break;
                case "Wed":
                    // extend until Friday
                    extension = new TimeSpan(2, 0, 0, 0);
                    break;
                case "Thu":
                    // extend until Friday
                    extension = new TimeSpan(1, 0, 0, 0);
                    break;
            }

            return expiry.Add(extension);
        }

        public static DateTime CalculateExpiry(DateTime serviceDate)
        {
            int days = Config.CaseManagerExpiryDuration;
            TimeSpan duration = new TimeSpan(days, 0, 0, 0);
            DateTime expiry = serviceDate.Add(duration);

            return ExtendedExpiry(expiry);
        }

        public static int AddMyClient(ClientViewModel cvm, int agencyId)
        {
           // DateTime now = Extras.DateTimeNow();

            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                DateTime now = Extras.DateTimeNow();

                Client client = new Client
                {
                    ServiceDate = now,
                    ServiceTicket = (string.IsNullOrEmpty(cvm.ServiceTicket) ? "?" : cvm.ServiceTicket),
                    Stage = "Screened",
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
                    DOB = cvm.DOB,  
                    Age = CalculateAge(cvm.DOB),
                    Conversation = (string.IsNullOrEmpty(cvm.Conversation) || cvm.Conversation.Equals("''") ? false : true),
                    Notes = cvm.Notes,
                    Screened = now,
                    CheckedIn = now,
                    Interviewing = now,
                    Interviewed = now,
                    BackOffice = now,
                    Done = now,
                    Expiry = CalculateExpiry(now),
                    AgencyId = agencyId,
                    IsActive = true
                };

                opidcontext.Clients.Add(client);
                opidcontext.SaveChanges();

                return client.Id;
            }
        }

        public static List<ClientViewModel> GetDependents(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(id);
                List<ClientViewModel> dependents = new List<ClientViewModel>();

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Dependents).Load();

                    foreach (Client dependent in client.Dependents)
                    {
                        if (dependent.IsActive)
                        {
                            ClientViewModel cvm = ClientEntityToClientViewModel(dependent);
                            dependents.Add(cvm);
                        }
                    }
                }

                return dependents;
            }
        }

        public static int AddDependentClient(int agencyId, int household, ClientViewModel cvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client familyHead = opiddailycontext.Clients.Where(c => c.Id == household).SingleOrDefault();

                if (familyHead != null)
                {
                    DateTime now = Extras.DateTimeNow();
                    // Adding a dependent client makes the family head a head of household
                    familyHead.HeadOfHousehold = true;
                    DateTime today = Extras.DateTimeToday();

                    Client dependent = new Client
                    {
                        ServiceDate = today.AddHours(12),
                        Expiry = CalculateExpiry(today),
                        ServiceTicket = familyHead.ServiceTicket,
                        Stage = "Screened",
                        FirstName = cvm.FirstName,
                        MiddleName = cvm.MiddleName,
                        LastName = cvm.LastName,
                        BirthName = cvm.BirthName,
                        DOB = cvm.DOB,

                        Age = CalculateAge(cvm.DOB),
                        Notes = cvm.Notes,
                        Screened = now,
                        CheckedIn = now,
                        Interviewing = now,
                        Interviewed = now,
                        BackOffice = now,
                        Done = now,
                        AgencyId = agencyId,
                        HeadOfHousehold = false,
                        HHId = household,
                        IsActive = true
                    };

                    if (String.IsNullOrEmpty(dependent.LastName))
                    {
                        return -1;
                    }

                    opiddailycontext.Entry(familyHead).Collection(c => c.Dependents).Load();
                    familyHead.Dependents.Add(dependent);
                    opiddailycontext.SaveChanges();

                    return dependent.Id;
                }

                return 0;
            }
        }

       
        public static void EditDependentClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client dependent = opiddailycontext.Clients.Find(cvm.Id);

                if (dependent != null)
                {
                    // cvm.Stage == null for case manager clients
                    
                    if (string.IsNullOrEmpty(cvm.Stage))
                    {
                        cvm.Stage = "Screened";
                        if (cvm.ACK == null)
                        {
                            cvm.ACK = (dependent.ACK ? "Y" : string.Empty);
                        }
                        if (cvm.XID == null)
                        {
                            cvm.XID = (dependent.XID ? "Y" : string.Empty);
                        }
                        if (cvm.XBC == null)
                        {
                            cvm.XBC = (dependent.XBC ? "Y" : string.Empty);
                        }
                    }
                    
                    Client headofhousehold = opiddailycontext.Clients.Find(dependent.HHId);

                    if (headofhousehold != null)
                    {
                        cvm.ServiceTicket = headofhousehold.ServiceTicket;
                    }

                    ClientViewModelToClientEntity(cvm, dependent);
                    opiddailycontext.SaveChanges();
                }
            }
        }

        
        public static void DeleteDependentClient(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client dependent = opiddailycontext.Clients.Find(id);

                if (dependent != null)
                {
                    // Don't physically delete dependent, just mark it as inactive
                    dependent.IsActive = false;
                    CascadeDelete(opiddailycontext, dependent.Id);
                    HandleLoneDependent(dependent);
                }
            }
        }

        public static void StageTransition(ClientViewModel cvm, Client client)
        {
            if (cvm.Stage.Equals("CheckedIn") && client.Stage.Equals("Screened"))
            {
                client.CheckedIn = Extras.DateTimeNow();
            }

            if (cvm.Stage.Equals("Interviewing") && client.Stage.Equals("CheckedIn"))
            {
                client.Interviewing = Extras.DateTimeNow();
            }

            if (cvm.Stage.Equals("Interviewed") && client.Stage.Equals("Interviewing"))
            {
                client.Interviewed = Extras.DateTimeNow();
            }

            if (cvm.Stage.Equals("BackOffice") && client.Stage.Equals("Interviewed"))
            {
                client.BackOffice = Extras.DateTimeNow();
            }

            if (cvm.Stage.Equals("BackOffice") && (client.Stage.Equals("CheckedIn") || client.Stage.Equals("Interviewing")))
            {
                // Interviewer forgot to make a change in stage
                client.Stage = "Interviewed";
                client.Interviewed = Extras.DateTimeNow().AddMinutes(-5);  // Assume interview completed 5 minutes ago.
            }

            if (cvm.Stage.Equals("Done") && client.Stage.Equals("BackOffice"))
            {
                client.Done = Extras.DateTimeNow();
            }
        }

        private static void PrependStageChangeMsg(ClientViewModel cvm, string clientMsgs)
        {
            string msg = string.Format("StageChange:{0}", cvm.Stage);

            // clientMsgs is a comma separated list of messages
            // Example: clientMsgs = "FromOPID:123588,FromAgency:123587"
            if (string.IsNullOrEmpty(clientMsgs))
            {
                cvm.Msgs = msg;
            }
            else
            {
                cvm.Msgs = string.Format("{0},{1}", msg, clientMsgs);
            }   
        }

        public static int EditClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(cvm.Id);
 
                if (client != null)
                {
                    /*
                    if (client.Stage != cvm.Stage)
                    {
                       // There as been a stage transition
                       PrependStageChangeMsg(cvm, client.Msgs);
                    }
                    */

                    // Disable automatic stage transitioning
                    // StageTransition(cvm, client);
                    ClientViewModelToClientEntity(cvm, client);

                    opidcontext.SaveChanges();
                    return client.Id;
                }

                return 0;
            }
        }

        public static int EditMyClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(cvm.Id);
              
                if (client != null)
                {
                    // Since these fields are not editable by a Case Manager, they will
                    // be set to NULL in cvm. This will cause ClientViewModelToClientEntity
                    // to generate an eror if we do not manually set theme here.
                    cvm.Stage = client.Stage;
                    cvm.ACK = (client.ACK ? "Y" : string.Empty);
                    cvm.LCK = (client.LCK ? "Y" : string.Empty);
                    cvm.XID = (client.XID ? "Y" : string.Empty);
                    cvm.XBC = (client.XBC ? "Y" : string.Empty);
                    
                    ClientViewModelToClientEntity(cvm, client);

                    opidcontext.SaveChanges();
                    return client.Id;
                }

                return 0;
            }
        }

        public static void EditServiceDate(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(cvm.Id);

                if (client != null)
                {
                    if (client.AgencyId != 0)
                    {
                        client.ServiceDate = cvm.ServiceDate;

                        // Update the expiry.
                        client.Expiry = CalculateExpiry(cvm.ServiceDate);

                        opidcontext.SaveChanges();
                    }
                }
            }
        }

        public static void UpdateOverflowExpiry(int nowServing, DateTime expiryDate)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(nowServing);

                if (client != null)
                {
                    client.Expiry = expiryDate;
                    opidcontext.SaveChanges();
                }
            }
        }

        private static void HandleLoneDependent(Client dependent)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<Client> activeCodependents = opiddailycontext.Clients.Where(c => c.HHId == dependent.HHId && c.IsActive == true).ToList();

                if (activeCodependents.Count == 0)
                {
                    // dependent was the lone dependent in this household
                    Client headofhousehold = opiddailycontext.Clients.Find(dependent.HHId);

                    if (headofhousehold != null)
                    {
                        headofhousehold.HeadOfHousehold = false;
                        opiddailycontext.SaveChanges();
                    }
                }
            }
        }

        public static void DeleteClient(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(id);

                if (client != null)
                {
                    client.IsActive = false; // Don't remove client, just mark client inactive
                    CascadeDelete(opiddailycontext, client.Id);
                }
            }
        }
       

        public static void CascadeDelete(OpidDailyDB opiddailycontext, int id)
        {
            Client client = opiddailycontext.Clients.Find(id);

            if (client != null)
            {
                // Physically remove the text msgs of a deactivated cleint
                // opiddailycontext.Entry(client).Collection(c => c.TextMsgs).Load();
                // opiddailycontext.TextMsgs.RemoveRange(client.TextMsgs);
            }

            opiddailycontext.SaveChanges();
        }

       
        // https://stackoverflow.com/questions/9/calculate-age-in-c-sharp
        private static int CalculateAge(DateTime dob)
        {
            DateTime today = Extras.DateTimeToday();
            int age = today.Year - dob.Year;

            if (today.Month < dob.Month || (today.Month == dob.Month && today.Day < dob.Day))
                age--;

            return age;
        }

        public static string ClientBeingServed(Client client, bool includePhone = true)
        {
            if (client != null)
            {
                string clientName;
                string phone = string.IsNullOrEmpty(client.Phone) ? "N/A" : client.Phone;

                if (string.IsNullOrEmpty(client.BirthName))
                {
                    if (includePhone)
                    {
                        clientName = string.Format("{0}, {1} {2} -- Phone: {3}", client.LastName, client.FirstName, client.MiddleName, phone);
                    }
                    else
                    {
                        clientName = string.Format("{0}, {1} {2}", client.LastName, client.FirstName, client.MiddleName);
                    }
                }
                else if (includePhone)
                {
                    clientName = string.Format("{0}, {1} {2} (Birth name: {3}) --  Phone: {4}", client.LastName, client.FirstName, client.MiddleName, client.BirthName, phone);
                }
                else
                {
                    clientName = string.Format("{0}, {1} {2} (Birth name: {3})", client.LastName, client.FirstName, client.MiddleName, client.BirthName);
                }

                return clientName;
            }

            return "Unknown";
        }

        private static ClientReviewViewModel ClientEntityToClientReviewViewModel(Client client)
        {
            return new ClientReviewViewModel
            {
                Id = client.Id,
                ServiceTicket = client.ServiceTicket,
                Stage = client.Stage,
                LastName = client.LastName,
                FirstName = client.FirstName,
                DOB = client.DOB.AddHours(12),
                Age = CalculateAge(client.DOB),
                Active = (client.IsActive ? "Y" : "N"),
                Notes = client.Notes
            };
        }

        public static List<ClientReviewViewModel> GetReviewClients(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientReviewViewModel> clientRVMS = new List<ClientReviewViewModel>();

                // A same-day-service client will have c.ServiceDate == c.Expiry
                List<Client> clients = opiddailycontext.Clients.Where(c => c.HHId == null && c.ServiceDate == date && c.Expiry == date).ToList();
                
                foreach (Client client in clients)
                {
                    clientRVMS.Add(ClientEntityToClientReviewViewModel(client));
                }

                opiddailycontext.SaveChanges();
                return clientRVMS;
            }
        }

        private static void ClientReviewViewModelToClientEntity(ClientReviewViewModel crvm, Client client)
        {
            client.LastName = crvm.LastName;
            client.FirstName = crvm.FirstName;
            client.IsActive = (crvm.Active.Equals("Y") ? true : false);
            client.Notes = crvm.Notes;
        }

        public static string EditReviewClient(ClientReviewViewModel crvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(crvm.Id);

                if (client != null)
                {
                    ClientReviewViewModelToClientEntity(crvm, client);
                    opidcontext.SaveChanges();
                    return "Success";
                }

                return "Failure";
            }
        }

        private static ClientServedViewModel ClientToClientServedViewModel(Client client)
        {
            return new ClientServedViewModel
            {
                ServiceTicket = client.ServiceTicket,
                Expiry = client.Expiry.ToString("MM/dd/yyyy"),
                LastName = client.LastName,
                FirstName = client.FirstName,
                DOB = client.DOB.ToString("MM/dd/yyyy"),
                Notes = client.Notes
            };
        }

        public static List<ClientServedViewModel> ClientsServed(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientServedViewModel> clientsServed = new List<ClientServedViewModel>();
     
                // A same-day-service client will have c.ServiceDate == c.Expiry
                List<Client> clients = opiddailycontext.Clients.Where(c => c.HHId == null && c.ServiceDate == date && c.Expiry == date).ToList();
                foreach (Client client in clients)
                {
                    clientsServed.Add(ClientToClientServedViewModel(client));
                }

                return clientsServed;
            }
        }

        public static void RemoveClients(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                DateTime today = Extras.DateTimeToday();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date).ToList();

                foreach (Client client in clients)
                { 
                    if (client.Expiry <= today)  // if client.Expiry > today then client's voucher is still valid, so don't remove
                    {
                      //  CascadeDelete(client.Id);
                    }
                }
            }
        }

        private static bool NoServicesRequested(RequestedServicesViewModel rsvm)
        {
            return
                (!rsvm.BC && !rsvm.MBVD
                && !rsvm.NewTID && !rsvm.ReplacementTID
                && !rsvm.NewTDL && !rsvm.ReplacementTDL
                && !rsvm.Numident);
        }

        public static void StoreRequestedServicesAndSupportingDocuments(int id, RequestedServicesViewModel rsvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(id);
                                
                if (rsvm.OtherAgency && !string.IsNullOrEmpty(rsvm.OtherAgencyName))
                {
                    client.AgencyName = rsvm.OtherAgencyName;
                    client.AgencyId = 0;
                }
                else
                {
                    client.AgencyName = string.Empty;
                    client.AgencyId = Convert.ToInt32(rsvm.AgencyId);
                }

                // Requested Services
                client.BC = rsvm.BC;
                client.HCC = rsvm.HCC;
                client.MBVD = rsvm.MBVD;
                client.State = rsvm.State;
                client.NewTID = rsvm.NewTID;
                client.ReplacementTID = rsvm.ReplacementTID;
                client.NewTDL = rsvm.NewTDL;
                client.ReplacementTDL = rsvm.ReplacementTDL;
                client.Numident = rsvm.Numident;
                client.RequestedDocument = rsvm.RequestedDocument;

                rsvm.TrackingOnly = NoServicesRequested(rsvm);
                
                // Supporting documents
                client.SDBC = rsvm.SDBC;
                client.SDSSC = rsvm.SDSSC;
                client.SDTID = rsvm.SDTID;
                client.SDTDL = rsvm.SDTDL;
                client.SDTDCJ = rsvm.SDTDCJ;
                client.SDVREG = rsvm.SDVREG;
                client.SDML = rsvm.SDML;
                client.SDDD = rsvm.SDDD;
                client.SDSL = rsvm.SDSL;
                client.SDDD214 = rsvm.SDDD214;
                client.SDGC = rsvm.SDGC;
                client.SDEBT = rsvm.SDEBT;
                client.SDHOTID = rsvm.SDHOTID;
                client.SDSchoolRecords = rsvm.SDSchoolRecords;
                client.SDPassport = rsvm.SDPassport;
                client.SDJobOffer = rsvm.SDJobOffer;
                client.SDOther = rsvm.SDOther;
                client.SDOthersd = rsvm.SDOthersd;

                opiddailycontext.SaveChanges();
            }
        }

        private static DemographicInfoViewModel ClientToClientDemographicInfoViewModel(Client client)
        {
            return new DemographicInfoViewModel
            {
                LastName = client.LastName,
                FirstName = client.FirstName,
                MiddleName = client.MiddleName,
                BirthName = client.BirthName,
                AKA = client.AKA,
                Email = client.Email,
                DOB = client.DOB.ToShortDateString(),
                BirthCity = client.BirthCity,
                BirthState = client.BirthState,
                Phone = client.Phone,
                CurrentAddress = client.CurrentAddress,
                City = client.City,
                State = client.Staat,
                Zip = client.Zip,

                Incarceration = client.Incarceration,
                HousingStatus = client.HousingStatus,
                USCitizen = client.USCitizen,
                Gender = client.Gender,
                Ethnicity = client.Ethnicity,
                Race = client.Race,
                MilitaryVeteran = client.MilitaryVeteran,
                DischargeStatus = client.DischargeStatus,
                Disabled = client.Disabled
            };
        }

        public static DemographicInfoViewModel GetDemographicInfoViewModel(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    return ClientToClientDemographicInfoViewModel(client);
                }
            }

            return null;
        }

        public static void StoreDemographicInfo(int nowServing, DemographicInfoViewModel civm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    client.AKA = civm.AKA;
                    client.Email = civm.Email;
                    client.BirthCity = civm.BirthCity;
                    client.BirthState = civm.BirthState;
                    client.Phone = civm.Phone;
                    client.CurrentAddress = civm.CurrentAddress;
                    client.City = civm.City;
                    client.Staat = civm.State;
                    client.Zip = civm.Zip;

                    client.Incarceration = civm.Incarceration;
                    client.HousingStatus = civm.HousingStatus;
                    client.USCitizen = civm.USCitizen;
                    client.Gender = civm.Gender;
                    client.Ethnicity = civm.Ethnicity;
                    client.Race = civm.Race;
                    client.MilitaryVeteran = civm.MilitaryVeteran;
                    client.DischargeStatus = civm.DischargeStatus;
                    client.Disabled = civm.Disabled;

                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static string ClientAddress(Client client)
        {
            string currrentAddress = string.Empty;

            if (string.IsNullOrEmpty(client.CurrentAddress))
            {
                return "none provided";
            }
            else if (!string.IsNullOrEmpty(client.City))
            {
                if (!string.IsNullOrEmpty(client.Staat))
                {
                    return string.Format("{0}, {1}, {2} {3}", client.CurrentAddress, client.City, client.Staat, client.Zip);
                }
                else
                {
                    return string.Format("{0}, {1} {2}", client.CurrentAddress, client.City, client.Zip);
                }
            }
            else if (!string.IsNullOrEmpty(client.Staat))
            {
                return string.Format("{0}, {1} {2}", client.CurrentAddress, client.Staat, client.Zip);
            }
            else
            {
                return string.Format("{0} {1}", client.CurrentAddress, client.Zip);
            }  
        }

        public static string GetBirthplace(Client client)
        {
            string birthplace = "none provided";

            if (!string.IsNullOrEmpty(client.BirthCity) && !string.IsNullOrEmpty(client.BirthState))
            {
                birthplace = string.Format("{0}, {1}", client.BirthCity, client.BirthState);
            }
            else if (!string.IsNullOrEmpty(client.BirthCity) && string.IsNullOrEmpty(client.BirthState))
            {
                birthplace = client.BirthCity;
            }
            else if (string.IsNullOrEmpty(client.BirthCity) && !string.IsNullOrEmpty(client.BirthState))
            {
                birthplace = client.BirthState;
            }

            return birthplace;
        }
    }
}