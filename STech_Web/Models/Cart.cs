//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace STech_Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cart
    {
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public string CustomerID { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
