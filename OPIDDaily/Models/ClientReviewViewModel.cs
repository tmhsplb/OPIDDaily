using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OPIDDaily.Models
{
    public class ClientReviewViewModel
    {
        public int Id { get; set; }

        public string ServiceTicket { get; set; }

        public string Expiry { get; set; }

        public string Stage { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string DOB { get; set; }

        public string Active { get; set; }
 
        public DateTime CheckedIn { get; set; }
 
        public string Notes { get; set; }
    }
}
