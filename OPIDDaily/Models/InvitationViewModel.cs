using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class InvitationViewModel
    {
        public int Id { get; set; }

        public string Extended { get; set; }

        public string Accepted { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }
    }
}