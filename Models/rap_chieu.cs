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
    using System.ComponentModel.DataAnnotations;

    public partial class rap_chieu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rap_chieu()
        {
            this.phong_chieu = new HashSet<phong_chieu>();
        }
    
        public int id { get; set; }
        [Required]
        public string ten_rap { get; set; }
        [Required]
        public string dia_chi { get; set; }
        public Nullable<int> status { get; set; }
        public string create_by { get; set; }
        public Nullable<System.DateTime> create_at { get; set; }
        public string update_by { get; set; }
        public Nullable<System.DateTime> update_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<phong_chieu> phong_chieu { get; set; }
    }
}
