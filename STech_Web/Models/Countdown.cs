using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class Countdown
    {
        public string endDate { get; set; }

        public Countdown() { }

        public Countdown(string endDate)
        {
            this.endDate = endDate;
        }

    }
}