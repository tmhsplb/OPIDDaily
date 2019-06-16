using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpidDaily.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        public string ReferralDate { get; set; }

        public string AppearanceDate { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }
        
        public string Notes { get; set; }
    }
}
