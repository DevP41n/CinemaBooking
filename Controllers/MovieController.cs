using CinemaBooking.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;

namespace CinemaBooking.Controllers
{
    public class MovieController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Movie
        //Chi tiết phim
        public ActionResult MovieDetail(String id)
        {
            try
            {
                var movie = db.phims
                            .Where(m => m.slug == id && m.status == 1).First();
                return View(movie);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //Phim đang chiếu
        public ActionResult NowShowing(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 1).OrderBy(s=>s.loai_phim_chieu == 1).ToPagedList(pageNumber,pageSize));
        }
        //Phim sắp chiếu
        public ActionResult ComingSoon(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 2).OrderBy(s => s.loai_phim_chieu == 2).ToPagedList(pageNumber,pageSize));
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