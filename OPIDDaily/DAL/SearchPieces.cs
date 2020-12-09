using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class SearchParameters
    {
        public string sidx { get; set; }

        public string sord { get; set; }

        public bool _search { get; set; }

        public string AgencyName { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }

        //  public string searchField { get; set; }

        //  public string searchOper { get; set; }

        //  public string searchString { get; set; }
    }

    public class SearchPieces
    {
        public JsonResult GetDashboard(SearchParameters sps, int page, int? rows = 15)
        {
            List<ClientViewModel> clients = Clients.GetDashboardClients(sps);

            int pageIndex = page - 1;
            int pageSize = (int)rows;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            clients = clients.OrderBy(c => c.Expiry).ToList();

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
    }

    public static List<ClientViewModel> GetDashboardClients(SearchParameters sps)
    {
        using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
        {
            List<ClientViewModel> clientCVMS = new List<ClientViewModel>();
            DateTime today = Extras.DateTimeToday();

            // A dashboard client will 
            //    come from an agency: c.AgencyId != 0
            //    be a head of household: c.HH == 0
            //    be not a same day client: c.ServiceDate != c.Expiry
            //    be unexpired: today <= c.Expiry
            // List<Client> clients = opiddailycontext.Clients.Where(c => c.AgencyId != 0 && c.HH == 0 && c.ServiceDate != c.Expiry && today <= c.Expiry).ToList();

            // For virtual front desk, c.AgencyId == 0, i.e. agency = OPID
            List<Client> clients = opiddailycontext.Clients.Where(c => c.HH == 0 && c.ServiceDate != c.Expiry && today <= c.Expiry && c.Active == true).ToList();

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

    private static List<ClientViewModel> GetFilteredDashboardClients(SearchParameters sps, List<ClientViewModel> cvms)
    {
        List<ClientViewModel> filteredCVMS;

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
}