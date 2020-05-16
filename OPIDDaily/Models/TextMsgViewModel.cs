using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class TextMsgViewModel
    {

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string From { get; set; }

        public string To { get; set; }
        
        public string Msg { get; set; }
    }
}