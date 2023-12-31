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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public string OrderID { get; set; }
        public string CustomerID { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentStatus { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> DeliveryFee { get; set; }
        public decimal TotalPaymentAmout { get; set; }
        public string ShipMethod { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string ShipAddress { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
