using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.DAL
{
    public class Agencies
    {
        
        public static SelectList GetAgenciesSelectList()
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<Agency> agencies = opiddailycontext.Agencies.ToList();
                return new SelectList(agencies, "Id", "AgencyName");
            }
        }

        public static string GetAgencyName(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Agency agency = opiddailycontext.Agencies.Find(id);

                if (agency != null)
                {
                    return agency.AgencyName;
                }

                return "Unknown";   
            }
        }
        

        private static Agency AgencyViewModelToAgency(AgencyViewModel avm)
        {
            return new Agency
            {
                AgencyId = avm.AgencyId,
                AgencyName = avm.AgencyName,
                ContactPerson = avm.ContactPerson,
                Phone = avm.Phone,
                Email = avm.Email,
                IsActive = (avm.IsActive == "Yes" ? true : false)
            };
        }

        private static AgencyViewModel AgencyToAgencyViewModel(Agency agency)
        {
            return new AgencyViewModel
            {
                Id = agency.Id,
                AgencyId = agency.AgencyId,
                AgencyName = agency.AgencyName,
                ContactPerson = agency.ContactPerson,
                Phone = agency.Phone,
                Email = agency.Email,
                IsActive = (agency.IsActive ? "Yes" : string.Empty)
            };
        }

        public static List<AgencyViewModel> GetAgencies(string sidx, string sord)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<AgencyViewModel> avm = new List<AgencyViewModel>();
                List<Agency> agencies = opiddailycontext.Agencies.ToList();

                foreach (Agency agency in agencies)
                {
                    avm.Add(AgencyToAgencyViewModel(agency));
                }

                switch (sidx)
                {
                    case "AgencyName":
                        if (sord.ToUpper().Equals("DESC"))
                        {
                            avm = avm.OrderByDescending(a => a.AgencyName).ToList();
                        }
                        else
                        {
                            avm = avm.OrderBy(a => a.AgencyName).ToList();
                        }
                        break;
                    default:
                        break;
                }

                return avm;
            }
        }

        public static void AddAgency(AgencyViewModel avm)
        {
            Agency agency = AgencyViewModelToAgency(avm);

            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                opiddailycontext.Agencies.Add(agency);
                opiddailycontext.SaveChanges();
            }
        }

        public static void EditAgency(AgencyViewModel avm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Agency agency = opiddailycontext.Agencies.Find(avm.Id);

                agency.AgencyId = avm.AgencyId;
                agency.AgencyName = avm.AgencyName;
                agency.ContactPerson = avm.ContactPerson;
                agency.Phone = avm.Phone;
                agency.Email = avm.Email;
                agency.IsActive = (avm.IsActive == "Yes" ? true : false);

                opiddailycontext.SaveChanges();
            }
        }

        public static void DeleteAgency(AgencyViewModel avm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Agency agency = opiddailycontext.Agencies.Find(avm.Id);

                opiddailycontext.Agencies.Remove(agency);

                opiddailycontext.SaveChanges();
            }
        }
    }
}