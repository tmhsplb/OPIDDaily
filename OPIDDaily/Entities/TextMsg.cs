using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpidDailyEntities
{
    public class TextMsg
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Msg { get; set; }
    }
}