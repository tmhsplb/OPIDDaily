using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Models
{
    public class RequestedServicesViewModel
    {
        public string XID { get; set; }

        public string XBC { get; set; }

        [Display(Name = "Client will return with requested documents")]
        public bool NeedsDocs { get; set; }

        [Display(Name = "Client has returned with requested documents")]
        public bool HasDocs { get; set; }

        [Required(ErrorMessage = "Please select an agency from the list.")]
        public string Agency { get; set; }

        public SelectList Agencies { get; set; }
 
        /*
        [Display(Name = "Use Birth Name?")]
        public bool UseBirthName { get; set; }
        */

        [Display(Name = "Texas BC")]
        public bool BC { get; set; }

        public string BCNotes { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool BCEligible { get; set; }
        */

        [Display(Name = "Harris County Clerk")]
        public bool HCC { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedBC { get; set; }

        [Display(Name = "MBVD")]
        public bool MBVD { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedMBVD { get; set; }

        public string MBVDNotes { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool MBVDEligible { get; set; }
        */

        public string State { get; set; }

        public string TIDNotes { get; set; }

        [Display(Name = "New/Dup ID")]
        public bool NewTID { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedNewTID { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool NewTIDEligible { get; set; }
        */

        [Display(Name = "Replacement ID")]
        public bool ReplacementTID { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedReplacementTID { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool ReplacementTIDEligible { get; set; }
        */

        [Display(Name = "New/Dup DL")]
        public bool NewTDL { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool NewTDLEligible { get; set; }
        */

        public string TDLNotes { get; set; }

        [Display(Name = "Replacement DL")]
        public bool ReplacementTDL { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedNewTDL { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedReplacementTDL { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool ReplacementTDLEligible { get; set; }
        */

        [Display(Name = "Numident")]
        public bool Numident { get; set; }

        // Supporting Documents
        [Display(Name = "Birth Certificate")]
        public bool SDBC { get; set; }

        [Display(Name = "SS Card")]
        public bool SDSSC { get; set; }

        [Display(Name = "Texas ID")]
        public bool SDTID { get; set; }

        [Display(Name = "Texas DL")]
        public bool SDTDL { get; set; }

        [Display(Name = "Other State ID")]
        public bool SDOSID { get; set; }

        [Display(Name = "Other State DL")]
        public bool SDOSDL { get; set; }

        [Display(Name = "Marriage License")]
        public bool SDML { get; set; }

        [Display(Name = "Divorce Decree")]
        public bool SDDD { get; set; }

        [Display(Name = "Service Letter")]
        public bool SDSL { get; set; }

        [Display(Name = "DD214")]
        public bool SDDD214 { get; set; }

        [Display(Name = "Gold Card")]
        public bool SDGC { get; set; }

        [Display(Name = "EBT Card")]
        public bool SDEBT { get; set; }

        [Display(Name = "HOTID")]
        public bool SDHOTID { get; set; }

        [Display(Name = "School Records")]
        public bool SDSchoolRecords { get; set; }

        [Display(Name = "Passport")]
        public bool SDPassport { get; set; }

        [Display(Name = "Job Offer Letter")]
        public bool SDJobOffer { get; set; }

        [Display(Name= "Other")]
        public bool SDOther { get; set; }

        public string SDOthersd { get; set; }
    }
}