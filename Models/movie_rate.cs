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
    
    public partial class movie_rate
    {
        public int id { get; set; }
        public Nullable<int> movie_id { get; set; }
        public Nullable<int> khachhang_id { get; set; }
        public Nullable<double> rate { get; set; }
        public string comment { get; set; }
        public string ten_khachhang { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
    }
}
