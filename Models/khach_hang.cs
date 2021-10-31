//------------------------------------------------------------------------------
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
    using System.Linq;

    public partial class khach_hang
    {
        CinemaBookingEntities db = new CinemaBookingEntities();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public int id { get; set; }
        public string ho_ten { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string sdt { get; set; }
        public string email { get; set; }
        public string dia_chi { get; set; }
        public Nullable<bool> gioi_tinh { get; set; }
        public Nullable<System.DateTime> ngay_sinh { get; set; }
        public string cmnd { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public Nullable<System.DateTime> update_at { get; set; }
        public string confirmpassword { get; set; }

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
