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
                double? dem = 0;
                double? tong = 0 ;
                int count = 0;
                foreach(var item in db.movie_rate.Where(x=>x.movie_id == movie.id))
                {
                    dem += item.rate;
                    count++;
                }
                
                if(dem == 0)
                {
                    tong = 0;
                    ViewBag.RatesTong = tong;
                }
                else
                {
                    tong = dem / count;
                    tong = Math.Round((double)tong, 1);
                    ViewBag.RatesTong = tong;
                }
                
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
            if (Session["TenCus"] == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("SignIn", "User");
            }

            ViewBag.date = db.suat_chieu.Where(n => n.phim_id == id);

            return View();
        }

        public ActionResult ShowTime(int? idsuatchieu)
        {
            var suatChieuTime = db.suatchieu_timeframe.Where(n => n.id_Suatchieu == idsuatchieu).OrderBy(x=>x.id_Timeframe).ToList();

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
            if (Session["TenCus"] == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("SignIn", "User");
            }
            
            var idpc = db.suat_chieu.Find(id);
            ViewBag.ngaychieu = idpc.ngay_chieu;

            var time = db.suatchieu_timeframe.Where(x=>x.id_Suatchieu==id && x.id_Timeframe == idtime).FirstOrDefault();
            string time1 = time.TimeFrame.Time.ToString();
            string[] time2 = time1.Split(':');
            string timef = time2[0] + ':' + time2[1];
            ViewBag.giochieu = timef;

            ViewBag.tenphim = db.phims.Find(idpc.phim_id);
            phong_chieu phongChieu = db.phong_chieu.Find(idpc.phong_chieu_id);
            ViewBag.pc = phongChieu;
            var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == phongChieu.id).ToList();
            ViewBag.ghe = ghengoi;
            ViewBag.idtime = idtime;
            ViewBag.idsc = id;
            var order = db.orders.Where(n => n.suatchieu_id == idpc.id && n.status == idtime);
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
        public ActionResult CheckOut(int? id, int? idtime, string idg)
        {
            if (Session["TenCus"] == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("SignIn", "User");
            }
            var sc = db.suat_chieu.Find(id);            
            var mkh = Convert.ToInt32(Session["MaKH"]);
            var kh = db.khach_hang.Find(mkh);
            ViewBag.time = db.TimeFrames.Find(idtime);
            ViewBag.kh = kh;
            List<String> idghe = new List<String>();
            string[] listid = idg.Split(',');
            for (int i =0; i < listid.Length;i++)
            {
                if (listid[i] != "")
                {
                    idghe.Add(listid[i]);
                }
            }
            ViewBag.idghengoi = idghe;
            ViewBag.soluongh = idghe.Count();
            return View(sc);
        }

        [HttpPost]
        public ActionResult withpay (FormCollection f)
        {
            TempData["idghe"] = Request.Form["idghe"];
            TempData["idsuatc"] = Request.Form["idsuatc"];
            TempData["idtime"] = Request.Form["idtime"];
            TempData["idkh"] = int.Parse(Session["MaKH"].ToString());
            return RedirectToAction("PaymentWithPaypal", "Payment");
        }

        [HttpPost]
        public ActionResult AddRate(movie_rate movieRate)
        {
            //if (Session["TenCus"] != null)
            //{
            //    return Json(new { success = false });
            //}
            if(Session["MaKH"]!= null)
            {

            }
            else
            {
                return Json(new { success = false });
            }
            int uID = int.Parse(Session["MaKH"].ToString());
            var listorder = db.orders.Where(x => x.id_khachhang == uID);
            int demorder = 0;
            foreach (var item in listorder)
            {
                if (item.id_phim == movieRate.movie_id)
                {
                    demorder++;
                }
            }
            if (demorder == 0)
            {
                return Json(new { check = true });
            }
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