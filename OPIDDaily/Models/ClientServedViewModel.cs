using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class ClientServedViewModel
    {
        public string ServiceTicket { get; set; }

        public string Expiry { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string DOB { get; set; }

        public string Notes { get; set; }
    }
}