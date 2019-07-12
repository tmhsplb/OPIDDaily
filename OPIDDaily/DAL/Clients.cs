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
        private static ClientViewModel ClientToClientViewModel(Client client)
        {
            return new ClientViewModel
            {
                Id = client.Id,
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
            client.LastName = cvm.LastName;
            client.FirstName = cvm.FirstName;
            client.MiddleName = cvm.MiddleName;
            client.BirthName = cvm.BirthName;
            client.DOB = DateTime.Parse(cvm.DOB);
            client.Age = CalculateAge(DateTime.Parse(cvm.DOB));
            client.Notes = cvm.Notes;
        }


        public static List<ClientViewModel> GetClients()
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
                List<Client> clients = opiddailycontext.Clients.ToList();

                foreach (Client client in clients)
                {
                    clientCVMS.Add(ClientToClientViewModel(client));
                }

                return clientCVMS;
            }
        }

        public static void AddClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = new Client
                {
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
                    DOB = DateTime.Parse(cvm.DOB),
                    Age = CalculateAge(DateTime.Parse(cvm.DOB)),
                    Notes = cvm.Notes
                };

                opidcontext.Clients.Add(client);
                opidcontext.SaveChanges();
            }
        }

        public static string EditClient(ClientViewModel cvm)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(cvm.Id);

                if (client != null)
                {        
                    ClientViewModelToClientEntity(cvm, client);
                    opidcontext.SaveChanges();
                    return "Success";
                }

                return "Failure";
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