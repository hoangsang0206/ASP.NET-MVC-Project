﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DatabaseSTechEntities : DbContext
    {
        public DatabaseSTechEntities()
            : base("name=DatabaseSTechEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Megamenu> Megamenus { get; set; }
        public virtual DbSet<MegamenuItem> MegamenuItems { get; set; }
        public virtual DbSet<ProductImgDetail> ProductImgDetails { get; set; }
        public virtual DbSet<ProductOutStanding> ProductOutStandings { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductsBackup> ProductsBackups { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SidebarMenuNav> SidebarMenuNavs { get; set; }
        public virtual DbSet<Specification> Specifications { get; set; }
        public virtual DbSet<WareHouse> WareHouses { get; set; }
        public virtual DbSet<Banner1> Banner1 { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
    }
}
