using OpidDaily.Models;
using OPIDDaily.DataContexts;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class Clients
    {
        private static ClientViewModel ClientEntityToClientViewModel(Client client)
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
                Notes = client.Notes,
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
            client.Notes = cvm.Notes;
        }

        private static int GetUpdatedWaitTime(Client client)
        {
            if (client.Stage.Equals("CheckedIn"))
            {
                double waitTime = (DateTime.Now).Subtract(client.CheckedIn).TotalMinutes;
                int waitMinutes = (int)Math.Round(waitTime);
                return waitMinutes;
            }

            return 0;
        }

        public static List<ClientViewModel> GetClients(DateTime date)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                List<Client> clients = opiddailycontext.Clients.Where(c => c.ServiceDate == date).ToList();

                foreach (Client client in clients)
                {
                    client.WaitTime = GetUpdatedWaitTime(client);
                    clientCVMS.Add(ClientEntityToClientViewModel(client));
                }

                opiddailycontext.SaveChanges();
                return clientCVMS;
            }
        }

        public static void AddClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = new Client
                {
                    ServiceDate = DateTime.Today,
                    ServiceTicket = (string.IsNullOrEmpty(cvm.ServiceTicket) ? "?" : cvm.ServiceTicket),
                    Stage = "Screened",
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
                    DOB = (string.IsNullOrEmpty(cvm.DOB) ? DateTime.Today : DateTime.Parse(cvm.DOB)),
                    Age = (string.IsNullOrEmpty(cvm.DOB) ? 0 : CalculateAge(DateTime.Parse(cvm.DOB))),
                    Notes = cvm.Notes,
                    Screened = DateTime.Now,
                    CheckedIn = DateTime.Now,
                    Interviewing = DateTime.Now,
                    Interviewed = DateTime.Now,
                    BackOffice = DateTime.Now,
                    Done = DateTime.Now
                };

                opidcontext.Clients.Add(client);
                opidcontext.SaveChanges();
            }
        }

        public static void StageTransition(ClientViewModel cvm, Client client)
        {
            if (cvm.Stage.Equals("CheckedIn") && client.Stage.Equals("Screened"))
            {
                client.CheckedIn = DateTime.Now;
            }

            if (cvm.Stage.Equals("Interviewing") && client.Stage.Equals("CheckedIn"))
            {
                client.Interviewing = DateTime.Now;
            }

            if (cvm.Stage.Equals("Interviewed") && client.Stage.Equals("Interviewing"))
            {
                client.Interviewed = DateTime.Now;
            }

            if (cvm.Stage.Equals("BackOffice") && client.Stage.Equals("Interviewed"))
            {
                client.BackOffice = DateTime.Now;
            }

            if (cvm.Stage.Equals("Done") && client.Stage.Equals("BackOffice"))
            {
                client.Done = DateTime.Now;
            }
        }

        public static string EditClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(cvm.Id);

                if (client != null)
                {
                    StageTransition(cvm, client);
                    
                    ClientViewModelToClientEntity(cvm, client);
                    opidcontext.SaveChanges();
                    return "Success";
                }

                return "Failure";
            }
        }

        public static void DeleteClient(int id)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(id);
                opidcontext.Clients.Remove(client);
                opidcontext.SaveChanges();
            }
        }

        // https://stackoverflow.com/questions/9/calculate-age-in-c-sharp
        private static int CalculateAge(DateTime dob)
        {
            DateTime today = DateTime.Today;
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
                    string clientName = string.Format("{0} {1}", client.FirstName, client.LastName);
                    return clientName;
                }

                return "Unknown";
            }
        }
    }
}