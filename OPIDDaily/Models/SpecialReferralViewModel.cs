using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class SpecialReferralViewModel
    {
        [Display(Name = "Client First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Client Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Client Last Name")]
        public string LastName { get; set; }

        [Display(Name = "To secure an employment opportunity (proof of job offer attached).")]
        public bool EMP { get; set; }

        [Display(Name = "To replace a stolen ID (police report attached).")]
        public bool SID { get; set; }

        [Display(Name = "Client is a regular participant at Holy Ground.")]
        public bool HGP { get; set; }

        [Display(Name = "Client was referred by a New Potential Partner Agency.")]
        public bool NPP { get; set; }

        public string Agency { get; set; }

        public string AgencyContact { get; set; }

        [Display(Name = "Social Security Card")]
        public bool SSCARD { get; set; }

        [Display(Name = "Voter's Registration")]
        public bool VREG { get; set; }

        [Display(Name = "Marriage License")]
        public bool MLIC { get; set; }

        [Display(Name = "Divorce Decree")]
        public bool DDEC { get; set; }

        [Display(Name = "DD-214 (Vets)")]
        public bool DD214 { get; set; }

        [Display(Name = "School Records")]
        public bool SREC { get; set; }

        [Display(Name = "Other _________________________________")]
        public bool Other { get; set; }

        [Display(Name = "TX Photo ID")]
        public bool TID { get; set; }

        [Display(Name = "New")]
        public bool NTID { get; set; }

        [Display(Name = "Replacement")]
        public bool RTID { get; set; }

        [Display(Name = "TX Driver's License")]
        public bool TDL { get; set; }

        [Display(Name = "New")]
        public bool NTDL { get; set; }

        [Display(Name = "Replacement")]
        public bool RTDL { get; set; }

        [Display(Name = "TX Birth Certificate")]
        public bool BC { get; set; }

        [Display(Name = "Out of State Birth Certificate")]
        public bool MBVD { get; set; }

        public string SpecialInstructions { get; set; }
    }
}