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
        private static string[] ExtractMsg(int vid, string[] msgs)
        {
            // msgs is an array of messages
            // Example: msgs = ["FromOPID:123588, FromAgency:123587]"
            // If there are no messages, then msgs = ["None:0"]
            foreach (string msg in msgs)
            {
                if (msg.Contains(vid.ToString()))
                {
                    // If vid = 123588, then using the example above, extractedMsg = ["FromOPID", "123588"]
                    string[] extractedMsg = msg.Split(':');
                    return extractedMsg;
                }
            }

            string[] none = new string[] { "None", "0" };
            return none;
        }

        private static VisitViewModel VisitToVisitViewModel(Visit visit, string[] msgs)
        {
            string[] msg = ExtractMsg(visit.Id, msgs);
            int extractedVid = Convert.ToInt32(msg[1]);
            string sender = msg[0];

            return new VisitViewModel
            {
                Id = visit.Id,
                Date = visit.Date.AddHours(12),
                Item = visit.Item,
                Check = (string.IsNullOrEmpty(visit.Check) || visit.Check.Equals("0") ? string.Empty : visit.Check),
                Status = visit.Status,
                Sender = visit.Id == extractedVid ? sender : string.Empty
            };
        }

        private static VisitViewModel AncientCheckToVisitViewModel(AncientCheck ancientCheck, string[] msgs)
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

            string[] msg = ExtractMsg(ancientCheck.Id, msgs);
            int extractedVid = Convert.ToInt32(msg[1]);
            string sender = msg[0];

            return new VisitViewModel
            {
                Id = ancientCheck.Id,
                Date = date.AddHours(12),  
                Item = ancientCheck.Service,
                Check = (ancientCheck.Num == 0 ? string.Empty : ancientCheck.Num.ToString()),
                Status = ancientCheck.Disposition,
                Sender = ancientCheck.Id == extractedVid ? sender : string.Empty
            };
        }

        private static VisitViewModel RCheckToVisitViewModel(RCheck rcheck, string[] msgs)
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

            string[] msg = ExtractMsg(rcheck.Id, msgs);
            int extractedVid = Convert.ToInt32(msg[1]);
            string sender = msg[0];

            return new VisitViewModel
            {
                Id = rcheck.Id,
                Date = date.AddHours(12), 
                Item = rcheck.Service,
                Check = (rcheck.Num == 0 ? string.Empty: rcheck.Num.ToString()),
                Status = rcheck.Disposition,
                Sender = rcheck.Id == extractedVid ? sender : string.Empty
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
                Client client = opiddailycontext.Clients.Find(nowServing);
                string[] msgs = (string.IsNullOrEmpty(client.Msgs) ? new[] {"None:0"} : client.Msgs.Split(','));
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
                        string check = visit.Check;
                        int icheck = 0;

                        if (!string.IsNullOrEmpty(check))
                        {
                            icheck = Convert.ToInt32(check);
                        }

                        // Exclude visit notes which all have icheck < 0
                        if (icheck >= 0)
                        {
                            visits.Add(VisitToVisitViewModel(visit, msgs));
                        }
                    }

                    foreach (AncientCheck ancientCheck in ancientChecks)
                    {
                        visits.Add(AncientCheckToVisitViewModel(ancientCheck, msgs));
                    }

                    foreach (RCheck rcheck in rchecks)
                    {
                        visits.Add(RCheckToVisitViewModel(rcheck, msgs));
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
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    Visit visit = client.Visits.Where(v => v.Id == vvm.Id).SingleOrDefault();

                    if (visit != null)
                    {
                        visit.Status = vvm.Status;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    AncientCheck ancientCheck = opiddailycontext.AncientChecks.Find(vvm.Id);

                    if (ancientCheck != null)
                    {
                        ancientCheck.Disposition = vvm.Status;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    RCheck rcheck = opiddailycontext.RChecks.Find(vvm.Id);

                    if (rcheck != null)
                    {
                        rcheck.Disposition = vvm.Status;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    return;           
                }
            }
        }

        public static void DeleteVisit(int nowServing, int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    Visit visit = opiddailycontext.Visits.Find(id);

                    if (visit != null)
                    {
                        opiddailycontext.Visits.Remove(visit);
                        opiddailycontext.SaveChanges();
                    }
                }
            }
        }

        public static List<VisitNoteModel> GetVisitNotes(int nowServing, int rowId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();
                    List<VisitNoteModel> visitNotes = new List<VisitNoteModel>();
                    
                    foreach (Visit visit in client.Visits)
                    {
                        if (visit.Check == rowId.ToString())
                        {
                            visitNotes.Add(VisitToVisitNote(visit));
                        }
                    }

                    return visitNotes;
                }
            }

            return null;
        }
        
        private static VisitNoteModel VisitToVisitNote(Visit visit)
        {
            return new VisitNoteModel
            {
                Id = visit.Id,
                RowId = -Convert.ToInt32(visit.Check),
                Date = visit.Date,
                From = visit.Item,
                Note = visit.Notes
            };
        }
        
        private static Visit VisitNoteModelToVisit(int vid, VisitNoteModel vnm)
        {
            return new Visit
            {
                Date = Extras.DateTimeToday(),
                Item = vnm.From,
                Check = (-vid).ToString(),
                Notes = vnm.Note
            };
        }

        private static void PrependMsg(Client client, string side, int vid)
        {
            string msg = string.Format("From{0}:{1}", side, vid);

            // client.Msgs will be a comma separated list of messages
            // Example: client.Msgs = "FromOPID:123588,FromAgency:123587"
            // client.Notes is split into an array of meessages in method GetVisits.
            if (string.IsNullOrEmpty(client.Msgs))
            {
                client.Msgs = msg;
            }
            else
            {
                client.Msgs = string.Format("{0},{1}", msg, client.Msgs);
            }
            
        }

        public static void AddVisitNote(int nowServing, int vid, string side, VisitNoteModel vnm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);
               
                if (client != null)
                {
                    PrependMsg(client, side, vid);
                    
                    Visit visit = VisitNoteModelToVisit(vid, vnm);

                    opiddailycontext.Entry(client).Collection(c => c.Visits).Load();

                    client.Visits.Add(visit);

                    opiddailycontext.SaveChanges();
                }
            }
        }

        private static void VisitNoteModelToVisitEntity(VisitNoteModel vnm, Visit visit)
        {
            visit.Item = vnm.From;
            visit.Notes = vnm.Note;
        }

        public static void EditVisitNote(VisitNoteModel vnm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Visit visit = opiddailycontext.Visits.Find(vnm.Id); 

                if (visit != null)
                {
                    VisitNoteModelToVisitEntity(vnm, visit);
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static void DeleteVisitNote(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Visit visit = opiddailycontext.Visits.Find(id);
                opiddailycontext.Visits.Remove(visit);
                opiddailycontext.SaveChanges();
            }
        }
    }
}