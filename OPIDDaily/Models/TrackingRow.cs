using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class TrackingRow
    {
        public int RecordID { get; set; }

        public int InterviewRecordID { get; set; }

        public DateTime Date { get; set; }

        public string Lname { get; set; }
        public string Fname { get; set; }

        public DateTime DOB { get; set; }

        public string RequestedItem { get; set; }

        public DateTime? OrderDate { get; set; }

        public int CheckNum { get; set; }
        public string CheckDisposition { get; set; }

        public string Scammed { get; set; }

        public bool Reissued { get; set; }

        public string ReissueReason { get; set; }

    }
}

