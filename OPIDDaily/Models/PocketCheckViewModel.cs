using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class PocketCheckViewModel
    {
        public int Id { get; set; }

        public string AgencyName { get; set; }

        public string Name { get; set; }

        public string HeadOfHousehold { get; set; }

        public DateTime Date { get; set; }

        public string Item { get; set; }

        public int Check { get; set; }

        public string Status { get; set; }
  
        public string Notes { get; set; }
    }
}