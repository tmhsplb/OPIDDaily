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

        public bool Conversation { get; set; }

        public bool HeadOfHousehold { get; set; }

        public int HH { get; set; }

        public DateTime DOB { get; set; }

        public int Age { get; set; }

        public string AKA { get; set; }
 
        public string Email { get; set; }

        public string BirthCity { get; set; }

        public string BirthState { get; set; }

        public string Phone { get; set; }

        public string CurrentAddress { get; set; }

        public string City { get; set; }

        public string Staat { get; set; }

        public string Zip { get; set; }

        public string Msgs { get; set; }

        public string Notes { get; set; }

        public DateTime Screened { get; set; }

        public DateTime CheckedIn { get; set; }

        public DateTime Interviewing { get; set; }

        public DateTime Interviewed { get; set; }

        public DateTime BackOffice { get; set; }

        public DateTime Done { get; set; }

        // Requested services
        public bool BC { get; set; }

        public bool HCC { get; set; }

        public bool MBVD { get; set; }

        public string State { get; set; }

        public bool NewTID { get; set; }

        public bool ReplacementTID { get; set; }

        public bool NewTDL { get; set; }

        public bool ReplacementTDL { get; set; }

        public bool Numident { get; set; }

        public string RequestedDocument { get; set; }

        // Supporting documents
        public bool SDBC { get; set; }

        public bool SDSSC { get; set; }

        public bool SDTID { get; set; }

        public bool SDTDL { get; set; }

        public bool SDTDCJ { get; set; }

        public bool SDVREG { get; set; }

        public bool SDML { get; set; }

        public bool SDDD { get; set; }

        public bool SDSL { get; set; }

        public bool SDDD214 { get; set; }

        public bool SDGC { get; set; }

        public bool SDEBT { get; set; }

        public bool SDHOTID { get; set; }

        public bool SDSchoolRecords { get; set; }

        public bool SDPassport { get; set; }

        public bool SDJobOffer { get; set; }

        public bool SDOther { get; set; }

        public string SDOthersd { get; set; }

        public string Incarceration { get; set; }

        public string HousingStatus { get; set; }

        public string USCitizen { get; set; }

        public string Gender { get; set; }

        public string Ethnicity { get; set; }
 
        public string Race { get; set; }

        public string MilitaryVeteran { get; set; }

        public string DischargeStatus { get; set; }

        public string Disabled { get; set; }

        public bool PND { get; set; }

        public bool LCK { get; set; }

        public bool XID { get; set; }

        public bool XBC { get; set; }

        public int AgencyId { get; set; }

        public string AgencyName { get; set; }

        public DateTime Expiry { get; set; }

        public bool Active { get; set; }

        public ICollection<TextMsg> TextMsgs { get; set; }

      
    }
}
