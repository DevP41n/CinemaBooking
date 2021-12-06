using CinemaBooking.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            if (id == null || id == "")
            {
                return RedirectToAction("Error404", "Home");
            }

            try
            {
                var movie = db.phims
                            .Where(m => m.slug == id && m.status == 1).First();
                ViewBag.Rates = db.movie_rate.Where(m => m.movie_id == movie.id);
                ViewBag.RatesCount = db.movie_rate.Where(m => m.movie_id == movie.id).Count();
                double? dem = 0;
                double? tong = 0;
                int count = 0;
                foreach (var item in db.movie_rate.Where(x => x.movie_id == movie.id))
                {
                    dem += item.rate;
                    count++;
                }

                if (dem == 0)
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
                return RedirectToAction("Error404", "Home");
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
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 2).OrderBy(s => s.ngay_cong_chieu).Where(x => x.theloaichinh.ToString().Contains(category1)).ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View(db.phims.Where(s => s.status == 1 && s.loai_phim_chieu == 2).OrderBy(s => s.ngay_cong_chieu).ToPagedList(pageNumber, pageSize));
            }

        }
        //Đặt vé
        public ActionResult BookTicket(int? id)
        {
            if (id != null)
            {
                if (Session["TenCus"] == null)
                {
                    string CurrentURL = Request.UrlReferrer.ToString(); //Request.UrlReferrer.ToString() Lấy ra url hiện tại
                    TempData["Warning"] = "Vui lòng đăng nhập";
                    return RedirectToAction("SignIn", "User", new { url = CurrentURL });
                }

                ViewBag.tenphim = db.phims.Find(id);
                //Lấy ra list suất chiếu với id phim
                var ngay = db.suat_chieu.Where(n => n.phim_id == id).OrderBy(n => n.ngay_chieu).ToList();
                List<DateTime?> ng = new List<DateTime?>();
                ViewBag.idphim = id;

                //Tạo biến congngay để cộng thêm vào ngày suất chiếu 1 ngày
                TimeSpan congngay = new TimeSpan(1, 0, 0, 0);

                foreach (var item in ngay)
                {
                    var checknull = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == item.id).Count(); //Kiểm tra xem ngày chiếu này có giờ chiếu nào không
                    if (checknull > 0) //Nếu có giờ chiếu thì dô, không thì khỏi dô :)))
                    {
                        var ngaychieu = Convert.ToDateTime(item.ngay_chieu);
                        var datenow = DateTime.UtcNow.ToString("d");

                        if ((ngaychieu + congngay) > DateTime.Now)  //Nếu ngày chiếu của suất chiếu cộng thêm 1 ngày mà lớn hơn ngày hiện tại thì thêm vào list.
                        {
                            var dem = 0;
                            if (ngaychieu == Convert.ToDateTime(datenow))
                            {
                                //Lấy ra suất chiếu có trong item.ngay_chieu của mảng ngay
                                var suatchieu = db.suat_chieu.Where(n => n.ngay_chieu == item.ngay_chieu && n.phim_id == id).ToList();
                                foreach (var sctoday in suatchieu)
                                {
                                    //Lấy ra danh sách giờ chiếu theo id suất chiếu của mảng suatchieu
                                    var suatChieuTime = db.suatchieu_timeframe.Where(n => n.id_Suatchieu == sctoday.id).OrderBy(x => x.id_Timeframe).ToList();
                                    TimeSpan timenow = DateTime.Now.TimeOfDay;
                                    foreach (var time in suatChieuTime)
                                    {
                                        if (time.TimeFrame.Time > timenow) // Nếu giờ của suất chiếu lớn giờ của hiện tại thì có suất chiếu giờ đó, tăng biến đếm lên 1.
                                        {
                                            dem++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                dem = -1; // Nếu ngày chiếu là ngày mơi ngày mốt gì đó thì cho biết đếm = -1 để nó add thẳng dô list luôn :D
                            }

                            if (dem > 0)   // Nếu biến đếm > 0 thì add ngày vào (vì có suất chiếu trong ngày)
                                           // nếu = 0 thì không add (vì không có suất chiếu nào trong ngày ).
                            {
                                ng.Add(item.ngay_chieu);
                            }
                            if (dem < 0)
                            {
                                ng.Add(item.ngay_chieu);
                            }
                        }
                    }
                }
                //Lấy ra ViewBag ngày với điều kiện ngày không được trùng.
                ViewBag.date = ng.Distinct();


                ViewBag.rap = db.rap_chieu.ToList().Distinct();
                var checkdate = ng.Distinct().Count();

                if (checkdate <= 0)
                {
                    TempData["Warning"] = "Phim này đã hết hoặc chưa có suất chiếu !";
                    return Redirect(Request.UrlReferrer.ToString());
                }
            }
            else
            {
                return RedirectToAction("Error404", "Home");
            }
            return View();
        }

        public ActionResult ShowCinema(string ngay, string idphim)
        {
            if (ngay != "")
            {
                var ng = Convert.ToDateTime(ngay);
                int idfilm = Convert.ToInt32(idphim);
                var suatchieu = db.suat_chieu.Where(n => n.ngay_chieu == ng && n.phim_id == idfilm).ToList();
                List<String> tenrap = new List<String>();


                foreach (var item in suatchieu)
                {
                    var suatChieuTime = db.suatchieu_timeframe.Where(n => n.id_Suatchieu == item.id).OrderBy(x => x.TimeFrame.Time).ToList(); //OrderBy theo giờ chiếu của bảng Time

                    //Chạy vòng lặp trong list suất chiếu timeframe để thêm từ suất chiếu vào list.
                    foreach (var time in suatChieuTime)
                    {
                        var datenow = DateTime.UtcNow.ToString("d");
                        if (ng == Convert.ToDateTime(datenow))
                        {
                            //Tạo biến thời gian hiện tại để so sánh với giờ của suất chiếu.
                            TimeSpan timenow = DateTime.Now.TimeOfDay;
                            if (time.TimeFrame.Time > timenow) // Nếu giờ của suất chiếu lớn hơn giờ của hiện tại thì thêm vào, nếu không thì không thêm.
                            {
                                tenrap.Add(item.phong_chieu.rap_chieu.ten_rap);
                            }
                        }
                        else // Nếu ngày chiếu cũng là ngày mơi ngày mốt gì đó thì cho biết đếm -- để nó add dô list luôn
                             // vì suất chiếu ngày = ngày hiện tại thì mới cho ++ :D
                        {
                            tenrap.Add(item.phong_chieu.rap_chieu.ten_rap);
                        }
                    }

                    //Chạy xong vòng lặp, Nếu biến dem > 0 thì thêm dấu - vào để phân biệt              

                }
                var rapphim = tenrap.Distinct();
                var Count = rapphim.Count();

                return Json(data: new { Count, rapphim }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(data: new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ShowTime(string ngay, string idphim)
        {
            if (ngay != "")
            {
                var ng = Convert.ToDateTime(ngay);
                int idfilm = Convert.ToInt32(idphim);
                var suatchieu = db.suat_chieu.Where(n => n.ngay_chieu == ng && n.phim_id == idfilm).ToList();

                //Tạo list để add giờ, id giờ, id suất chiếu
                List<String> times = new List<String>();
                List<String> idtimes = new List<String>();
                List<String> idsc = new List<String>();
                List<String> tenrap = new List<String>();
                string nhay = "-";
                times.Add(nhay);
                idtimes.Add(nhay);
                idsc.Add(nhay);
                tenrap.Add(nhay);

                //Chạy vòng lặp trong list suất chiếu lấy từ ngày chiếu và id phim.
                foreach (var item in suatchieu)
                {
                    var dem = 0;
                    var suatChieuTime = db.suatchieu_timeframe.Where(n => n.id_Suatchieu == item.id).OrderBy(x => x.TimeFrame.Time).ToList(); //OrderBy theo giờ chiếu của bảng Time

                    //Chạy vòng lặp trong list suất chiếu timeframe để thêm từ suất chiếu vào list.
                    foreach (var time in suatChieuTime)
                    {
                        var datenow = DateTime.UtcNow.ToString("d");
                        if (ng == Convert.ToDateTime(datenow))
                        {
                            //Tạo biến thời gian hiện tại để so sánh với giờ của suất chiếu.
                            TimeSpan timenow = DateTime.Now.TimeOfDay;
                            if (time.TimeFrame.Time > timenow) // Nếu giờ của suất chiếu lớn hơn giờ của hiện tại thì thêm vào, nếu không thì không thêm.
                            {
                                times.Add((time.TimeFrame.Time).ToString());
                                idtimes.Add(time.TimeFrame.id.ToString());
                                idsc.Add(item.id.ToString());
                                tenrap.Add(item.phong_chieu.rap_chieu.ten_rap);
                                dem++;
                            }
                        }
                        else // Nếu ngày chiếu cũng là ngày mơi ngày mốt gì đó thì cho biết đếm -- để nó add dô list luôn
                             // vì suất chiếu ngày = ngày hiện tại thì mới cho ++ :D
                        {
                            times.Add((time.TimeFrame.Time).ToString());
                            idtimes.Add(time.TimeFrame.id.ToString());
                            idsc.Add(item.id.ToString());
                            tenrap.Add(item.phong_chieu.rap_chieu.ten_rap);
                            dem--;
                        }
                    }

                    //Chạy xong vòng lặp, Nếu biến dem > 0 thì thêm dấu - vào để phân biệt              
                    if (dem > 0)
                    {
                        times.Add(nhay);
                        idtimes.Add(nhay);
                        idsc.Add(nhay);
                        tenrap.Add(nhay);
                    }
                    else if (dem < 0)
                    {
                        times.Add(nhay);
                        idtimes.Add(nhay);
                        idsc.Add(nhay);
                        tenrap.Add(nhay);
                    }

                }

                var Count = times.Count();

                return Json(data: new { times, idtimes, Count, idsc, tenrap }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["Warning"] = "Phim này đã hết hoặc chưa có suất chiếu !";
                var slug = db.phims.Find(Convert.ToInt32(idphim));
                return RedirectToAction("MovieDetail", "Movie", new { id = slug.slug });
            }
        }

        //[HttpPost]
        //public ActionResult CheckSeat(string suatchieu, string giochieu)
        //{
        //    int idsc = Convert.ToInt32(suatchieu);
        //    int idtime = Convert.ToInt32(giochieu);
        //    //ghế đang chờ thanh toán
        //    TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
        //    var orderss = db.orders.Where(n => n.suatchieu_id == idsc && n.idtime == idtime && n.status == 10);
        //    var counttt = orderss.Count();
        //    List<int> idgheddss = new List<int>();
        //    foreach (var itemm in orderss)
        //    {
        //        if (itemm.ngay_mua + tinhgio <= DateTime.Now)
        //        {
        //            itemm.status = 0;
        //            db.Entry(itemm).State = EntityState.Modified;
        //        }
        //    }
        //    db.SaveChanges();
        //    return RedirectToAction("BookSeat", "Movie", new { idd = suatchieu, idtimee = giochieu });
        //}


        //Chọn ghế
        public ActionResult BookSeat(string idd, string idtimee)
        {
            try
            {
                if (Session["TenCus"] == null)
                {
                    string CurrentURL = HttpContext.Request.Url.AbsoluteUri;
                    TempData["Warning"] = "Vui lòng đăng nhập";
                    return RedirectToAction("SignIn", "User", new { url = CurrentURL });
                }

                if (idd == null || idtimee == null || idd == "" || idtimee == "")
                {
                    TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                    return RedirectToAction("Error404", "Home");
                }


                var id = Convert.ToInt32(idd);

                var idtime = Convert.ToInt32(idtimee);
                var idpc = db.suat_chieu.Find(id);

                var time = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == id && x.id_Timeframe == idtime).FirstOrDefault();
                //ràng buộc
                if (time == null || idpc == null)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                    return RedirectToAction("Error404", "Home");
                }
                ViewBag.ngaychieu = idpc.ngay_chieu;
                string time1 = time.TimeFrame.Time.ToString();
                string[] time2 = time1.Split(':');
                string timef = time2[0] + ':' + time2[1];
                ViewBag.giochieu = timef;

                ViewBag.tenphim = db.phims.Find(idpc.phim_id);
                phong_chieu phongChieu = db.phong_chieu.Find(idpc.phong_chieu_id);
                ViewBag.pc = phongChieu;
                var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == phongChieu.id).OrderBy(x=>x.Row);
                ViewBag.ghe = ghengoi;
                ViewBag.idtime = idtime;
                ViewBag.idsc = id;
                //ghế đang chờ thanh toán
                TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
                                                           //Status 2: đang chờ thanh toán tại quầy
                var orderss = db.orders.Where(n => n.suatchieu_id == idpc.id && n.idtime == idtime && n.status == 2);
                var counttt = orderss.Count();
                List<int> idgheddss = new List<int>();
                foreach (var itemm in orderss)
                {
                    if (itemm.ngay_mua + tinhgio <= DateTime.Now)
                    {
                        //    var idghed = db.order_details.Where(n => n.id_orders == itemm.id);
                        //    foreach (var ii in idghed)
                        //    {
                        //        idgheddss.Add((int)ii.id_ghe);
                        //    }
                        //}
                        //else
                        //{
                        itemm.status = 0;
                        db.Entry(itemm).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();

                var order = db.orders.Where(n => n.suatchieu_id == idpc.id && n.idtime == idtime && n.status == 1 ||
                n.suatchieu_id == idpc.id && n.idtime == idtime && n.status == 2);
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
            catch (Exception)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error404", "Home");
            }
        }
        //Thanh toán
        public ActionResult CheckOut(int? id, int? idtime, string idg)
        {
            if (Session["TenCus"] == null)
            {
                string CurrentURL = HttpContext.Request.Url.AbsoluteUri;
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("SignIn", "User", new { url = CurrentURL });
            }

            if (id == null || idtime == null || idg == null)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error404", "Home");
            }

            if (id < 1 || idtime < 1 || idg == "")
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error404", "Home");
            }
            var sc = db.suat_chieu.Find(id);
            var mkh = Convert.ToInt32(Session["MaKH"]);
            var kh = db.khach_hang.Find(mkh);

            var timefr = db.TimeFrames.Find(idtime);

            if (sc == null || kh == null || timefr == null)
            {
                TempData["Warning"] = "Đã xảy ra lỗi, vui lòng chọn lại!";
                return RedirectToAction("Error404", "Home");
            }

            ViewBag.time = timefr;
            ViewBag.kh = kh;
            List<String> idghe = new List<String>();
            string[] listid = idg.Split(',');
            for (int i = 0; i < listid.Length; i++)
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
        public ActionResult withpay(FormCollection f)
        {
            TempData["idghe"] = Request.Form["idghe"];
            TempData["idsuatc"] = Request.Form["idsuatc"];
            TempData["idtime"] = Request.Form["idtime"];
            TempData["idkh"] = int.Parse(Session["MaKH"].ToString());
            return RedirectToAction("PaymentWithPaypal", "Payment");
        }

        public ActionResult MomoPay(FormCollection f)
        {
            string currentUrl = Request.UrlReferrer.ToString();
            TempData["Url"] = currentUrl;
            TempData["idghe"] = Request.Form["idghe"];
            TempData["idsuatc"] = Request.Form["idsuatc"];
            TempData["idtime"] = Request.Form["idtime"];
            TempData["idkh"] = int.Parse(Session["MaKH"].ToString());
            return RedirectToAction("Momo", "PaymentMomo");
        }

        public ActionResult ReceptionPay(FormCollection f)
        {
            TempData["idghe"] = Request.Form["idghe"];
            TempData["idsuatc"] = Request.Form["idsuatc"];
            TempData["idtime"] = Request.Form["idtime"];
            TempData["idkh"] = int.Parse(Session["MaKH"].ToString());
            return RedirectToAction("withReceptionPay", "ReceptionPayment");
        }

        [HttpPost]
        public ActionResult AddRate(movie_rate movieRate)
        {
            //if (Session["TenCus"] != null)
            //{
            //    return Json(new { success = false });
            //}
            if (Session["MaKH"] != null)
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