using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CinemaBooking.Models
{
    public class Facebook
    {
        CinemaBookingEntities db = new CinemaBookingEntities();
        public long InsertForFacebook(khach_hang entity)
        {
            var user = db.khach_hang.SingleOrDefault(x => x.username == entity.ho_ten);
            if (user == null)
            {
                db.khach_hang.Add(entity);
                db.SaveChanges();
                return entity.id;
            }
            else
            {
                return user.id;
            }

        }
    }
}