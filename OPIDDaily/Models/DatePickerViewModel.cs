﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class DatePickerViewModel
    {
        [Required]
        [Display(Name = "Date")]
        public string datepicker { get; set; }
    }
}