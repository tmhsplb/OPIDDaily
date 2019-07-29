using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpidDailyEntities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        public DateTime ServiceDate { get; set; }

        public string ServiceTicket { get; set; }

        public int WaitTime { get; set; }

        public string Stage { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }

        public DateTime DOB { get; set; }

        public int Age { get; set; }
        
        public string Notes { get; set; }

        public DateTime Screened { get; set; }

        public DateTime CheckedIn { get; set; }

        public DateTime Interviewing { get; set; }

        public DateTime Interviewed { get; set; }

        public DateTime BackOffice { get; set; }

        public DateTime Done { get; set; }

        public bool EXP { get; set; }

        public bool PND { get; set; }

        public bool XID { get; set; }

        public bool XBC { get; set; }

        public bool Active { get; set; }

        public ICollection<Visit> Visits { get; set; }
    }
}
