using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace STech_Web.Models
{
    public class Countdown
    {
        public string startDate {  get; set; }
        public string endDate { get; set; }

        public Countdown() { }

        public Countdown(string startDate, string endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

    }
}