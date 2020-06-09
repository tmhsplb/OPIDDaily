using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpidDailyEntities
{
    public class PocketCheck
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string Item { get; set; }
        public int Num { get; set; }
        public string Disposition { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
    }
}