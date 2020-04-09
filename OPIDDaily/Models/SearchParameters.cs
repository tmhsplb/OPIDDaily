using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class SearchParameters
    {
        public string sidx { get; set; }

        public string sord { get; set; }

        public bool _search { get; set; }

        public string AgencyName { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string BirthName { get; set; }

      //  public string searchField { get; set; }

      //  public string searchOper { get; set; }

      //  public string searchString { get; set; }
    }
}