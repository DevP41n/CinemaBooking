﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaBooking.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CinemaBookingEntities : DbContext
    {
        public CinemaBookingEntities()
            : base("name=CinemaBookingEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<dao_dien> dao_dien { get; set; }
        public virtual DbSet<dien_vien> dien_vien { get; set; }
        public virtual DbSet<ghe_ngoi> ghe_ngoi { get; set; }
        public virtual DbSet<gia_ve> gia_ve { get; set; }
        public virtual DbSet<khach_hang> khach_hang { get; set; }
        public virtual DbSet<lien_he> lien_he { get; set; }
        public virtual DbSet<list_phim_dienvien> list_phim_dienvien { get; set; }
        public virtual DbSet<loai_ghe> loai_ghe { get; set; }
        public virtual DbSet<movie_rate> movie_rate { get; set; }
        public virtual DbSet<phim> phims { get; set; }
        public virtual DbSet<phong_chieu> phong_chieu { get; set; }
        public virtual DbSet<su_kien> su_kien { get; set; }
        public virtual DbSet<suat_chieu> suat_chieu { get; set; }
        public virtual DbSet<the_loai_phim> the_loai_phim { get; set; }
        public virtual DbSet<thong_tin_cong_ty> thong_tin_cong_ty { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<ve_ban> ve_ban { get; set; }
    }
}
