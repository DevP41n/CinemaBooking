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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class khach_hang
    {
        CinemaBookingEntities db = new CinemaBookingEntities();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public int id { get; set; }
        [Display(Name = "Họ Tên")]
        public string ho_ten { get; set; }
        public string username { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        public string password { get; set; }
        [NotMapped]
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("password")]
        public string confirmpassword { get; set; }
        [Display(Name = "Số điện thoại")]
        public string sdt { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string email { get; set; }
        [Display(Name = "Địa Chỉ")]
        public string dia_chi { get; set; }
        public Nullable<bool> gioi_tinh { get; set; }
        public Nullable<System.DateTime> ngay_sinh { get; set; }
        public string cmnd { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public Nullable<System.DateTime> update_at { get; set; }

        //Facebook
        public long InsertForFacebook(khach_hang KH)
        {
            var user = db.khach_hang.SingleOrDefault(x => x.email == KH.email);
            if (user == null)
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.khach_hang.Add(KH);
                db.SaveChanges();
                return KH.id;
            }
            else
            {
                return user.id;
            }

        }
        public long InsertForgot(khach_hang KH)
        {
            var user = db.khach_hang.SingleOrDefault(x => x.email == KH.email);
            if (user != null)
            {
                return user.id;
            }
            return user.id;
        }
    }
}