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
    
    public partial class ProductContent
    {
        public int Id { get; set; }
        public string ProductID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentImg { get; set; }
        public string ContentVideo { get; set; }
    
        public virtual Product Product { get; set; }
    }
}