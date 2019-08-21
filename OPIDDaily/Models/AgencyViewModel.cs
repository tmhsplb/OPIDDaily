using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class AgencyViewModel
    {
        public int Id { get; set; }
        public int AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string ContactPerson { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IsActive { get; set; }
    }
}