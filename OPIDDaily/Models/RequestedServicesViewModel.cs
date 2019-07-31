using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class RequestedServicesViewModel
    {
        [Display(Name = "Texas Birth Certificate")]
        public bool BC { get; set; }

        [Display(Name = "Harris County Clerk")]
        public bool HCC { get; set;}

        [Display(Name= "MBVD")]
        public bool MBVD { get; set; }

        public string State { get; set; }

        [Display(Name = "New/Duplicate ID")]
        public bool NewTID { get; set; }

        [Display(Name = "Replacement ID")]
        public bool ReplacementTID { get; set; }

        [Display(Name = "New/Duplicate DL")]
        public bool NewTDL { get; set; }

        [Display(Name = "Replacement DL")]
        public bool ReplacementTDL { get; set; }

        [Display(Name = "Numident")]
        public bool Numident { get; set; }
    }
}