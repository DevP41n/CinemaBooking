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
    
    public partial class dao_dien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dao_dien()
        {
            this.phims = new HashSet<phim>();
        }
    
        public int id { get; set; }
        public string ho_ten { get; set; }
        public Nullable<System.DateTime> ngay_sinh { get; set; }
        public string mo_ta { get; set; }
        public string quoc_tich { get; set; }
        public Nullable<int> chieu_cao { get; set; }
        public string chi_tiet { get; set; }
        public string anh { get; set; }
        public string slug { get; set; }
        public string phim_da_tham_gia { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<phim> phims { get; set; }
    }
}
