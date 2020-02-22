using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace OPIDDaily.Models
{
    public class FileViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }

        public string Year { get; set; }
    }
}