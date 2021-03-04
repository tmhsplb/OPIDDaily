using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OPIDDaily.Models;
using OPIDDaily.DataContexts;
using OpidDailyEntities;

namespace OPIDDaily.DAL
{
    public class PocketChecks
    {
        private static PocketCheckViewModel PocketCheckToPocketCheckViewModel(Client client, PocketCheck pc)
        {
            return new PocketCheckViewModel
            {
                Id = pc.Id,
                AgencyName = (!string.IsNullOrEmpty(client.AgencyName) ? client.AgencyName : Agencies.GetAgencyName(client.AgencyId)),
                Name = pc.Name,
                HeadOfHousehold = (pc.HeadOfHousehold ? "Y" : string.Empty),
                Date = pc.Date,
                Item = pc.Item,
                Check = pc.Num,
                Status = pc.Disposition,
                Notes = pc.Notes
            };
        }

        private static bool IsPocketCheck(PocketCheck pcheck)
        {
            // Does not depend on Disposition, which might not be determined yet.
            // It's still a pocket check based on check number alone.
            // See CheckManager.PocketCheck which DOES depend on disposition.
            return 0 < pcheck.Num && pcheck.Num < 9999;
        }

        public static List<PocketCheckViewModel> GetPocketChecks()
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                List<PocketCheck> pchecks = opiddailycontext.PocketChecks.Where(pc => pc.HH == 0 && pc.IsActive == true).ToList();
                List<PocketCheckViewModel> pocketChecks = new List<PocketCheckViewModel>();

                foreach (PocketCheck pcheck in pchecks)
                {
                    if (IsPocketCheck(pcheck))
                    {
                        Client client = opiddailycontext.Clients.Find(pcheck.ClientId);

                        if (client != null)
                        {
                            pocketChecks.Add(PocketCheckToPocketCheckViewModel(client, pcheck));
                        }
                    }
                }
 
                // Make sure that pocket checks are listed in alphabetical order
                pocketChecks = pocketChecks.OrderBy(pc => pc.Name).ToList();
                return pocketChecks;
            }
        }

        public static void EditPocketCheck(PocketCheckViewModel pcvm)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                PocketCheck pcheck = opiddailycontext.PocketChecks.Where(pc => pc.Id == pcvm.Id).SingleOrDefault();

                if (pcheck != null)
                {
                    pcheck.Notes = pcvm.Notes;
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static List<PocketCheckViewModel> GetDependentPocketChecks(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                PocketCheck pcheck = opiddailycontext.PocketChecks.Find(id);

                if (pcheck != null)
                {
                    Client client = opiddailycontext.Clients.Find(pcheck.ClientId);

                    if (client != null)
                    {
                        List<PocketCheck> dependentPocketChecks = opiddailycontext.PocketChecks.Where(pc => pc.HH == client.Id).ToList();
                        List<PocketCheckViewModel> pcvms = new List<PocketCheckViewModel>();

                        foreach (PocketCheck dependentPocketCheck in dependentPocketChecks)
                        {
                            PocketCheckViewModel pcvm = PocketCheckToPocketCheckViewModel(client, dependentPocketCheck);
                            pcvms.Add(pcvm);
                        }

                        return pcvms;
                    }

                    return null;
                }

                return null;
            }
        }
    }
}