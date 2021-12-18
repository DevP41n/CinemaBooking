using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class ShowtimeController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/Showtime
        public ActionResult ListShowTime()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            var listsc = db.suat_chieu.Where(x => x.status == 1 || x.status == 2).ToList();
            TimeSpan timecheck = new TimeSpan(0, 10, 0);
            //suất chiếu + giờ chiếu cuối cùng - đi 10 phút < giờ hiện tại thì sc hết hạn
            foreach (var item in listsc)
            {
                var listtime = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == item.id).OrderByDescending(x => x.TimeFrame.Time).FirstOrDefault();
                if (listtime != null)
                {
                    if (item.ngay_chieu + listtime.TimeFrame.Time - timecheck <= DateTime.Now)
                    {
                        //Chuyển sang hết hạn
                        item.status = 0;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else if (listtime == null)
                {
                    if (item.ngay_chieu <= DateTime.Now)
                    {
                        item.status = 0;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.cbcongchieu = db.suat_chieu.Where(x => x.status == 2).Count();
            return View(db.suat_chieu.ToList().OrderBy(x => x.ngay_chieu));
        }

        public ActionResult CreateShowTime()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            ViewBag.Timeid = new SelectList(db.TimeFrames.Where(x => x.status == 1).ToList().OrderBy(n => n.Time), "id", "Time");
            ViewBag.phim_id = new SelectList(db.phims.ToList().Where(n => n.loai_phim_chieu == 1 && n.status == 1).OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.rapchieu = db.rap_chieu.Where(x => x.status == 1).ToList();

            return View();
        }
        [HttpPost]
        public ActionResult CreateShowTime(suat_chieu suatChieu, String[] timeeframe)
        {

            if (timeeframe == null)
            {
                TempData["Warning"] = "Giờ chiếu không thể trống!";
                return RedirectToAction("CreateShowTime");
            }
            if (ModelState.IsValid)
            {

                ViewBag.Timeid = new SelectList(db.TimeFrames.Where(n=>n.status==1).ToList().OrderBy(n => n.Time), "id", "Time");
                ViewBag.phim_id = new SelectList(db.phims.Where(n => n.loai_phim_chieu == 1 && n.status == 1).ToList().OrderBy(n => n.id), "id", "ten_phim");
                ViewBag.rapchieu = db.rap_chieu.Where(x => x.status == 1).ToList();
                suatchieu_timeframe sctime = new suatchieu_timeframe();
                suatChieu.status = 2;

                // không cho đặt suất chiếu ngày hiện tại Hoặc đặt suất chiếu trước 10 ngày
                string now = (DateTime.Now).ToString("dd/MM/yyyy");
                TimeSpan plustime1day = new TimeSpan(1, 0, 0, 0);
                DateTime datenow = Convert.ToDateTime(now) + plustime1day;
                //10 ngày
                TimeSpan check10day = new TimeSpan(11, 0, 0, 0);
                DateTime date10day = Convert.ToDateTime(now) + check10day;
                if (suatChieu.ngay_chieu < datenow || suatChieu.ngay_chieu > date10day)
                {
                    TempData["Warning"] = "Ngày chiếu phải hơn ngày hiện tại 1 ngày hoặc không thể hơn quá 10 ngày!";
                    return View();
                }
                var phim = db.phims.Find(suatChieu.phim_id);
                if (phim.status != 1 || phim.loai_phim_chieu != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phim này chưa công chiếu hoặc đã bị ẩn";
                    return RedirectToAction("CreateShowTime");
                }

                //check ngày công chiếu với ngày chiếu
                string ngaycongchieu = Convert.ToDateTime(phim.ngay_cong_chieu).ToString("dd/MM/yyyy");
                if (phim.ngay_cong_chieu > suatChieu.ngay_chieu)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Ngày công chiếu phải từ " + ngaycongchieu + " trở đi";
                    return RedirectToAction("CreateShowTime");
                }

                //check lại nếu có suất chiếu trùng ngày + trùng phòng (cùng rạp) => không cho tạo
                var rc = db.phong_chieu.Find(suatChieu.phong_chieu_id);
                var checksc = db.suat_chieu.Where(x => x.phim_id == suatChieu.phim_id && x.ngay_chieu == suatChieu.ngay_chieu
                                                        && x.phong_chieu.rap_chieu.id == rc.rap_chieu.id && x.status != 0).Count();
                if (checksc != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 suất chiếu tại rạp này. Vui lòng kiểm tra lại!";
                    return View();
                }


                //check khác film cùng rạp thì không cho tạo
                var checkkhac = db.suat_chieu.Where(x => x.phong_chieu_id == suatChieu.phong_chieu_id && x.phim_id != suatChieu.phim_id
                                                       && x.ngay_chieu == suatChieu.ngay_chieu && x.status != 0 && x.id != suatChieu.id).Count();

                if (checkkhac != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phòng chiếu đã chọn đã có suất chiếu. Vui lòng kiểm tra lại!";
                    return View();
                }

                db.suat_chieu.Add(suatChieu);
                db.SaveChanges();
                if (ModelState.IsValid)
                {
                    foreach (var timeidd in timeeframe)
                    {
                        int timeee = Convert.ToInt32(timeidd);
                        //TimeFrame time = db.TimeFrames.Find(timeee);
                        sctime.id_Timeframe = timeee;
                        sctime.id_Suatchieu = suatChieu.id;
                        db.suatchieu_timeframe.Add(sctime);
                        db.SaveChanges();
                    }
                    TempData["Message"] = "Tạo thành công, Suất chiếu đã chuyển sang phần chuẩn bị chiếu!";
                    return RedirectToAction("ListShowTime");
                }
            }
            else
            {
                TempData["Warning"] = "Không thể để trống!";
                return RedirectToAction("CreateShowTime");
            }
            return View(suatChieu);
        }


        public ActionResult EditShowTime(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {
                suat_chieu suatChieu = db.suat_chieu.Find(id);
                if (suatChieu.status != 2)
                {
                    TempData["Warning"] = "Suất này đang chiếu. Chỉ chỉnh sửa được suất chiếu đang chuẩn bị!";
                    return RedirectToAction("ListShowTime");
                }
                ViewBag.phim_id = new SelectList(db.phims.Where(n => n.loai_phim_chieu == 1 && n.status == 1).ToList().OrderBy(n => n.id), "id", "ten_phim");
                ViewBag.rapchieu = db.rap_chieu.Where(x => x.status == 1).ToList();
                if (suatChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                return View(suatChieu);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditShowTime(suat_chieu suatChieu)
        {
            ViewBag.phim_id = new SelectList(db.phims.Where(n => n.loai_phim_chieu == 1 && n.status == 1).ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.rapchieu = db.rap_chieu.Where(x => x.status == 1).ToList();
            if (ModelState.IsValid)
            {
                //// check xem có sửa gì không nếu không thì trả về
                //suat_chieu sccc = db.suat_chieu.Find(suatChieu.id);
                //if (sccc.ngay_chieu == suatChieu.ngay_chieu && sccc.phim_id == suatChieu.phim_id && sccc.phong_chieu_id == suatChieu.phong_chieu_id 
                //    && sccc.id == suatChieu.id)
                //{
                //    TempData["Message"] = "Bạn đã chưa thay đổi gì khi sửa suất chiếu!";
                //    return RedirectToAction("ListShowTime");
                //}

                var phim = db.phims.Find(suatChieu.phim_id);
                if (phim == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                if (phim.status != 1 || phim.loai_phim_chieu != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phim này chưa công chiếu hoặc đã bị ẩn";
                    return RedirectToAction("ListShowTime");
                }

                var pccc = db.phong_chieu.Find(suatChieu.phong_chieu_id);
                if (pccc == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                if (pccc.status != 1)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! phòng chiếu này chưa hoạt động hoặc không có";
                    return RedirectToAction("ListShowTime");
                }


                string ngaycongchieu = Convert.ToDateTime(phim.ngay_cong_chieu).ToString("dd/MM/yyyy");
                //check ngày công chiếu với ngày chiếu
                if (phim.ngay_cong_chieu > suatChieu.ngay_chieu)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Ngày công chiếu phải từ " + ngaycongchieu + " trở đi";
                    return RedirectToAction("EditShowTime", new { id = suatChieu.id });
                }

                // không cho đặt suất chiếu ngày hiện tại Hoặc đặt suất chiếu trước 10 ngày
                string now = (DateTime.Now).ToString("dd/MM/yyyy");
                //string checkdate = Convert.ToDateTime(suatChieu.ngay_chieu).ToString("dd/MM/yyyy");
                DateTime datee = Convert.ToDateTime(now);
                //TimeSpan plustime1day = new TimeSpan(1, 0, 0, 0);
                //DateTime datenow = Convert.ToDateTime(now) + plustime1day;
                //10 ngày
                TimeSpan check10day = new TimeSpan(11, 0, 0, 0);
                DateTime date10day = Convert.ToDateTime(now) + check10day;
                if (suatChieu.ngay_chieu < datee || suatChieu.ngay_chieu > date10day)
                {
                    TempData["Warning"] = "Ngày chiếu phải hơn ngày hiện tại 1 ngày hoặc không thể hơn quá 10 ngày!";
                    return RedirectToAction("EditShowTime", new { id = suatChieu.id });
                }

                //check lại nếu có suất chiếu trùng ngày + trùng phòng (cùng rạp) => không cho sửa lại(không tính chính nó)
                var rc = db.phong_chieu.Find(suatChieu.phong_chieu_id);
                var checksc = db.suat_chieu.Where(x => x.phim_id == suatChieu.phim_id && x.ngay_chieu == suatChieu.ngay_chieu
                                                        && x.phong_chieu.rap_chieu.id == rc.rap_chieu.id && x.status != 0 && x.id != suatChieu.id).Count();
                if (checksc != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 suất chiếu tại rạp này. Vui lòng kiểm tra lại!";
                    return RedirectToAction("EditShowTime", new { id = suatChieu.id });
                }


                //check khác film cùng rạp thì không cho tạo
                var checkkhac = db.suat_chieu.Where(x => x.phong_chieu_id == suatChieu.phong_chieu_id && x.phim_id != suatChieu.phim_id
                                                       && x.ngay_chieu == suatChieu.ngay_chieu && x.status != 0 && x.id != suatChieu.id).Count();

                if (checkkhac != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Phòng chiếu đã chọn đã có suất chiếu. Vui lòng kiểm tra lại!";
                    return RedirectToAction("EditShowTime", new { id = suatChieu.id });
                }

                db.Entry(suatChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListShowTime");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(suatChieu);
        }

        //Phòng chiếu của rạp chiếu
        public JsonResult RoomOfCinema(int? id, int? idphong)
        {
            if (id == null)
            {
                return Json(new { succes = false });
            }
            List<int> idroom = new List<int>();
            List<string> roomname = new List<string>();
            if (id == 0)
            {
                var count = idroom.Count();
                return Json(new { count, idroom, roomname }, JsonRequestBehavior.AllowGet);
            }
            if (idphong == null)
            {


                var room = db.phong_chieu.Where(n => n.id_rapchieu == id && n.status == 1).ToList();
                foreach (var item in room)
                {
                    idroom.Add(item.id);
                    roomname.Add(item.ten_phong);
                }
                var count = idroom.Count();
                return Json(new { count, idroom, roomname }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var room = db.phong_chieu.Where(n => n.id_rapchieu == id && n.status == 1).ToList();
                foreach (var item in room)
                {
                    if (item.id != idphong)
                    {
                        idroom.Add(item.id);
                        roomname.Add(item.ten_phong);
                    }

                }
                var count = idroom.Count();
                return Json(new { count, idroom, roomname }, JsonRequestBehavior.AllowGet);
            }
        }




        // chuyển sang công chiếu
        public ActionResult changetoSTime(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {
                suat_chieu sc = db.suat_chieu.Find(id);
                if (sc == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }

                if (sc.status != 2)
                {
                    TempData["Error"] = "Suất này đã và đang công chiếu hoặc đã hết hạn!";
                    return RedirectToAction("ListShowTime");
                }

                var listtime = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == sc.id).OrderByDescending(x => x.TimeFrame.Time).FirstOrDefault();
                if (listtime == null)
                {
                    TempData["Error"] = "Chưa có giờ chiếu. Không thể công chiếu!";
                    return RedirectToAction("ListShowTime");
                }
                TimeSpan timecheck = new TimeSpan(0, 30, 0);
                //suất chiếu + giờ chiếu cuối cùng - đi 30 phút > giờ hiện tại thì công chiếu
                if (sc.ngay_chieu + listtime.TimeFrame.Time - timecheck > DateTime.Now)
                {
                    //Chuyển sang công chiếu
                    sc.status = 1;
                    db.Entry(sc).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Cập nhật trạng thái thành công!";
                    return RedirectToAction("ListShowTime");
                }
                else
                {
                    TempData["Error"] = "Cập nhật trạng thái không thành công vì quá hạn!";
                    return RedirectToAction("ListShowTime");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Cập nhật trạng thái không thành công!";
                return RedirectToAction("ListShowTime");
            }
        }


        //Back về nếu chưa có ai đặt
        public ActionResult BackToReady(int? id)
        {

            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {
                suat_chieu suatChieu = db.suat_chieu.Find(id);
                if (suatChieu.status != 1)
                {
                    TempData["Error"] = "Đã xảy ra lỗi!";
                    return RedirectToAction("ListShowTime");
                }

                var check = db.orders.Where(x => x.suatchieu_id == suatChieu.id).Count();
                if (check != 0)
                {
                    TempData["Error"] = "Không thể sửa trạng thái: chuẩn bị công chiếu, vì đã có người đặt vé ở suất này!";
                    return RedirectToAction("ListShowTime");
                }
                //Xóa giờ chiếu
                suatChieu.status = 2;
                db.Entry(suatChieu).State = EntityState.Modified;
                db.SaveChanges();

                TempData["Message"] = "Đã chỉnh sửa trạng thái: chuẩn bị công chiếu!";
                return RedirectToAction("ListShowTime");

            }
            catch (Exception)
            {
                TempData["Error"] = "Đã xảy ra lỗi!";
                return RedirectToAction("ListShowTime");
            }
        }

        //Xóa khi chưa công chiếu.

        public ActionResult DeleteConfirmed(int? id)
        {

            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {
                suat_chieu suatChieu = db.suat_chieu.Find(id);
                if (suatChieu.status != 2)
                {
                    TempData["Error"] = "Đã xảy ra lỗi!";
                    return RedirectToAction("ListShowTime");
                }

                //Xóa giờ chiếu
                var timesc = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == id).ToList();
                foreach (var item in timesc)
                {
                    suatchieu_timeframe sctime = db.suatchieu_timeframe.Find(item.id);
                    db.suatchieu_timeframe.Remove(sctime);
                    db.SaveChanges();
                }

                db.suat_chieu.Remove(suatChieu);
                db.SaveChanges();

                TempData["Message"] = "Xóa suất chiếu thành công!";
                return RedirectToAction("ListShowTime");

            }
            catch (Exception)
            {
                TempData["Error"] = "Đã xảy ra lỗi!";
                return RedirectToAction("ListShowTime");
            }
        }


        //show giờ chiếu và có thể chỉnh sửa cho phần chuẩn bị chiếu

        public ActionResult ShowDataTime(int? idsuatchieu)
        {
            var list = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == idsuatchieu).OrderBy(x => x.TimeFrame.Time).ToList();
            List<int> id = new List<int>();
            List<String> Times = new List<String>();
            foreach (var item in list)
            {
                id.Add(item.id);
                var contime = Convert.ToString(item.TimeFrame.Time);
                string[] t = contime.Split(':');
                //Format lại bỏ phần milisecond
                Times.Add(t[0] + ":" + t[1]);
            }
            var count = list.Count();
            return Json(data: new { id, Times, count, idsuatchieu }, JsonRequestBehavior.AllowGet);
        }


        //show giờ chiếu cho stt = 1, stt = 0
        public ActionResult ShowTimeFor12(int? idsuatchieu)
        {
            var list = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == idsuatchieu).OrderBy(x => x.TimeFrame.Time).ToList();
            List<int> id = new List<int>();
            List<String> Times = new List<String>();
            foreach (var item in list)
            {
                id.Add(item.id);
                var contime = Convert.ToString(item.TimeFrame.Time);
                string[] t = contime.Split(':');
                //Format lại bỏ phần milisecond
                Times.Add(t[0] + ":"+ t[1]);
            }
            var count = list.Count();
            return Json(data: new { id, Times, count, idsuatchieu }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult DeleteTime(int? id)
        {
            suatchieu_timeframe sctime = db.suatchieu_timeframe.Find(id);
            db.suatchieu_timeframe.Remove(sctime);
            db.SaveChanges();
            return Json(new { success = true });
        }


        [HttpGet]
        public ActionResult ShowCreateTimeFr(int? id)
        {
            var time = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == id);
            var timeframe = db.TimeFrames.Where(x => x.status == 1).ToList().OrderBy(n => n.Time);
            List<int> idtimes = new List<int>();
            List<String> times = new List<String>();
            foreach (var item in timeframe)
            {
                var dem = 0;
                foreach (var i in time)
                {
                    if (item.id == i.id_Timeframe)
                    {
                        dem++;
                    }
                }
                if (dem == 0)
                {
                    idtimes.Add(item.id);
                    var g = Convert.ToString(item.Time);
                    string[] t = g.Split(':');
                    //Format lại bỏ phần milisecond
                    times.Add(t[0] + ":" + t[1]);
                }
            }
            var count = idtimes.Count();
            return Json(data: new { idtimes, times, count, id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateTimeFr(int? id, int? idtime)
        {
            var check = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == id && x.id_Timeframe == idtime).Count();
            if(check!=0)
            {
                return Json(new { success = false });
            }
            suatchieu_timeframe time = new suatchieu_timeframe();
            time.id_Suatchieu = id;
            time.id_Timeframe = idtime;
            db.suatchieu_timeframe.Add(time);
            db.SaveChanges();
            return Json(new { success = true });
        }
        //Time Frame
        /// ---------------------------
        ///
        public ActionResult ListShowTimeFrame()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            return View(db.TimeFrames.Where(x=>x.status == 1).ToList().OrderBy(n => n.Time));
        }

        public ActionResult CreateShowTimeFrame()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateShowTimeFrame(TimeFrame timeFrame)
        {
            var check = db.TimeFrames.Where(x => x.Time == timeFrame.Time  && x.status == 1).Count();
            if(check!=0)
            {
                TempData["Warning"] = "Đã tồn tại thời gian trên!";
                return View();
            }
            if (ModelState.IsValid)
            {
                timeFrame.status = 1;
                db.TimeFrames.Add(timeFrame);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListShowTimeFrame");
            }
            return View(timeFrame);
        }

        public ActionResult EditShowTimeFrame(int? id)
        {
            var check = db.suatchieu_timeframe.Where(x => x.id_Timeframe == id);
            int dem = 0;
            foreach (var item in check)
            {
                var checksc = db.suat_chieu.Find(item.id_Suatchieu);
                if (checksc.status != 0)
                {
                    dem++;
                    break;
                }
            }
            if (dem > 0)
            {
                TempData["Warning"] = "Giờ này đang tồn tại trong suất chiếu đang có!";
                return RedirectToAction("ListShowTimeFrame");
            }

            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            TimeFrame timeFrame = db.TimeFrames.Find(id);
            if (timeFrame == null)
            {
                return HttpNotFound();
            }
            return View(timeFrame);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditShowTimeFrame(TimeFrame timeFrame)
        {


            var check = db.TimeFrames.Where(x => x.Time == timeFrame.Time && x.id != timeFrame.id && x.status == 1).Count();
            if (check != 0)
            {
                TempData["Warning"] = "Đã tồn tại thời gian trên!";
                return RedirectToAction("EditShowTimeFrame","Showtime", new { id = timeFrame.id});
            }
            if (ModelState.IsValid)
            {
                db.Entry(timeFrame).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListShowTimeFrame");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(timeFrame);
        }

        public ActionResult DeleteConfirmedFrame(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }
            var check = db.suatchieu_timeframe.Where(x => x.id_Timeframe == id);
            int dem = 0;
            foreach(var item in check)
            {
                var checksc = db.suat_chieu.Find(item.id_Suatchieu);
                if(checksc.status !=0)
                {
                    dem++;
                    break;
                }
            }
            if(dem>0)
            {
                TempData["Warning"] = "Giờ này đang tồn tại trong suất chiếu đang có!";
                return RedirectToAction("ListShowTimeFrame");
            }
            TimeFrame timeFrame = db.TimeFrames.Find(id);
            timeFrame.status = 0;
            TempData["Message"] = "Xóa thành công!";
            db.Entry(timeFrame).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListShowTimeFrame");
        }
    }
}