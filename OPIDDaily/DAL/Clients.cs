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
                ReferralDate = client.ReferralDate.ToString("MM/dd/yyyy"),
                AppearanceDate = (client.AppearanceDate == (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null ? string.Empty : client.AppearanceDate.ToString("MM/dd/yyyy")),
                LastName = client.LastName,
                FirstName = client.FirstName,
                MiddleName = client.MiddleName,
                BirthName = client.BirthName,
                Notes = client.Notes,
            };
        }

        private static void ClientViewModelToClientEntity(ClientViewModel cvm, Client client)
        {
            client.ReferralDate = DateTime.Parse(cvm.ReferralDate);
            client.AppearanceDate = (string.IsNullOrEmpty(cvm.AppearanceDate) ? (System.DateTime)System.Data.SqlTypes.SqlDateTime.Null : DateTime.Parse(cvm.AppearanceDate));
            client.LastName = cvm.LastName;
            client.FirstName = cvm.FirstName;
            client.MiddleName = cvm.MiddleName;
            client.BirthName = cvm.BirthName;
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
                    ReferralDate = DateTime.Parse(cvm.ReferralDate),
                    AppearanceDate = DateTime.Parse(cvm.AppearanceDate),
                    FirstName = cvm.FirstName,
                    MiddleName = cvm.MiddleName,
                    LastName = cvm.LastName,
                    BirthName = cvm.BirthName,
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
    }
}