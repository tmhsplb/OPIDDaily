using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.DAL
{
    public class Visits
    {
        private static VisitViewModel VisitToVisitViewModel(Visit visit)
        {
            return new VisitViewModel
            {
                Id = visit.Id,
                Date = visit.Date.AddHours(2),  // make the time be 12 noon
                Item = visit.Item,
                Check = (visit.Check.Equals("0") ? string.Empty : visit.Check),
                Status = visit.Status,
                Notes = visit.Notes
            };
        }

        private static VisitViewModel RCheckToVisitViewModel(RCheck rcheck)
        {
            DateTime? rdate = rcheck.Date;
            DateTime date;

            if (rdate == null)
            {
                date = new DateTime(1900, 1, 1);
            }
            else
            {
                date = (DateTime)rdate;
            }

            return new VisitViewModel
            {
                Id = rcheck.Id,
                Date = date.AddHours(12),  // make the time be 12 noon
                Item = rcheck.Service,
                Check = (rcheck.Num == 0 ? string.Empty: rcheck.Num.ToString()),
                Status = rcheck.Disposition,
                Notes = string.Empty
            };
        }

        private static Visit VisitViewModelToVisit(VisitViewModel vvm)
        {
            return new Visit
            {
                Date = vvm.Date.AddHours(12), // make the time be 12 noon
                Item = vvm.Item,
                Check = vvm.Check,
                Status = vvm.Status,
                Notes = vvm.Notes
            };
        }

        /*
        public static bool HasVisits(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                bool hasVisits;

                Client client = opiddailycontext.Clients.Where(c => c.Id == nowServing).FirstOrDefault();

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    hasVisits = client.Visits.Count() > 0;

                    return hasVisits;
                }

                return false;
            }
        }
        */

        public static List<VisitViewModel> GetVisits(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Where(c => c.Id == nowServing).FirstOrDefault();
                DateTime DOB = client.DOB;
                string lastName = client.LastName.ToUpper();

                if (client != null)
                {
                    List<RCheck> rchecks = opiddailycontext.RChecks.Where(rc => rc.DOB == DOB && rc.Name.ToUpper().StartsWith(lastName)).ToList();
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    List<VisitViewModel> visits = new List<VisitViewModel>();

                    foreach (Visit visit in client.Visits)
                    {
                        visits.Add(VisitToVisitViewModel(visit));
                    }

                    foreach (RCheck rcheck in rchecks)
                    {
                        visits.Add(RCheckToVisitViewModel(rcheck));
                    }

                    // Make sure that visits are listed by ascending referral date
                    visits = visits.OrderBy(v => v.Date).ToList();
                    return visits;
                }
            }

            return null;
        }

        public static void AddVisit(int nowServing, VisitViewModel vvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Where(c => c.Id == nowServing).SingleOrDefault();

                if (client != null)
                {
                    Visit visit = VisitViewModelToVisit(vvm);

                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    client.Visits.Add(visit);
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static void EditVisit(int nowServing, VisitViewModel vvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Where(c => c.Id == nowServing).SingleOrDefault();

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    Visit visit = opiddailycontext.Visits.Where(v => v.Id == vvm.Id).SingleOrDefault();

                    if (visit != null)
                    {
                        visit.Date = vvm.Date.AddHours(12);  // make the time be 12 noon
                        visit.Item = vvm.Item;
                        visit.Check = vvm.Check;
                        visit.Status = vvm.Status;
                        visit.Notes = vvm.Notes;
                    }

                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static void DeleteVisit(int nowServing, int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Where(c => c.Id == nowServing).SingleOrDefault();

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    Visit visit = opiddailycontext.Visits.Where(v => v.Id == id).SingleOrDefault();

                    if (visit != null)
                    {
                        opiddailycontext.Visits.Remove(visit);
                        opiddailycontext.SaveChanges();
                    }
                }
            }
        }
    }
}