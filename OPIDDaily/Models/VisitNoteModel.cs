using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class VisitNoteModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string Note { get; set; }
    }
}