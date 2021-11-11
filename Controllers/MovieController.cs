using CinemaBooking.Models;
using PagedList;
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
            try
            {
                var movie = db.phims
                            .Where(m => m.slug == id && m.status == 1).First();
                ViewBag.Rates = db.movie_rate.Where(m => m.movie_id == movie.id);
                ViewBag.RatesCount = db.movie_rate.Where(m => m.movie_id == movie.id).Count();
                return View(movie);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //Phim đang chiếu
        public ActionResult NowShowing(int? page,int? category)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if(category != null)
            {
                ViewBag.category = category;
                string category1 = category.ToString();
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 1).OrderByDescending(s => s.ngay_cong_chieu).Where(x=>x.theloaichinh.ToString().Contains(category1)).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 1).OrderByDescending(s => s.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));
            }
            
            
        }
        //Phim sắp chiếu
        public ActionResult ComingSoon(int? page, int? category)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if (category != null)
            {
                ViewBag.category = category;
                string category1 = category.ToString();
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 2).OrderByDescending(s => s.ngay_cong_chieu).Where(x => x.theloaichinh.ToString().Contains(category1)).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 2).OrderByDescending(s => s.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));
            }

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

        [HttpPost]
        public ActionResult AddRate(movie_rate movieRate)
        {

            //if (Session["MaKH"] == null)
            //    return Json(new { result = 0 });
            int uID = int.Parse(Session["MaKH"].ToString());
            movieRate.ten_khachhang = Session["TenCus"].ToString();
            movieRate.khachhang_id = uID;
            movieRate.created_at = DateTime.Now;
            db.movie_rate.Add(movieRate);
            try
            {
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Json(new { success = false });
            }


        }
    }
}