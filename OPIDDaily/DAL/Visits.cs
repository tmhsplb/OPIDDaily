﻿using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
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
                Date = visit.Date,
                Item = visit.Item,
                Check = (string.IsNullOrEmpty(visit.Check) || visit.Check.Equals("0") ? string.Empty : visit.Check),
                Status = visit.Status,
                Notes = visit.Notes
            };
        }

        private static VisitViewModel AncientCheckToVisitViewModel(AncientCheck ancientCheck)
        {
            DateTime? adate = ancientCheck.Date;
            DateTime date;

            if (adate == null)
            {
                date = new DateTime(1900, 1, 1);
            }
            else
            {
                date = (DateTime)adate;
            }

            return new VisitViewModel
            {
                Id = ancientCheck.Id,
                Date = date,  
                Item = ancientCheck.Service,
                Check = (ancientCheck.Num == 0 ? string.Empty : ancientCheck.Num.ToString()),
                Status = ancientCheck.Disposition,
                Notes = string.Empty
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
                Date = date, 
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
                Date = vvm.Date,
                Item = vvm.Item,
                Check = vvm.Check,
                Status = vvm.Status,
                Notes = vvm.Notes
            };
        }

        public static List<VisitViewModel> GetVisits(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Where(c => c.Id == nowServing).FirstOrDefault();
                DateTime DOB = client.DOB;
                string lastName = Extras.StripSuffix(client.LastName.ToUpper());

                if (client != null)
                {
                    List<AncientCheck> ancientChecks = opiddailycontext.AncientChecks.Where(ac => ac.DOB == DOB && ac.Name.ToUpper().StartsWith(lastName)).ToList();
                    List<RCheck> rchecks = opiddailycontext.RChecks.Where(rc => rc.DOB == DOB && rc.Name.ToUpper().StartsWith(lastName)).ToList();

                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    List<VisitViewModel> visits = new List<VisitViewModel>();

                    foreach (Visit visit in client.Visits)
                    {
                        visits.Add(VisitToVisitViewModel(visit));
                    }

                    foreach (AncientCheck ancientCheck in ancientChecks)
                    {
                        visits.Add(AncientCheckToVisitViewModel(ancientCheck));
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
                        visit.Date = vvm.Date; 
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