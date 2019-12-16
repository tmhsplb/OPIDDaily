using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpidDaily.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        public string ServiceDate { get; set; }

        public string Expiry { get; set; }

        public string ServiceTicket { get; set; }

        public int WaitTime { get; set; }

        public string Stage { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }

        public string DOB { get; set; }

        public int Age { get; set; }

        public string EXP { get; set; }

        public string PND { get; set; }

        public string XID { get; set; }

        public string XBC { get; set; }
        
        public string History { get; set; }

        public DateTime Screened { get; set; }

        public DateTime CheckedIn { get; set; }

        public DateTime Interviewing { get; set; }

        public DateTime Interviewed { get; set; }

        public DateTime BackOffice { get; set; }

        public DateTime Done { get; set; }
                
        public string Notes { get; set; }
    }
}
