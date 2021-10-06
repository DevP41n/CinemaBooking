using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class MovieController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Movie
        //Chi tiết phim
        public ActionResult MovieDetail(/*int? id*/)
        {
            return View(/*db.phims.SingleOrDefault(p => p.id.Equals(id))*/);
        }
        //Phim đang chiếu
        public ActionResult NowShowing()
        {
            return View();
        }
        //Phim sắp chiếu
        public ActionResult ComingSoon()
        {
            return View();
        }

    }
}