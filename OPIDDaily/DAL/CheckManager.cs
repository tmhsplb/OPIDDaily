using Microsoft.AspNet.Identity.EntityFramework;
 
using OPIDDaily.Utils;
 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Mvc;
using DataTables.Mvc;
using System.Threading;
using OPIDDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.DataContexts;
using OpidDailyEntities;

namespace OPIDDaily.DAL
{
    public class CheckManager
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CheckManager));

        private static bool firstCall = true;
      //  private static List<int> incidentals;

        private static List<Check> newResearchChecks;
        private static List<Check> newTrackingChecks;
        private static List<CheckViewModel> resolvedChecks;
        private static List<Check> resolvedPocketChecks;
        private static List<int> mistakenlyResolved;
       // private static List<Check> typoChecks;

        public static void Init()
        {
            if (firstCall)
            {
             //   typoChecks = new List<Check>();
                resolvedChecks = new List<CheckViewModel>();
                mistakenlyResolved = new List<int>();
                firstCall = false;
            }

            newResearchChecks = new List<Check>();
            newTrackingChecks = new List<Check>();
            resolvedPocketChecks = new List<Check>();
            //  incidentals = new List<int>();
        }

        public static IQueryable<CheckViewModel> GetResolvedChecksAsQueryable()
        {
            if (resolvedChecks == null)
            {
                List<CheckViewModel> emptyList = new List<CheckViewModel>();
                return emptyList.AsQueryable();
            }

            return resolvedChecks.AsQueryable();
        }

        public static List<CheckViewModel> GetResolvedChecksAsList()
        {
            return resolvedChecks;
        }

        public static void PersistResearchChecks(List<DispositionRow> researchRows)
        {
            Init();
            List<Check> rChecks = DetermineResearchChecks(researchRows);
            UpdateCheckTables(rChecks);
        }

        public static void PersistAncientChecks(List<DispositionRow> researchRows)
        {
            Init();
            List<Check> ancientChecks = DetermineResearchChecks(researchRows);
            UpdateAncientChecksTable(ancientChecks);
        }

        public static void PersistTrackingChecks(List<TrackingRow> trackingRows)
        {
            Init();
            List<Check> trackingChecks = DetermineTrackingChecks(trackingRows);
            UpdateCheckTables(trackingChecks);
        }

        public static void NewResearchCheck(DispositionRow row, string service, DateTime? serviceDate, string disposition)
        {
            int checkNum;

            switch (service)
            {
                case "LBVD":
                    checkNum = row.LBVDCheckNum;
                    break;
                case "LBVD2":
                    checkNum = row.LBVDCheckNum2;
                    break;
                case "LBVD3":
                    checkNum = row.LBVDCheckNum3;
                    break;
                case "TID":
                    checkNum = row.TIDCheckNum;
                    break;
                case "TID2":
                    checkNum = row.TIDCheckNum2;
                    break;
                case "TID3":
                    checkNum = row.TIDCheckNum3;
                    break;
                case "TDL":
                    checkNum = row.TDLCheckNum;
                    break;
                case "TDL2":
                    checkNum = row.TDLCheckNum2;
                    break;
                case "TDL3":
                    checkNum = row.TDLCheckNum3;
                    break;
                case "MBVD":
                    checkNum = row.MBVDCheckNum;
                    break;
                case "MBVD2":
                    checkNum = row.MBVDCheckNum2;
                    break;
                case "MBVD3":
                    checkNum = row.MBVDCheckNum3;
                    break;

                // Supporting Documents
                /*
                case "SD1":
                    checkNum = row.SDCheckNum1;
                    break;
                case "SD2":
                    checkNum = row.SDCheckNum2;
                    break;
                case "SD3":
                    checkNum = row.SDCheckNum3;
                    break;

                case "SD12":
                    checkNum = row.SDCheckNum12;
                    break;
                case "SD22":
                    checkNum = row.SDCheckNum22;
                    break;
                case "SD32":
                    checkNum = row.SDCheckNum32;
                    break;

                case "SD13":
                    checkNum = row.SDCheckNum13;
                    break;
                case "SD23":
                    checkNum = row.SDCheckNum23;
                    break;
                case "SD33":
                    checkNum = row.SDCheckNum33;
                    break;
                */

                default:
                    checkNum = -1;
                    break;
            }

            newResearchChecks.Add(new Check
            {
                RecordID = row.RecordID,
                InterviewRecordID = row.InterviewRecordID,
                Num = checkNum,
                Name = string.Format("{0}, {1}", row.Lname, row.Fname),
                DOB = row.DOB,
                Date = serviceDate,
                Service = service,
                Disposition = disposition
            });
        }

        private static List<Check> DetermineResearchChecks(List<DispositionRow> researchRows)
        {
            foreach (DispositionRow row in researchRows)
            {
                bool lbvd = false, tid = false, tdl = false, mbvd = false;

                if (row != null) // NewDispositionRow may have found a corrupt record and inserted a null
                {
                    if (!string.IsNullOrEmpty(row.RequestedItem))
                    {
                        string[] services = row.RequestedItem.Split('|');
                        lbvd = services.Contains("LBVD");
                        tid = services.Contains("TID");
                        tdl = services.Contains("TDL");
                        mbvd = services.Contains("MBVD");
                    }

                    if (row.LBVDCheckNum != 0 || (lbvd && row.LBVDCheckNum == 0))
                    {
                        NewResearchCheck(row, "LBVD", row.Date, row.LBVDCheckDisposition);
                    }

                    if (row.LBVDCheckNum2 != 0)
                    {
                        NewResearchCheck(row, "LBVD2", row.LBVDOrderDateTwo, row.LBVDCheck2Disposition);
                    }

                    if (row.LBVDCheckNum3 != 0)
                    {
                        NewResearchCheck(row, "LBVD3", row.LBVDOrderDateThree, row.LBVDCheck3Disposition);
                    }

                    if (row.TIDCheckNum != 0 || (tid && row.TIDCheckNum == 0))
                    {
                        NewResearchCheck(row, "TID", row.Date, row.TIDCheckDisposition);
                    }

                    if (row.TIDCheckNum2 != 0)
                    {
                        NewResearchCheck(row, "TID2", row.TIDOrderDateTwo, row.TIDCheck2Disposition);
                    }

                    if (row.TIDCheckNum3 != 0)
                    {
                        NewResearchCheck(row, "TID3", row.TIDOrderDateThree, row.TIDCheck3Disposition);
                    }

                    if (row.TDLCheckNum != 0 || (tdl && row.TDLCheckNum == 0))
                    {
                        NewResearchCheck(row, "TDL", row.Date, row.TDLCheckDisposition);
                    }

                    if (row.TDLCheckNum2 != 0)
                    {
                        NewResearchCheck(row, "TDL2", row.TDLOrderDateTwo, row.TDLCheck2Disposition);
                    }

                    if (row.TDLCheckNum3 != 0)
                    {
                        NewResearchCheck(row, "TDL3", row.TDLOrderDateThree, row.TDLCheck3Disposition);
                    }

                    if (row.MBVDCheckNum != 0 || (mbvd && row.MBVDCheckNum == 0))
                    {
                        NewResearchCheck(row, "MBVD", row.Date, row.MBVDCheckDisposition);
                    }

                    if (row.MBVDCheckNum2 != 0)
                    {
                        NewResearchCheck(row, "MBVD2", row.MBVDOrderDateTwo, row.MBVDCheck2Disposition);
                    }

                    if (row.MBVDCheckNum3 != 0)
                    {
                        NewResearchCheck(row, "MBVD3", row.MBVDOrderDateThree, row.MBVDCheck3Disposition);
                    }

                    // Supporting documents
                    /*
                    // row.Date
                    if (row.SDCheckNum1 != 0)
                    {
                        NewResearchCheck(row, "SD1", row.Date, row.SDCheckDisposition);
                    }
                    if (row.SDCheckNum2 != 0)
                    {
                        NewResearchCheck(row, "SD2", row.Date, row.SDCheckDisposition2);
                    }
                    if (row.SDCheckNum3 != 0)
                    {
                        NewResearchCheck(row, "SD3", row.Date, row.SDCheckDisposition2);
                    }

                    // row.SDOrderDate2
                    if (row.SDCheckNum12 != 0)
                    {
                        NewResearchCheck(row, "SD12", row.SDOrderDate2, row.SDCheckDisposition12);
                    }
                    if (row.SDCheckNum22 != 0)
                    {
                        NewResearchCheck(row, "SD22", row.SDOrderDate2, row.SDCheckDisposition22);
                    }
                    if (row.SDCheckNum32 != 0)
                    {
                        NewResearchCheck(row, "SD32", row.SDOrderDate2, row.SDCheckDisposition32);
                    }

                    // row.SDOrderDate3
                    if (row.SDCheckNum13 != 0)
                    {
                        NewResearchCheck(row, "SD13", row.SDOrderDate3, row.SDCheckDisposition13);
                    }
                    if (row.SDCheckNum23 != 0)
                    {
                        NewResearchCheck(row, "SD23", row.SDOrderDate3, row.SDCheckDisposition23);
                    }
                    if (row.SDCheckNum33 != 0)
                    {
                        NewResearchCheck(row, "SD33", row.SDOrderDate3, row.SDCheckDisposition33);
                    }
                    */
                }
            }

            return newResearchChecks;
        }

        private static void NewTrackingCheck(TrackingRow row)
        {
            newTrackingChecks.Add(new Check
            {
                RecordID = row.RecordID,
                InterviewRecordID = row.InterviewRecordID,
                Num = row.CheckNum,
                Name = string.Format("{0}, {1}", row.Lname, row.Fname),
                DOB = row.DOB,
                Date = (row.OrderDate != null ? row.OrderDate : row.Date),
                Service = row.RequestedItem,
                Disposition = row.CheckDisposition
            });
        }

        private static List<Check> DetermineTrackingChecks(List<TrackingRow> trackingRows)
        {
            foreach (TrackingRow row in trackingRows)
            {
                if (row != null)
                {
                    // row will be null in case of an error, a reissued check or a scammed check.
                    // Don't let an error stop the processing of the rest of the file. Check
                    // the log file for an error if one is suspected after loading an 
                    // OPID Daily Tracking file.
                    // In the case of a reissued or scammed check the disposition of an existing check
                    // will be changed instead of adding a new tracking row. See method
                    // MyExcelDataReader.NewTrackingRow
                    NewTrackingCheck(row);
                }
            }

            return newTrackingChecks;
        }

        private static RCheck NewRCheck(Check check, string checkDate)
        {
            return new RCheck
            {
                RecordID = check.RecordID,
                sRecordID = check.RecordID.ToString(),
                InterviewRecordID = check.InterviewRecordID,
                sInterviewRecordID = check.InterviewRecordID.ToString(),
                Num = check.Num,
                sNum = check.Num.ToString(),
                Name = check.Name,
                DOB = check.DOB,
                sDOB = check.DOB.ToString("MM/dd/yyyy"),
                Date = Convert.ToDateTime(checkDate),
                sDate = (check.Date == null ? string.Empty : checkDate),
                Service = check.Service,
                Disposition = check.Disposition,
            };
        }

        private static void UpdateCheckTables(List<Check> checks)
        {
            int i = 0;
            int checkCount = checks.Count;

            try
            {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    // Using context.AddRange as described in: https://entityframework.net/improve-ef-add-performance
                    opidcontext.Configuration.AutoDetectChangesEnabled = false;
                    List<RCheck> rchecks = new List<RCheck>();
                   
                    foreach (Check check in checks)
                    {
                        bool uec = UpdateExistingCheck(check, opidcontext);

                        if (!uec)
                        {
                            // This check will become a new RCheck (even if it is ancient)
                            string checkDate = "01/01/1900";

                            if (check.Date != null)
                            {
                                // Coerce from DateTime? to DateTime, then get date string
                                checkDate = ((DateTime)check.Date).ToString("MM/dd/yyyy");
                            }

                            RCheck rcheck = NewRCheck(check, checkDate);

                            rchecks.Add(rcheck);
                        }

                        // Slow down updating/adding a little bit so we can see the progress bar
                        // Thread.Sleep(10);

                        i += 1;
                        DailyHub.SendProgress("Updating Research Table...", i, checkCount);
                    }

                    opidcontext.RChecks.AddRange(rchecks);
                    opidcontext.ChangeTracker.DetectChanges();
                    opidcontext.SaveChanges(); 
                }
            }
            catch (Exception e)
            {
                // Check the value of problemCheck;
                Log.Error(e.Message);
            }
        }

        private static bool UpdateExistingCheck(Check check, OpidDailyDB opidcontext)
        {
            int recordID = check.RecordID;
            bool found = false;

            // There may be multiple existing checks that share the same check number.
            // For example, members of the same household using a single check number
            // to cover the cost of a birth certificate for each. They all get resolved
            // with the same disposition as the disposition of this check.

            List<PocketCheck> pocketMatches = opidcontext.PocketChecks.Where(u => u.Num == check.Num).ToList();
            foreach (PocketCheck pocketCheck in pocketMatches)
            {
                pocketCheck.Disposition = check.Disposition;
                found = true;
            }

            if (found) return true;

            return UpdateExistingResearchCheck(check, opidcontext);
        }

        private static bool UpdateExistingResearchCheck(Check check, OpidDailyDB opidcontext)
        {
            bool found = false;

            List<RCheck> currentMatches = opidcontext.RChecks.Where(u => u.Num == check.Num).ToList();
            foreach (RCheck rcheck in currentMatches)
            {
                if (rcheck.RecordID == check.RecordID)
                {
                    rcheck.InterviewRecordID = (rcheck.InterviewRecordID == 0 ? check.InterviewRecordID : rcheck.InterviewRecordID);
                    rcheck.Disposition = check.Disposition;
                    found = true;
                }
            }

            if (found) return true;

            List<AncientCheck> ancientMatches = opidcontext.AncientChecks.Where(u => u.Num == check.Num).ToList();
            foreach (AncientCheck acheck in ancientMatches)
            {
                if (acheck.RecordID == check.RecordID)
                {
                    acheck.InterviewRecordID = (acheck.InterviewRecordID == 0 ? check.InterviewRecordID : acheck.InterviewRecordID);
                    acheck.Disposition = check.Disposition;
                    found = true;
                }
            }

            if (found) return true;

            return false;
        }

        private static AncientCheck NewAncientCheck(Check check, string checkDate)
        {
            return new AncientCheck
            {
                RecordID = check.RecordID,
                sRecordID = check.RecordID.ToString(),
                InterviewRecordID = check.InterviewRecordID,
                sInterviewRecordID = check.InterviewRecordID.ToString(),
                Num = check.Num,
                sNum = check.Num.ToString(),
                Name = check.Name,
                DOB = check.DOB,
                sDOB = check.DOB.ToString("MM/dd/yyyy"),
                Date = check.Date,
                sDate = (check.Date == null ? string.Empty : checkDate),
                Service = check.Service,
                Disposition = check.Disposition,
            };
        }
        
        private static void UpdateAncientChecksTable(List<Check> ancientChecks)
        {
            bool saveIndividualChecks = false;
            AncientCheck problemCheck;
            int i = 0;
            int checkCount = ancientChecks.Count;

            try
            {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    // Using context.AddRange as described in: https://entityframework.net/improve-ef-add-performance
                    opidcontext.Configuration.AutoDetectChangesEnabled = false;
                   
                    List<AncientCheck> achecks = new List<AncientCheck>();

                    foreach (Check check in ancientChecks)
                    {
                        int recordID = check.RecordID;
                        bool found = false;
                        
                        List<AncientCheck> ancientMatches = opidcontext.AncientChecks.Where(u => u.Num == check.Num).ToList();

                        // There may be multiple existing checks that share the same check number.
                        // For example, members of the same household using a single check number
                        // to cover the cost of a birth certificate for each. They all get resolved
                        // with the same disposition as the disposition of this check.
                        foreach (AncientCheck acheck in ancientMatches)
                        {
                            if (acheck.RecordID == recordID)
                            {
                                found = true;
                                acheck.Disposition = check.Disposition;
                            }
                        }

                        if (!found)
                        {
                            // This check represents a new AncientCheck
                            string checkDate = "01/01/1900";

                            if (check.Date != null)
                            {
                                // Coerce from DateTime? to DateTime, then get date string
                                checkDate = ((DateTime)check.Date).ToString("MM/dd/yyyy");
                            }

                            AncientCheck acheck = NewAncientCheck(check, checkDate); 
                            achecks.Add(acheck);

                            if (saveIndividualChecks)
                            {
                                // Only save individual checks if trying to isolate a check saving problem
                                problemCheck = acheck;
                                opidcontext.AncientChecks.Add(acheck);
                                opidcontext.SaveChanges();
                            }
                        }

                        // Slow down updating/adding a little bit so we can see the progress bar
                        // Thread.Sleep(10);

                        i += 1;
                        DailyHub.SendProgress("Updating Research Table...", i, checkCount);
                    }

                    if (!saveIndividualChecks)
                    {
                        opidcontext.AncientChecks.AddRange(achecks);
                        opidcontext.ChangeTracker.DetectChanges();
                        opidcontext.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                // Check the value of problemCheck;
                Log.Error(e.Message);
            }
        }

        private static CheckViewModel NewCheckViewModel(RCheck rc)
        {
            return new CheckViewModel
            {
                RecordID = rc.RecordID,
                InterviewRecordID = rc.InterviewRecordID,
                Num = rc.Num,
                Name = rc.Name,
                DOB = rc.DOB,
                Date = string.IsNullOrEmpty(rc.Date.ToString()) ? string.Empty : ((DateTime)rc.Date).ToShortDateString(),
                Service = rc.Service,
                Disposition = rc.Disposition
            };
        }

        public static List<CheckViewModel> GetChecks()
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                var rchecks = (from check in opidcontext.RChecks select check).ToList();

                List<CheckViewModel> checks = new List<CheckViewModel>();

                foreach (RCheck rc in rchecks)
                {
                    CheckViewModel cvm = NewCheckViewModel(rc);
                    checks.Add(cvm);
                }

                return checks;
            }
        }
 
        public static void DeleteResearchChecksForYear(int year)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                // Using RemoveRange as described in: https://stackoverflow.com/questions/21568479/how-can-i-delete-1-000-rows-with-ef6
                var checksToDelete = opidcontext.RChecks.Where(c => c.Date != null & c.Date.Value.Year == year);
                opidcontext.RChecks.RemoveRange(checksToDelete);
                opidcontext.SaveChanges();
            }
        }

        public static void DeleteAncientChecksForYear(int year)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                // Using RemoveRange as described in: https://stackoverflow.com/questions/21568479/how-can-i-delete-1-000-rows-with-ef6
                var checksToDelete = opidcontext.AncientChecks.Where(c => c.Date != null & c.Date.Value.Year == year);
                opidcontext.AncientChecks.RemoveRange(checksToDelete);
                opidcontext.SaveChanges();
            }
        }

        private static Check NewRCheck(RCheck rc)
        {
            return new Check
            {
                RecordID = rc.RecordID,
                InterviewRecordID = rc.InterviewRecordID,
                Num = rc.Num,
                Name = rc.Name,
                Date = rc.Date,
                Service = rc.Service,
                Disposition = rc.Disposition,
            };
        }

        private static Check NewACheck(AncientCheck ac)
        {
            return new Check
            {
                RecordID = ac.RecordID,
                InterviewRecordID = ac.InterviewRecordID,
                Num = ac.Num,
                Name = ac.Name,
                Date = ac.Date,
                Service = ac.Service,
                Disposition = ac.Disposition,
            };
        }

        public static List<Check> GetResearchChecks()
        {
            List<Check> researchChecks = new List<Check>();
            Check check;

            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                List<RCheck> rchecks = opidcontext.RChecks.Select(c => c).ToList();
                List<AncientCheck> ancientChecks = opidcontext.AncientChecks.Select(a => a).ToList();

                foreach (var rc in rchecks)
                {
                    check = NewRCheck(rc);
                    researchChecks.Add(check);
                }

                foreach (var ac in ancientChecks)
                {
                    check = NewACheck(ac);
                    researchChecks.Add(check);
                }
            }

            return researchChecks;
        }

        public static bool HasHistory(int nowServing)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                Client client = opidcontext.Clients.Find(nowServing);

                if (client != null)
                {
                    DateTime DOB = client.DOB;
                    string lastName = Extras.StripSuffix(client.LastName.ToUpper());

                    bool rchecks = opidcontext.RChecks.Any(rc => rc.DOB == DOB && rc.Name.StartsWith(lastName));

                    if (rchecks)
                    {
                        return true;
                    }

                    bool ancientChecks = opidcontext.AncientChecks.Any(ac => ac.DOB == DOB && ac.Name.StartsWith(lastName));

                    if (ancientChecks)
                    {
                        return true;
                    }

                    bool pchecks = opidcontext.PocketChecks.Any(pc => pc.ClientId == nowServing && pc.IsActive == true);

                    if (pchecks)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static void ResolveResearchChecks()
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                var recentChecks = opidcontext.RChecks;
                var ancientChecks = opidcontext.AncientChecks;

                foreach (CheckViewModel check in resolvedChecks)
                {
                    List<RCheck> rchecks = recentChecks.Where(u => u.Num == check.Num).ToList();
                    List<AncientCheck> achecks = ancientChecks.Where(u => u.Num == check.Num).ToList();

                    foreach (RCheck rcheck in rchecks)
                    {
                        rcheck.Disposition = check.Disposition;
                    }

                    foreach (AncientCheck acheck in achecks)
                    {
                        acheck.Disposition = check.Disposition;
                    }
                }

                opidcontext.SaveChanges();
            }
        }

        public static bool IsProtectedCheck(string disposition)
        {
            if (string.IsNullOrEmpty(disposition))
            {
                return false;
            }

            return disposition.Equals("Voided/Replaced")
                || disposition.Equals("Voided/Reissued")
                || disposition.Equals("Voided/No Reissue")
                || disposition.Equals("Voided/Reissue Other")
                || disposition.Equals("Scammed Check");
        }

        public static void ResolvePocketChecks(List<Check> excelChecks, string disposition)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<int> resolvedClients = new List<int>();
                var pchecks = opiddailycontext.PocketChecks;
                bool resolved;

                foreach (PocketCheck pcheck in pchecks)
                {
                    resolved = resolvedChecks.Any(r => r.Num == pcheck.Num);

                    if (resolved && !IsProtectedCheck(pcheck.Disposition))
                    {
                        // If pcheck is among resolvedChecks, then mark pcheck as inactive
                        // This will not happen when we are no longer updating the Research Table
                        // by uploading an OPID Daily Tracking file from Apricot and copying Origen
                        // checks directy into the Research Table. Only the else-clause will happen,
                        // that is, we will only resolve Pocket Checks by loading Origen checks.
                        pcheck.IsActive = false;
                        resolvedClients.Add(pcheck.ClientId);
                    }
                    else
                    {
                       // resolved = excelChecks.Any(e => e.Num == pcheck.Num);

                        Check echeck = excelChecks.Where(e => e.Num == pcheck.Num).SingleOrDefault();

                        if (echeck != null && !IsProtectedCheck(pcheck.Disposition))
                        { 
                            // This is the case of an Origen Bank check resolving a pocket check.
                            // This pocket check will become a full-fledged research check.  
                            pcheck.IsActive = false;
                            resolvedClients.Add(pcheck.ClientId);
                            NewResolvedPocketCheck(echeck, pcheck, disposition);
                        }
                    }
                }

                if (resolvedClients.Count > 0)
                {
                    foreach (PocketCheck pcheck in pchecks)
                    {
                        if (IsProtectedCheck(pcheck.Disposition) && resolvedClients.Contains(pcheck.ClientId))
                        {
                            // A protected pocket check belonging to a resolved client will be removed from
                            // the PocketChecks table and moved into the ResearchChecks table.
                            pcheck.IsActive = false;
                            NewResolvedPocketCheck(null, pcheck, pcheck.Disposition);
                        }
                    }
                }

                opiddailycontext.SaveChanges();           
            }

            DeleteInactivePocketChecks();
            UpdateResearchTableByResolvedPocketChecks();
        }

        private static void DeleteInactivePocketChecks()
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                // Using RemoveRange as described in: https://stackoverflow.com/questions/21568479/how-can-i-delete-1-000-rows-with-ef6
                var pocketChecksToDelete = opidcontext.PocketChecks.Where(pc => pc.IsActive == false);
                opidcontext.PocketChecks.RemoveRange(pocketChecksToDelete);
                opidcontext.SaveChanges();
            }
        }

        private static void UpdateResearchTableByResolvedPocketChecks()
        {
            try
            {
                using (OpidDailyDB opidcontext = new OpidDailyDB())
                {
                    // Using context.AddRange as described in: https://entityframework.net/improve-ef-add-performance
                    opidcontext.Configuration.AutoDetectChangesEnabled = false;
                    List<RCheck> rchecks = new List<RCheck>();
                    int i = 0;
                    int checkCount = resolvedPocketChecks.Count;

                    foreach (Check check in resolvedPocketChecks)
                    {
                        bool uec = UpdateExistingResearchCheck(check, opidcontext);

                        if (!uec)
                        {
                            string checkDate = "01/01/1900";

                            if (check.Date != null)
                            {
                                // Coerce from DateTime? to DateTime, then get date string
                                checkDate = ((DateTime)check.Date).ToString("MM/dd/yyyy");
                            }

                            RCheck rcheck = NewRCheck(check, checkDate);
                             
                            rchecks.Add(rcheck);
                        }

                        i += 1;
                        DailyHub.SendProgress("Updating Research Table...", i, checkCount);
                    }

                    opidcontext.RChecks.AddRange(rchecks);
                    opidcontext.ChangeTracker.DetectChanges();
                    opidcontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static bool ResearchTableIsEmpty()
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                var checks = opidcontext.RChecks;

                if (checks.Count() == 0) // Is the table empty for a restore operation?
                {
                    return true;
                }
            }

            return false;
        }

        public static void DeleteResearchTable()
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                opidcontext.RChecks.RemoveRange(opidcontext.RChecks);  // Remove all checks from table RChecks (the Research Table)
                opidcontext.SaveChanges();
                return;
            }
        }

        private static AncientCheck NewAncientCheck(CheckViewModel cvm, DateTime epoch)
        {
            return new AncientCheck
            {
                RecordID = cvm.RecordID,
                sRecordID = cvm.sRecordID,
                InterviewRecordID = cvm.InterviewRecordID,
                sInterviewRecordID = cvm.sInterviewRecordID,

                Num = cvm.Num,
                sNum = cvm.sNum,
                Name = cvm.Name,
                DOB = cvm.DOB,
                sDOB = cvm.DOB.ToString("MM/dd/yyyy"),
                Date = string.IsNullOrEmpty(cvm.Date) ? epoch : Convert.ToDateTime(cvm.Date),
                sDate = string.IsNullOrEmpty(cvm.Date) ? string.Empty : cvm.Date,
                Service = cvm.Service,
                Disposition = cvm.Disposition
            };
        }

        private static void RestoreAncientChecksTable(List<CheckViewModel> ancientChecks)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                // Using context.AddRange as described in: https://entityframework.net/improve-ef-add-performance
                opidcontext.Configuration.AutoDetectChangesEnabled = false;

                var checks = opidcontext.AncientChecks;
                List<AncientCheck> listAncientChecks = new List<AncientCheck>();

                int checkCount = ancientChecks.Count;
                int i = 0;

                if (checks.Count() == 0) // Is the table empty for rebuild?
                {
                    DateTime epoch = new DateTime(1900, 1, 1);

                    foreach (CheckViewModel cvm in ancientChecks)
                    {
                        try
                        {
                            AncientCheck ancientCheck = NewAncientCheck(cvm, epoch);

                            if (string.IsNullOrEmpty(ancientCheck.sDate))
                            {
                                ancientCheck.Date = null;
                            }

                            listAncientChecks.Add(ancientCheck);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }

                        i += 1;
                        DailyHub.SendProgress("Restore in progress...", i, checkCount);
                    }

                    checks.AddRange(listAncientChecks);
                    opidcontext.ChangeTracker.DetectChanges();
                    opidcontext.SaveChanges();
                    return;
                }
            }
        }

        public static void RestoreResearchTable(string rtFileName)
        {
            string pathToResearchTableFile = System.Web.HttpContext.Current.Request.MapPath(string.Format("~/Uploads/{0}", rtFileName));

            List<CheckViewModel> rchecks = MyExcelDataReader.GetCVMS(pathToResearchTableFile);

            RestoreRChecksTable(rchecks);
        }

        private static RCheck NewRCheck(CheckViewModel cvm, DateTime epoch)
        {
            return new RCheck
            {
                RecordID = cvm.RecordID,
                sRecordID = cvm.sRecordID,
                InterviewRecordID = cvm.InterviewRecordID,
                sInterviewRecordID = cvm.sInterviewRecordID,

                Num = cvm.Num,
                sNum = cvm.sNum,
                Name = cvm.Name,
                DOB = cvm.DOB,
                sDOB = cvm.DOB.ToString("MM/dd/yyyy"),
                Date = string.IsNullOrEmpty(cvm.Date) ? epoch : Convert.ToDateTime(cvm.Date),
                sDate = string.IsNullOrEmpty(cvm.Date) ? string.Empty : cvm.Date,
                Service = cvm.Service,
                Disposition = cvm.Disposition
            };
        }

        private static void RestoreRChecksTable(List<CheckViewModel> rChecks)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                // Using context.AddRange as described in: https://entityframework.net/improve-ef-add-performance
                opidcontext.Configuration.AutoDetectChangesEnabled = false;

                var checks = opidcontext.RChecks;
                List<RCheck> listRChecks = new List<RCheck>();

                int checkCount = rChecks.Count;
                int i = 0;

                if (checks.Count() == 0) // Is the table empty for rebuild?
                {
                    DateTime epoch = new DateTime(1900, 1, 1);

                    foreach (CheckViewModel cvm in rChecks)
                    {
                        try
                        {
                            RCheck rcheck = NewRCheck(cvm, epoch);
                            
                            if (string.IsNullOrEmpty(rcheck.sDate))
                            {
                                rcheck.Date = null;
                            }

                            listRChecks.Add(rcheck);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.Message);
                        }

                        i += 1;
                        DailyHub.SendProgress("Restore in progress...", i, checkCount);
                    }

                    checks.AddRange(listRChecks);
                    opidcontext.ChangeTracker.DetectChanges();
                    opidcontext.SaveChanges();
                    return;
                }
            }
        }

        public static List<DispositionRow> GetResearchRows(string uploadedFileName)
        {
            List<DispositionRow> resRows = MyExcelDataReader.GetResearchRows(uploadedFileName);
            return resRows;
        }

        public static List<TrackingRow> GetTrackingRows(string uploadedFileName)
        {
            List<TrackingRow> trackingRows = MyExcelDataReader.GetTrackingRows(uploadedFileName);
            return trackingRows;
        }

        private static RCheck NewRCheck(System.Data.DataRow dataRow, int checkNumber, string disposition)
        {
            string lastName = dataRow["Last Name"].ToString();
            string firstName = dataRow["First Name"].ToString();
            DateTime dob = Convert.ToDateTime(dataRow["Date of Birth"].ToString());

            return new RCheck
            {
                RecordID = Convert.ToInt32(dataRow["Record ID"].ToString()),
                sRecordID = dataRow["Record ID"].ToString(),
                InterviewRecordID = Convert.ToInt32(dataRow["Interview Record ID"].ToString()),
                sInterviewRecordID = dataRow["Interview Record ID"].ToString(),
                Num = checkNumber,
                sNum = checkNumber.ToString(),
                Name = string.Format("{0}, {1}", lastName, firstName),
                DOB = dob,
                sDOB = dataRow["Date of Birth"].ToString(),
                Date = Convert.ToDateTime(dataRow["OPID Interview Date"]),
                sDate = dataRow["OPID Interview Date"].ToString(),
                Service = dataRow["Requested Item"].ToString(),
                Disposition = disposition
            };
        }

        public static void SetDisposition(System.Data.DataRow dataRow, int checkNumber, string disposition)
        {
            string lastName = dataRow["Last Name"].ToString();
            string firstName = dataRow["First Name"].ToString();
            DateTime dob = Convert.ToDateTime(dataRow["Date of Birth"].ToString());

            try
            {
                using (OpidDailyDB opiddailycontext = new DataContexts.OpidDailyDB())
                {
                    AncientCheck ancientCheck = opiddailycontext.AncientChecks.Where(ac => ac.DOB == dob && ac.Name.ToUpper().StartsWith(lastName) && ac.Num == checkNumber).SingleOrDefault();

                    if (ancientCheck != null)
                    {
                        ancientCheck.Disposition = disposition;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    RCheck rcheck = opiddailycontext.RChecks.Where(rc => rc.DOB == dob && rc.Name.ToUpper().StartsWith(lastName) && rc.Num == checkNumber).SingleOrDefault();

                    if (rcheck != null)
                    {
                        rcheck.Disposition = disposition;
                        opiddailycontext.SaveChanges();
                        return;
                    }

                    // Add a new research check on the fly.
                    // This is the case where an OPID Daily Tracking File reissues or replaces a check which
                    // was not yet recorded as a research check.
                    rcheck = NewRCheck(dataRow, checkNumber, disposition);

                    opiddailycontext.RChecks.Add(rcheck);
                    opiddailycontext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("Error setting check dispositon for {0}, {1} -- Check number: {2}, Error: {3}", 
                    lastName, firstName, checkNumber, e.Message));
            }
        }
        
        private static string Successor(string sequencedItem)
        {   // Example: sequencedItem = "TID2"
            // Then:
            //    length = 3
            //    item = "TID"
            //    sequenceNumber = 2
            // Returns: "TID3"
            int length = sequencedItem.Length;
            string item = sequencedItem.Substring(0, length - 1);
            int sequenceNumber = Convert.ToInt32(sequencedItem.Substring(length - 1));

            return string.Format("{0}{1}", item, sequenceNumber + 1);
        }

        public static string SequencedRequestedItem(List<VisitViewModel> visits, string requestedItem)
        {
            string sequencedItem = string.Empty;

            foreach (VisitViewModel vvm in visits)
            {
                if (vvm.Item.StartsWith(requestedItem))
                {
                    sequencedItem = vvm.Item;
                }
            }

            if (string.IsNullOrEmpty(sequencedItem))
            {
                return requestedItem;
            }
            else if (sequencedItem.Equals(requestedItem))
            {
                // Example: if requestedItem = "TDL" and sequencedItem = "TDL"
                // then the next sequencedItem is TDL2.
                return string.Format("{0}2", sequencedItem);
            }

            return Successor(sequencedItem);
        }

        public static List<Check> GetExcelChecks(string uploadedFileName, string disposition)
        {
            if (uploadedFileName.Equals("unknown"))
            {
                // Return an emmpty list of checks.
                return new List<Check>();
            }

            List<Check> excelChecks = MyExcelDataReader.GetExcelChecks(uploadedFileName);

            foreach (Check check in excelChecks)
            {
                check.Disposition = disposition;
            }

            return excelChecks;
        }

        private static CheckViewModel NewCheckViewModel(Check check, DateTime checkDate, string disposition)
        {
            return new CheckViewModel
            {
                RecordID = check.RecordID,
                sRecordID = check.RecordID.ToString(),
                InterviewRecordID = check.InterviewRecordID,
                sInterviewRecordID = check.InterviewRecordID.ToString(),
                Name = check.Name,
                Num = check.Num,
                sNum = check.Num.ToString(),
                Date = ((DateTime)check.Date).ToShortDateString(),
                sDate = (check.Date == null ? "" : checkDate.ToString("MM/dd/yyyy")),
                Service = check.Service,
                Disposition = disposition
            };
        }

        public static void NewResolvedCheck(Check check, string disposition)
        {
            // PLB 1/23/2019 Added r.RecordID == check.RecordID.
            // This fixed the problem that Bill reported in an email dated 1/21/2019.
            CheckViewModel alreadyResolved = resolvedChecks.Where(r => (r.RecordID == check.RecordID && r.Num == check.Num)).FirstOrDefault();
            CheckViewModel cvm = null;
            DateTime checkDate = new DateTime(1900, 1, 1);

            if (check.Date != null)
            {
                checkDate = (DateTime)check.Date;
            }

            if (alreadyResolved == null)
            {
                cvm = NewCheckViewModel(check, checkDate, disposition);
                resolvedChecks.Add(cvm);
            }
        }

        private static Check NewCheck(Client client, Check echeck, PocketCheck pcheck, DateTime checkDate, string disposition)
        {
            // If it is not null, then echeck is an Excel Check created by MyExcelDataReader.GetExcelChecks.
            // In this case, echeck.InterviewRecordID will be set to the memo field of an Origen Bank check.
            // Copy this memo field into the InterviewRecordID field of a new pocket check.
            // This pocket check will be removed from the PocketChecks table and used to update the Research Table.
            // See method ResolvePocketChecks.
            return new Check
            {
                RecordID = pcheck.ClientId,
                InterviewRecordID = (echeck == null ? 0 : echeck.InterviewRecordID),
                DOB = client.DOB,
                Name = pcheck.Name,
                Num = pcheck.Num,
                Date = checkDate,
                Service = pcheck.Item,
                Disposition = disposition
            };
        }

        public static void NewResolvedPocketCheck(Check echeck, PocketCheck pcheck, string disposition)
        {
            Check alreadyResolved = resolvedPocketChecks.Where(r => (r.RecordID == pcheck.ClientId && r.Num == pcheck.Num)).FirstOrDefault();
            DateTime checkDate = new DateTime(1900, 1, 1);

            if (pcheck.Date != null)
            {
                checkDate = (DateTime)pcheck.Date;
            }

            if (alreadyResolved == null)
            {
                using (OpidDailyDB opidcontext = new DataContexts.OpidDailyDB())
                {
                    Client client = opidcontext.Clients.Find(pcheck.ClientId);

                    if (client != null)
                    {
                        Check check = NewCheck(client, echeck, pcheck, checkDate, disposition);
                        resolvedPocketChecks.Add(check);
                    }
                }
            }
        }

        public static void MarkMistakenlyResolvedChecks(List<Check> mistakenlyResolved)
        {
            using (OpidDailyDB opidcontext = new OpidDailyDB())
            {
                foreach (Check mr in mistakenlyResolved)
                {
                    List<RCheck> rchecks = opidcontext.RChecks.Where(u => u.Num == mr.Num).ToList();

                    foreach (RCheck rcheck in rchecks)
                    {
                        rcheck.Disposition = "Mistakenly Resolved";
                    }
                }

                opidcontext.SaveChanges();
            }
        }

        public static bool IsNewMistakenlyResolved(Check check)
        {
            if (mistakenlyResolved.Contains(check.Num))
            {
                return false;
            }
            else
            {
                mistakenlyResolved.Add(check.Num);
                return check.Disposition.Equals("Mistakenly Resolved");
            }
        }
    }
}