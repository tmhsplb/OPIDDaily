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
    }
}