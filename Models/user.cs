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
    
    public partial class user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public user()
        {
            this.ve_ban = new HashSet<ve_ban>();
        }
    
        public int id { get; set; }
        public string ho_ten { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public Nullable<int> role { get; set; }
        public string sdt { get; set; }
        public string email { get; set; }
        public string dia_chi { get; set; }
        public Nullable<bool> gioi_tinh { get; set; }
        public Nullable<System.DateTime> ngay_sinh { get; set; }
        public Nullable<System.DateTime> ngay_vao_lam { get; set; }
        public string cmnd { get; set; }
        public Nullable<bool> dang_lam { get; set; }
        public string create_by { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public string update_by { get; set; }
        public Nullable<System.DateTime> update_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ve_ban> ve_ban { get; set; }
    }
}
