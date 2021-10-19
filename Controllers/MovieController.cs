using CinemaBooking.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class MovieController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Movie
        //Chi tiết phim
        public ActionResult MovieDetail(String id)
        {
            var movie = db.phims
                        .Where(m => m.slug == id && m.status == 1).First();
            return View(movie);
        }
        //Phim đang chiếu
        public ActionResult NowShowing()
        {
            return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 1));
        }
        //Phim sắp chiếu
        public ActionResult ComingSoon()
        {
            return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 2));
        }
        //Đặt vé
        public ActionResult BookTicket()
        {
            return View();
        }
        //Chọn ghế
        public ActionResult BookSeat()
        {
            return View();
        }
        //Thanh toán
        public ActionResult CheckOut()
        {
            return View();
        }

    }
}