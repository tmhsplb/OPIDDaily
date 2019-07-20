using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OpidDailyEntities
{
    public class Visit
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Item { get; set; }

        public string Check { get; set; }

        public string Status { get; set; }

        public string Notes { get; set; }
    }
}
