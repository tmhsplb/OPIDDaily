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

        public DateTime ReferralDate { get; set; }

        public DateTime AppearanceDate { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }
        
        public string Notes { get; set; }
    }
}
