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

        /*
        // Requested services
        public bool BC { get; set; }

        public bool HCC { get; set; }

        public bool MBVD { get; set; }

        public bool NewTID { get; set; }

        public bool ReplacementTID { get; set; }

        public bool NewTDL { get; set; }

        public bool ReplacementTDL { get; set; }

        public bool Numident { get; set; }

        // Supporting documents
        public bool SDBC { get; set; }

        public bool SDSCC { get; set; }

        public bool SDTID { get; set; }

        public bool SDTDL { get; set; }

        public bool SDOSID { get; set; }

        public bool SDOSDL { get; set; }

        public bool SDML { get; set; }

        public bool SDDD { get; set; }

        public bool SDSL { get; set; }

        public bool SDDD214 { get; set; }

        public bool SDEBT { get; set; }

        public bool SDHOTID { get; set; }

        public bool SDSchoolRecords { get; set; }

        public bool SDPassport { get; set; }

        public bool SDJobOffer { get; set; }

        public bool SDOther { get; set; }

        public string SDOthersd { get; set; }
        */

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
