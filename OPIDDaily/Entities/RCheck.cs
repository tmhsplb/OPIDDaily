using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpidDailyEntities
{
    public class RCheck
    {
        public int Id { get; set; }
        public int RecordID { get; set; }
        public string sRecordID { get; set; }
        public int InterviewRecordID { get; set; }
        public string sInterviewRecordID { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string sDOB { get; set; }
        public int Num { get; set; }
        public string sNum { get; set; }
        public DateTime? Date { get; set; }
        public string sDate { get; set; }
        public string Service { get; set; }
        public int Amount { get; set; }
        public string Disposition { get; set; }
        public string Notes { get; set; }
    }
}