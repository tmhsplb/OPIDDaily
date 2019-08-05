using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class RequestedServicesViewModel
    {
        public string Agency { get; set; }

        [Display(Name = "Texas BC")]
        public bool BC { get; set; }

        [Display(Name = "Eligible?")]
        public bool BCEligible { get; set; }

        [Display(Name = "HCC")]
        public bool HCC { get; set;}

        [Display(Name = "MBVD")]
        public bool MBVD { get; set; }

        [Display(Name = "Eligible?")]
        public bool MBVDEligible { get; set; }

        public string State { get; set; }

        [Display(Name = "New/Dup ID")]
        public bool NewTID { get; set; }

        [Display(Name = "Eligible?")]
        public bool NewTIDEligible { get; set; }

        [Display(Name = "Replcmnt ID")]
        public bool ReplacementTID { get; set; }

        [Display(Name = "Eligible?")]
        public bool ReplacementTIDEligible { get; set; }

        [Display(Name = "Eligible?")]
        public bool NewTDLEligible { get; set; }
        [Display(Name = "New/Dup DL")]
        public bool NewTDL { get; set; }

        [Display(Name = "Eligible?")]
        public bool ReplacementTDLEligible { get; set; }

        [Display(Name = "Replcmnt DL")]
        public bool ReplacementTDL { get; set; }

        [Display(Name = "Numident")]
        public bool Numident { get; set; }
    }
}