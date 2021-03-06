using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpidDailyEntities
{
    public class MBVD
    {
        [Key]
        public int Id { get; set; }
        public int MBVDId { get; set; }
        public string MBVDName { get; set; }
        public bool IsActive { get; set; }
    }
}