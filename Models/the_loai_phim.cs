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
    
    public partial class the_loai_phim
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public the_loai_phim()
        {
            this.list_phim_theloai = new HashSet<list_phim_theloai>();
        }
    
        public int id { get; set; }
        public string ten_the_loai { get; set; }
        public string slug { get; set; }
        public string create_by { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public string update_by { get; set; }
        public Nullable<System.DateTime> update_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<list_phim_theloai> list_phim_theloai { get; set; }
    }
}
