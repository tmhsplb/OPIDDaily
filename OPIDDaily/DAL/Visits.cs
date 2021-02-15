using OPIDDaily.DataContexts;
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
        private static string MostRecentSender(int id)
        {
            // This method was too complicated to implement.  It was intended
            // to support color coding of a client's visit history. But instead
            // I just settled on indicating whether there was a conversation
            // going on regarding a particualr check. See method HavingConversation.
            return string.Empty;
        }

        private static bool HavingConversation(int id)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                bool havingConversation = opiddailycontext.TextMsgs.Any(tm => tm.Vid == id);

                return havingConversation;
            }
        }

        private static string NormalizedService(string service)
        {
            if (service.StartsWith("LBVD"))
            {
                if (service.Equals("LBVD"))
                {
                    return "BC";
                }
                else
                {
                    // Example: service = LBVD2 returns BC2
                    return string.Format("BC{0}", service.Substring(4));
                }
            }
            else
            {
                return service;
            }
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

            return new VisitViewModel
            {
                Id = ancientCheck.Id,
                Date = date.AddHours(12),
                Conversation = (HavingConversation(ancientCheck.Id) ? "Y" : string.Empty),
                Item = NormalizedService(ancientCheck.Service),
                Check = (ancientCheck.Num == 0 ? string.Empty : ancientCheck.Num.ToString()),
                Status = ancientCheck.Disposition,
                Notes = ancientCheck.Notes,
                Sender = MostRecentSender(ancientCheck.Id)
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

            return new VisitViewModel
            {
                Id = rcheck.Id,
                Date = date.AddHours(12),
                Conversation = (HavingConversation(rcheck.Id) ? "Y" : string.Empty),
                Item = NormalizedService(rcheck.Service),
                Check = (rcheck.Num == 0 ? string.Empty: rcheck.Num.ToString()),
                Status = rcheck.Disposition,
                Notes = rcheck.Notes,
                Sender = MostRecentSender(rcheck.Id)
            };
        }

        private static VisitViewModel PocketCheckToVisitViewModel(PocketCheck pcheck)
        {
            return new VisitViewModel
            {
                Id = pcheck.Id,
                Date = pcheck.Date,
                Conversation = (HavingConversation(pcheck.Id) ? "Y" : string.Empty),
                Item = pcheck.Item,
                Check = pcheck.Num.ToString(),
                Status = pcheck.Disposition,
                Sender = MostRecentSender(pcheck.Id),
                Notes = pcheck.Notes
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
                    List<PocketCheck> pchecks = opiddailycontext.PocketChecks.Where(pc => pc.ClientId == client.Id && pc.IsActive == true).ToList();

                    List<VisitViewModel> visits = new List<VisitViewModel>();

                    foreach (AncientCheck ancientCheck in ancientChecks)
                    {
                        visits.Add(AncientCheckToVisitViewModel(ancientCheck, msgs));
                    }

                    foreach (RCheck rcheck in rchecks)
                    {
                        visits.Add(RCheckToVisitViewModel(rcheck, msgs));
                    }

                    foreach (PocketCheck pcheck in pchecks)
                    {
                        visits.Add(PocketCheckToVisitViewModel(pcheck));
                    }
                    
                    // Make sure that visits are listed by ascending date
                    visits = visits.OrderBy(v => v.Date).ToList();
                    return visits;
                }
            }

            return null;
        }

        public static List<VisitViewModel> GetPocketChecks(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);
                List<PocketCheck> pchecks = opiddailycontext.PocketChecks.Where(pc => pc.ClientId == client.Id && pc.IsActive == true).ToList();

                List<VisitViewModel> visits = new List<VisitViewModel>();

                foreach (PocketCheck pcheck in pchecks)
                {
                    if (pcheck.IsActive)
                    {
                        visits.Add(PocketCheckToVisitViewModel(pcheck));
                    }
                }

                // Make sure that visits are listed by ascending date
                visits = visits.OrderBy(v => v.Date).ToList();
                return visits;
            }
        }

        /*
        // A Pocket Check is unresolved if it has a non-zero check number and an unknown disposition.
        public static List<VisitViewModel> GetUnresolvedPocketChecks(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);
                List<PocketCheck> pchecks = opiddailycontext.PocketChecks.Where(pc => pc.ClientId == client.Id && pc.Num != 0  // && (pc.Disposition.Equals(null) || pc.Disposition.Equals(""))
                                                                                      && pc.IsActive == true).ToList();

                List<VisitViewModel> visits = new List<VisitViewModel>();

                foreach (PocketCheck pcheck in pchecks)
                {
                    if (pcheck.IsActive)
                    {
                        visits.Add(PocketCheckToVisitViewModel(pcheck));
                    }
                }

                // Make sure that visits are listed by ascending date
                visits = visits.OrderBy(v => v.Date).ToList();
                return visits;
            }
        }
        */

        private static PocketCheck NewPocketCheck(Client client, VisitViewModel vvm)
        {
            return new PocketCheck
            {
                ClientId = client.Id,
                Date = vvm.Date.AddHours(12),
                Name = Clients.ClientBeingServed(client, false),
                DOB = client.DOB,
                Item = vvm.Item,
                Num = Convert.ToInt32(vvm.Check),
                Disposition = vvm.Status,
                Notes = vvm.Notes,
                IsActive = true
            };
        }
              
        public static void AddPocketCheck(int nowServing, VisitViewModel vvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    PocketCheck pocketCheck = NewPocketCheck(client, vvm);

                    if (pocketCheck != null)
                    {
                        // A Pocket Check will be moved to the Research Table
                        // when it is resolved by loading an Origen Bank file
                        // of checks. See CheckManager.ResolvePocketChecks.
                        opiddailycontext.PocketChecks.Add(pocketCheck);
                        opiddailycontext.SaveChanges();
                    }
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
                    AncientCheck ancientCheck = opiddailycontext.AncientChecks.Find(vvm.Id);

                    if (ancientCheck != null)
                    {
                        ancientCheck.Num = Convert.ToInt32(vvm.Check);
                        ancientCheck.Service = vvm.Item;
                        ancientCheck.Disposition = vvm.Status;
                        ancientCheck.Notes = vvm.Notes;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    RCheck rcheck = opiddailycontext.RChecks.Find(vvm.Id);

                    if (rcheck != null)
                    {
                        rcheck.Num = Convert.ToInt32(vvm.Check);
                        rcheck.Service = vvm.Item;
                        rcheck.Disposition = vvm.Status;
                        rcheck.Notes = vvm.Notes;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    PocketCheck pcheck = opiddailycontext.PocketChecks.Find(vvm.Id);

                    if (pcheck != null)
                    {
                        pcheck.Item = vvm.Item;
                        pcheck.Num = Convert.ToInt32(vvm.Check);
                        pcheck.Disposition = vvm.Status;
                        pcheck.Notes = vvm.Notes;
                        opiddailycontext.SaveChanges();
                        return;
                    }        
                }
            }
        }

        public static void DeletePocketCheck(int nowServing, int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    PocketCheck pcheck = opiddailycontext.PocketChecks.Where(p => p.ClientId == nowServing).SingleOrDefault();

                    if (pcheck != null)
                    {
                        DeleteVisitNotes(pcheck.Id);
                        opiddailycontext.PocketChecks.Remove(pcheck);
                        opiddailycontext.SaveChanges();
                    }
                }
            }
        }

        private static VisitNoteModel TextMsgToVisitNoteModel(TextMsg textmsg)
        {
            return new VisitNoteModel
            {
                Id = textmsg.Vid,
                Date = textmsg.Date,
                From = textmsg.From,
                Note = textmsg.Msg
            };
        }

        public static List<VisitNoteModel> GetVisitNotes(int nowServing, int vid)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<TextMsg> textmsgs = opiddailycontext.TextMsgs.Where(m => m.Vid == vid).ToList();
                List<VisitNoteModel> visitNotes = new List<VisitNoteModel>();

                foreach (TextMsg textmsg in textmsgs)
                {
                    visitNotes.Add(TextMsgToVisitNoteModel(textmsg));
                }

                return visitNotes;
            }          
        }
         
        private static TextMsg VisitNoteModelToTextMsg(VisitNoteModel vnm, int vid)
        {
            return new TextMsg
            {
                Date = Extras.DateTimeToday().AddHours(12),
                Vid = vid,
                From = vnm.From,
                Msg = vnm.Note
            };
        }

        public static void AddVisitNote(int nowServing, int vid, string sender, VisitNoteModel vnm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                opiddailycontext.Entry(client).Collection(c => c.TextMsgs).Load();

                TextMsg textmsg = VisitNoteModelToTextMsg(vnm, vid);

                client.TextMsgs.Add(textmsg);

                opiddailycontext.SaveChanges();
            }
        }

        private static void VisitNoteModelToTextMsg(VisitNoteModel vnm, TextMsg textmsg)
        {
            textmsg.From = vnm.From;
            textmsg.Msg = vnm.Note;
        }

        public static void EditVisitNote(VisitNoteModel vnm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<TextMsg> textmsgs = opiddailycontext.TextMsgs.Where(m => m.Vid == vnm.Id).ToList();

                if (textmsgs.Count == 1)
                {
                    TextMsg textmsg = textmsgs[0];
                    VisitNoteModelToTextMsg(vnm, textmsg);
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static void DeleteVisitNote(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<TextMsg> textmsgs = opiddailycontext.TextMsgs.Where(m => m.Vid == id).ToList();
                TextMsg textmsg = textmsgs[0];

                if (textmsgs.Count == 1)
                {
                    opiddailycontext.TextMsgs.Remove(textmsg);
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static void DeleteVisitNotes(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<TextMsg> textmsgs = opiddailycontext.TextMsgs.Where(m => m.Vid == id).ToList();
                opiddailycontext.TextMsgs.RemoveRange(textmsgs);
                opiddailycontext.SaveChanges();  
            }
        }
    }
}