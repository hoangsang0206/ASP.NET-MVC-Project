using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace STech_Web.Models
{
    public class WareHouse
    {
        [Key]
        public string ProductID {  get; set; }
        public Nullable<int> Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}