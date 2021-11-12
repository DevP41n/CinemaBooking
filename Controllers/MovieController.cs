using CinemaBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
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
        public ActionResult NowShowing(int? page, int? category)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if (category != null)
            {
                ViewBag.category = category;
                string category1 = category.ToString();
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 1).OrderByDescending(s => s.ngay_cong_chieu).Where(x => x.theloaichinh.ToString().Contains(category1)).ToPagedList(pageNumber, pageSize));
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
        public ActionResult BookTicket(int? id)
        {
            ViewBag.date = db.suat_chieu.Where(n => n.phim_id == id);

            return View();
        }

        public ActionResult ShowTime(int? idsuatchieu)
        {
            var suatChieuTime = db.suatchieu_timeframe.Where(n => n.id_Suatchieu == idsuatchieu);

            List<String> times = new List<String>();
            List<int> idtimes = new List<int>();
            foreach (var time in suatChieuTime)
            {
                times.Add((time.TimeFrame.Time).ToString());
                idtimes.Add(time.TimeFrame.id);
            }

            var Count = times.Count();

            return Json(data: new { times, idtimes, Count, idsuatchieu }, JsonRequestBehavior.AllowGet);
        }


        //Chọn ghế
        public ActionResult BookSeat(int id, int idtime)
        {

            var idpc = db.suat_chieu.Find(id);
            ViewBag.tenphim = db.phims.Find(idpc.phim_id);
            phong_chieu phongChieu = db.phong_chieu.Find(idpc.phong_chieu_id);
            ViewBag.pc = phongChieu;
            var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == phongChieu.id).ToList();
            ViewBag.ghe = ghengoi;

            var order = db.orders.Where(n => n.id_phong_chieu == phongChieu.id && n.status == idtime);
            List<int> idghedd = new List<int>();
            foreach (var item in order)
            {
                var idghe = db.order_details.Where(n => n.id_orders == item.id);
                foreach (var i in idghe)
                {
                    idghedd.Add((int)i.id_ghe);
                }
            }

            ViewBag.idghedat = idghedd;
            return View(ghengoi);
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