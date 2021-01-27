using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class DemographicInfoViewModel
    {
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Display(Name = "Birth Name")]
        public string BirthName { get; set; }

        [Display(Name = "AKA")]
        public string AKA { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Date of Birth")]
        public string DOB { get; set; }

        [Display(Name = "Birth City")]
        public string BirthCity { get; set; }

        [Display(Name = "Birth State")]
        public string BirthState { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Current Address")]
        public string CurrentAddress { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Zip")]
        public string Zip { get; set; }

        public string Incarceration { get; set; }

        public string HousingStatus { get; set; }

        public string USCitizen { get; set; }

        public string Gender { get; set; }

        public string Ethnicity { get; set; }

        public string Race { get; set; }

        public string MilitaryVeteran { get; set; }

        public string DischargeStatus { get; set; }

        public string Disabled { get; set; }
    }
}