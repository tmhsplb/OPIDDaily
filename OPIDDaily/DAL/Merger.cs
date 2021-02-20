using OPIDDaily.Models;
using OPIDDaily.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace OPIDDaily.DAL
{
    public class Merger
    {
        public static void PerformMerge(string uploadedFile, string fileType)
        {
            switch (fileType)
            {
                case "OPIDDaily":
                case "BoundedResearchTableFile":
                    UpdateResearchTableFromOPIDFile(uploadedFile);
                    break;

                case "OPIDDailyTracking":
                    UpdateResearchTableFromOPIDTrackingFileCrossLoad(uploadedFile);
                    break;

                case "NewClients":
                    UpdateClientsTableFromFile(uploadedFile);
                    break;

                case "AncientChecksFile":
                    UpdateAncientChecksTableFromFile(uploadedFile);
                    break;

                case "VoidedChecks":
                    UpdateResearchTableFromOrigenBankFile(uploadedFile, "Voided");
                    break;

                case "ClearedChecks":
                    UpdateResearchTableFromOrigenBankFile(uploadedFile, "Cleared");
                    break;

                case "ReresolvedChecks":
                    ProcessMistakenlyResolvedChecks(uploadedFile);
                    break;

                case "ReresolvedVoidedChecks":
                    ReResolveResearchChecks(uploadedFile, "Voided");
                    break;

                case "ReresolvedClearedChecks":
                    ReResolveResearchChecks(uploadedFile, "Cleared");
                    break;

                default:
                    break;
            }
        }

        private static void UpdateClientsTableFromFile(string uploadedFile)
        {
            List<ClientRow> newClients = Clients.GetNewClients(uploadedFile);
            Clients.AddNewClients(newClients);
        }
        
        public static void UpdateResearchTableFromOPIDFile(string uploadedFile)
        {
            List<DispositionRow> researchRows = CheckManager.GetResearchRows(uploadedFile);

            // Handle incidental checks before persisting unmatched checks.
            // This way an Interview Research file cannot add to the set
            // of resolved checks by mistake.
            // For example, the Interview Research File may contain both
            //    Estes, Jason  TID = 74726, TID Disposition = Voided/Replaced
            //    Justice, Mark TID = 74726, TID Disposition = ?
            // In this case, check number 74726 was mistakenly assigned to both
            // the TID for Jason Estes and the TID for Mark Justice.
            // If incidental checks are handled after unmatched checks are persisted,
            // then the check for Jason Estes will resolve the check for Mark Justice.
            // We don't want this to happen! Most likely, the check number 74726
            // for Mark Justice was a typo.
            // PLB 12/14/2018 CheckManager.HandleIncidentalChecks(researchRows);
            CheckManager.RebuildResearchChecksTable(researchRows);
            //  PLB 12/14/2018 Don't call RemoveTypoChecks
            // CheckManager.RemoveTypoChecks();
        }

        public static void UpdateResearchTableFromOPIDTrackingFileCrossLoad(string uploadedFile)
        {
            List<TrackingRow> trackingRows = CheckManager.GetTrackingRows(uploadedFile);

            CheckManager.CrossLoadTrackingChecks(trackingRows);
        }

        public static void UpdateAncientChecksTableFromFile(string uploadedFile)
        {
            List<DispositionRow> researchRows = CheckManager.GetResearchRows(uploadedFile);

            // Handle incidental checks before persisting unmatched checks.
            // This way an Interview Research file cannot add to the set
            // of resolved checks by mistake.
            // For example, the Interview Research File may contain both
            //    Estes, Jason  TID = 74726, TID Disposition = Voided/Replaced
            //    Justice, Mark TID = 74726, TID Disposition = ?
            // In this case, check number 74726 was mistakenly assigned to both
            // the TID for Jason Estes and the TID for Mark Justice.
            // If incidental checks are handled after unmatched checks are persisted,
            // then the check for Jason Estes will resolve the check for Mark Justice.
            // We don't want this to happen! Most likely, the check number 74726
            // for Mark Justice was a typo.
            // PLB 12/14/2018 CheckManager.HandleIncidentalChecks(researchRows);
            CheckManager.RebuildAncientChecksTable(researchRows);
            //  PLB 12/14/2018 Don't call RemoveTypoChecks
            // CheckManager.RemoveTypoChecks();
        }

        public static void UpdateResearchTableFromOrigenBankFile(string uploadedFile, string disposition)
        {
            CheckManager.Init();

            List<Check> excelChecks = CheckManager.GetExcelChecks(uploadedFile, disposition);
            List<Check> researchChecks = CheckManager.GetResearchChecks();
           
            List<CheckViewModel> resolvedChecks = CheckManager.GetResolvedChecks(excelChecks, disposition, researchChecks);

            CheckManager.ResolveResearchChecks(resolvedChecks);
            
            // Resolving pocket checks is simply a matter of deleting any pocket check
            // whose corresponding research check has been resolved by the preceding call.
            CheckManager.ResolvePocketChecks(resolvedChecks);
        }

        private static void ProcessMistakenlyResolvedChecks(string uploadedFile)
        {
            List<Check> mistakenlyResolved = CheckManager.GetExcelChecks(uploadedFile, "DontCare");
            CheckManager.MarkMistakenlyResolvedChecks(mistakenlyResolved);
        }

        private static void ReResolveResearchChecks(string uploadedFile, string disposition)
        {
            CheckManager.Init();

            List<Check> researchChecks = CheckManager.GetResearchChecks();
            List<Check> excelChecks = CheckManager.GetExcelChecks(uploadedFile, disposition);

            CheckManager.DetermineReResolvedChecks(excelChecks, researchChecks);

            // Remove the set of resolved checks determined above from the Research Table. 
            // CheckManager.RemoveReResolvedChecks();
        }
    }
}