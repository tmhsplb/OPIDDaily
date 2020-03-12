using OpidDaily.Models;
using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OPIDDaily.DAL
{
    public class Clients
    { 
        public static Client GetClient(int nowServing)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(nowServing);
                return client;
            }
        }

        private static ClientViewModel ClientEntityToClientViewModel(Client client)
        {
            return new ClientViewModel
            {
                Id = client.Id,
                ServiceDate = client.ServiceDate.ToString("MM/dd/yyyy"),
                Expiry = client.Expiry.ToString("MM/dd/yyyy"),
                ServiceTicket = client.ServiceTicket,
                Stage = client.Stage,
                WaitTime = client.WaitTime,
                LastName = client.LastName,
                FirstName = client.FirstName,
                MiddleName = client.MiddleName,
                BirthName = client.BirthName,
                DOB = client.DOB.ToString("MM/dd/yyyy"),
                Age = client.Age,
                EXP = (client.EXP == true ? "Y" : string.Empty),
                PND = (client.PND == true ? "Y" : string.Empty),
                XID = (client.XID == true ? "Y" : string.Empty),
                XBC = (client.XBC == true ? "Y" : string.Empty),
             //   History = (hasHistory ? "Y" : string.Empty),
                Notes = client.Notes
            };
        }

        private static void ClientViewModelToClientEntity(ClientViewModel cvm, Client client)
        {
            client.ServiceTicket = cvm.ServiceTicket;
            client.Stage = cvm.Stage;
            client.LastName = cvm.LastName;
            client.FirstName = cvm.FirstName;
            client.MiddleName = cvm.MiddleName;
            client.BirthName = cvm.BirthName;
            client.DOB = DateTime.Parse(cvm.DOB);
            client.Age = CalculateAge(DateTime.Parse(cvm.DOB));
           // client.EXP = (cvm.EXP.Equals("Y") ? true : false);
            client.PND = (cvm.PND.Equals("Y") ? true : false);
            client.XID = (cvm.XID.Equals("Y") ? true : false);
            client.XBC = (cvm.XBC.Equals("Y") ? true : false);
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

        public static List<ClientViewModel> GetClients(DateTime date, bool? superadmin = false, bool? updateWaittimes = true)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                DateTime today = Extras.DateTimeToday();
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date || c.Expiry >= today).ToList();

                foreach (Client client in clients)
                {
                    if (client.Active == false && superadmin == false)
                    {
                        // If superadmin == true, then this call is coming from SuperadminController.
                        // In this case return both active and inactive clients. Otherwise return only active clients.
                    }
                    else
                    {
                        //  bool hasHistory = CheckManager.HasHistory(client);

                        if (updateWaittimes == true)
                        {
                            client.WaitTime = GetUpdatedWaitTime(client);
                        }

                        clientCVMS.Add(ClientEntityToClientViewModel(client));
                    }
                }

                opiddailycontext.SaveChanges();
                return clientCVMS;
            }
        }

        public static List<ClientViewModel> GetMyUnexpiredClients(int referringAgency)
        {
            DateTime today = Extras.DateTimeToday();

            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.AgencyId == referringAgency && c.Expiry >= today && c.Active == true).ToList();

                foreach (Client client in clients)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    client.EXP = client.Visits.Count == 0;
                    
                    clientCVMS.Add(ClientEntityToClientViewModel(client));
                }

                return clientCVMS;
            }
        }

        public static int AddClient(ClientViewModel cvm)
        {
            DateTime today = Extras.DateTimeToday();
            DateTime now = Extras.DateTimeNow();

            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = new Client
                {
                    ServiceDate = today,
                    ServiceTicket = (string.IsNullOrEmpty(cvm.ServiceTicket) ? "?" : cvm.ServiceTicket),
                    Stage = cvm.Stage,
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
                    DOB = (string.IsNullOrEmpty(cvm.DOB) ? Extras.DateTimeToday() : DateTime.Parse(cvm.DOB)),
                    Age = (string.IsNullOrEmpty(cvm.DOB) ? 0 : CalculateAge(DateTime.Parse(cvm.DOB))),
                 //   EXP = (cvm.EXP.Equals("Y") ? true : false),
                    PND = (cvm.PND.Equals("Y") ? true : false),
                    XID = (cvm.XID.Equals("Y") ? true : false),
                    XBC = (cvm.XBC.Equals("Y") ? true : false),
                    Notes = cvm.Notes,
                    Screened = now,
                    CheckedIn = now,
                    Interviewing = now,
                    Interviewed = now,
                    BackOffice = now,
                    Done = now,
                    Expiry = today,
                    Active = true
                };

                opidcontext.Clients.Add(client);
                opidcontext.SaveChanges();

                return client.Id;
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

        public static DateTime CalculateExpiry()
        {
            DateTime today = Extras.DateTimeToday();
            int days = Config.CaseManagerVoucherDuration;
            TimeSpan duration = new TimeSpan(days, 0, 0, 0);
            DateTime expiry = today.Add(duration);

            return ExtendedExpiry(expiry);
        }

        public static int AddMyClient(ClientViewModel cvm, int agencyId)
        {
            DateTime now = Extras.DateTimeNow();

            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = new Client
                {
                    ServiceDate = Extras.DateTimeToday(),
                    ServiceTicket = (string.IsNullOrEmpty(cvm.ServiceTicket) ? "?" : cvm.ServiceTicket),
                    Stage = "Screened",
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
                    DOB = (string.IsNullOrEmpty(cvm.DOB) ? Extras.DateTimeToday() : DateTime.Parse(cvm.DOB)),
                    Age = (string.IsNullOrEmpty(cvm.DOB) ? 0 : CalculateAge(DateTime.Parse(cvm.DOB))),
                  //  EXP = (cvm.EXP.Equals("Y") ? true : false),
                  //  PND = (cvm.PND.Equals("Y") ? true : false),
                  //  XID = (cvm.XID.Equals("Y") ? true : false),
                  //  XBC = (cvm.XBC.Equals("Y") ? true : false),
                    Notes = cvm.Notes,
                    Screened = now,
                    CheckedIn = now,
                    Interviewing = now,
                    Interviewed = now,
                    BackOffice = now,
                    Done = now,
                    Expiry = CalculateExpiry(),
                    AgencyId = agencyId,
                    Active = true
                };

                opidcontext.Clients.Add(client);
                opidcontext.SaveChanges();

                return client.Id;
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

        public static int EditClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(cvm.Id);

                // needed for case manager clients
                if (string.IsNullOrEmpty(cvm.Stage))
                {
                    cvm.Stage = "Screened"; 
                    cvm.PND = (client.PND ? "Y" : string.Empty);
                    cvm.XID = (client.XID ? "Y" : string.Empty);
                    cvm.XBC = (client.XBC ? "Y" : string.Empty);
                }

                if (client != null)
                {
                    StageTransition(cvm, client);

                    ClientViewModelToClientEntity(cvm, client);
                    opidcontext.SaveChanges();
                    return client.Id;
                }

                return 0;
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

            public static void DeleteClient(int id)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(id);
                if (client != null)
                {
                    client.Active = false; // Don't remove client, just mark client inactive
                    opidcontext.SaveChanges();
                }
            }
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

        public static string ClientBeingServed(int nowServing)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(nowServing);

                if (client != null)
                {
                    string clientName;

                    if (string.IsNullOrEmpty(client.BirthName))
                    {
                        clientName = string.Format("{0}, {1} {2}", client.LastName, client.FirstName, client.MiddleName);
                    }
                    else
                    {
                       clientName = string.Format("{0}, {1} {2} (Birth name: {3})", client.LastName, client.FirstName, client.MiddleName, client.BirthName);
                    }
                   
                    return clientName;
                }

                return "Unknown";
            }
        }

        private static ClientReviewViewModel ClientEntityToClientReviewViewModel(Client client)
        {
            return new ClientReviewViewModel
            {
                Id = client.Id,
                ServiceTicket = client.ServiceTicket,
                Expiry = client.Expiry.ToString("MM/dd/yyyy"),
                Stage = client.Stage,
                LastName = client.LastName,
                FirstName = client.FirstName,
                DOB = client.DOB.ToString("MM/dd/yyyy"),
                Active = (client.Active ? "Y" : "N"),
                Notes = client.Notes
            };
        }

        public static List<ClientReviewViewModel> GetReviewClients(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientReviewViewModel> clientRVMS = new List<ClientReviewViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date || c.Expiry >= date).ToList();

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
            client.Active = (crvm.Active.Equals("Y") ? true : false);
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
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date || c.Expiry >= date).ToList();

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
                        // Manually perform cascade delete on the related (by a foreign key) Visits table.
                        opiddailycontext.Entry(client).Collection(c => c.Visits).Load();
                        opiddailycontext.Visits.RemoveRange(client.Visits);

                        // Physically remove client from table
                        opiddailycontext.Clients.Remove(client);
                    }
                }

                opiddailycontext.SaveChanges();
            }
        }

        public static void StoreRequestedServices(int nowServing, RequestedServicesViewModel rsvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                //    client.Agency = rsvm.Agency;
                }

                opiddailycontext.SaveChanges();   
            }
        }
    }
}