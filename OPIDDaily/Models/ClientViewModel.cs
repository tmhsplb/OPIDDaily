using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OPIDDaily.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        public DateTime ServiceDate { get; set; }   

        public DateTime Expiry { get; set; }    

        public string ServiceTicket { get; set; }

        public int WaitTime { get; set; }

        public string Stage { get; set; }
        
        public string Conversation { get; set; }

        public string HeadOfHousehold { get; set; }
        
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }

        public DateTime DOB { get; set; }   

        public string sDOB { get; set; }

        public int Age { get; set; }
 
        public string ACK { get; set; }

        public string LCK { get; set; }

        public string XID { get; set; }

        public string XBC { get; set; }
        
        public string History { get; set; }

        public string AgencyName { get; set; }

        public DateTime Screened { get; set; }

        public DateTime CheckedIn { get; set; }

        public DateTime Interviewing { get; set; }

        public DateTime Interviewed { get; set; }

        public DateTime BackOffice { get; set; }

        public DateTime Done { get; set; }

        public string MSG { get; set; }

        public string Msgs { get; set; }
                
        public string Notes { get; set; }
    }
}
