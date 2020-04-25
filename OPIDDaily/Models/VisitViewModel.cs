using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class VisitViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
 
        public string Item { get; set; }

        public string Check { get; set; }

        public string Status { get; set; }

        public string Sender { get; set; }
  
        public string Notes { get; set; }
    }
}