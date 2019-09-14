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

        private static ClientViewModel ClientEntityToClientViewModel(Client client, bool hasHistory)
        {
            return new ClientViewModel
            {
                Id = client.Id,
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
                History = (hasHistory ? "Y" : string.Empty),
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
            client.EXP = (cvm.EXP.Equals("Y") ? true : false);
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

        public static List<ClientViewModel> GetClients(DateTime date, bool? updateWaittimes = true)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date && c.Active == true).ToList();

                foreach (Client client in clients)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    bool hasHistory = client.Visits.Count > 0;

                    if (updateWaittimes == true)
                    {
                        client.WaitTime = GetUpdatedWaitTime(client);
                    }
                    clientCVMS.Add(ClientEntityToClientViewModel(client, hasHistory));
                }

                opiddailycontext.SaveChanges();
                return clientCVMS;
            }
        }

        public static int AddClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = new Client
                {
                    ServiceDate = Extras.DateTimeToday(),
                    ServiceTicket = (string.IsNullOrEmpty(cvm.ServiceTicket) ? "?" : cvm.ServiceTicket),
                    Stage = cvm.Stage,
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
                    DOB = (string.IsNullOrEmpty(cvm.DOB) ? Extras.DateTimeToday() : DateTime.Parse(cvm.DOB)),
                    Age = (string.IsNullOrEmpty(cvm.DOB) ? 0 : CalculateAge(DateTime.Parse(cvm.DOB))),
                    EXP = (cvm.EXP.Equals("Y") ? true : false),
                    PND = (cvm.PND.Equals("Y") ? true : false),
                    XID = (cvm.XID.Equals("Y") ? true : false),
                    XBC = (cvm.XBC.Equals("Y") ? true : false),
                    Notes = cvm.Notes,
                    Screened = Extras.DateTimeNow(),
                    CheckedIn = Extras.DateTimeNow(),
                    Interviewing = Extras.DateTimeNow(),
                    Interviewed = Extras.DateTimeNow(),
                    BackOffice = Extras.DateTimeNow(),
                    Done = Extras.DateTimeNow(),
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
                    string clientName = string.Format("{0}, {1} {2} (Birth name: {3})", client.LastName, client.FirstName, client.MiddleName, client.BirthName);
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
                Stage = client.Stage,
                LastName = client.LastName,
                FirstName = client.FirstName,
                Active = (client.Active ? "Y" : string.Empty),
                CheckedIn = client.CheckedIn,
                Notes = client.Notes
            };
        }

        public static List<ClientReviewViewModel> GetReviewClients(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientReviewViewModel> clientRVMS = new List<ClientReviewViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date).ToList();

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
                LastName = client.LastName,
                FirstName = client.FirstName,
                MiddleName = client.MiddleName,
                Notes = client.Notes
            };
        }

        public static List<ClientServedViewModel> ClientsServed(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientServedViewModel> clientsServed = new List<ClientServedViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date).ToList();

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
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date).ToList();

                foreach (Client client in clients)
                {
                    // Manually perform cascade delete on the related (by a foreign key) Visits table.
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();
                    opiddailycontext.Visits.RemoveRange(client.Visits);

                    // Physically remove client from table
                    opiddailycontext.Clients.Remove(client);
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