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

            var listsc = db.suat_chieu.Where(x => x.status == 1 || x.status == 2).ToList();
            TimeSpan timecheck = new TimeSpan(0, 30, 0);
            //suất chiếu + giờ chiếu cuối cùng - đi 30 phút < giờ hiện tại thì sc hết hạn
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
            return View(db.suat_chieu.ToList());
        }

        public ActionResult CreateShowTime()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.Timeid = new SelectList(db.TimeFrames.ToList().OrderBy(n => n.Time), "id", "Time");
            ViewBag.phim_id = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.rapchieu = db.rap_chieu.ToList();

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

                ViewBag.Timeid = new SelectList(db.TimeFrames.ToList().OrderBy(n => n.Time), "id", "Time");
                ViewBag.phim_id = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
                ViewBag.rapchieu = db.rap_chieu.ToList();
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
                ViewBag.rapchieu = db.rap_chieu.ToList();
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
            if (ModelState.IsValid)
            {
                ViewBag.rapchieu = db.rap_chieu.ToList();
                if (ModelState.IsValid)
                {
                    db.Entry(suatChieu).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Cập nhật thành công!";
                    return RedirectToAction("ListShowTime");
                }
                else
                {
                    TempData["Error"] = "Cập nhật không thành công!";
                }
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


                var room = db.phong_chieu.Where(n => n.id_rapchieu == id).ToList();
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
                var room = db.phong_chieu.Where(n => n.id_rapchieu == id).ToList();
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

                    TempData["Massage"] = "Cập nhật trạng thái thành công!";
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
                //Note : chưa check - fix nhiều quá mệt chưa kịp fix
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

                //Note : chưa check - fix nhiều quá mệt chưa kịp fix
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
                Times.Add(contime);
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
                Times.Add(contime);
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
            var timeframe = db.TimeFrames.ToList().OrderBy(n => n.Time);
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
                    times.Add(g);
                }
            }
            var count = idtimes.Count();
            return Json(data: new { idtimes, times, count, id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateTimeFr(int? id, int? idtime)
        {
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
            return View(db.TimeFrames.ToList().OrderBy(n => n.Time));
        }

        public ActionResult CreateShowTimeFrame()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateShowTimeFrame(TimeFrame timeFrame)
        {
            if (ModelState.IsValid)
            {
                db.TimeFrames.Add(timeFrame);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListShowTimeFrame");
            }
            return View(timeFrame);
        }

        public ActionResult EditShowTimeFrame(int? id)
        {
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
            TimeFrame timeFrame = db.TimeFrames.Find(id);
            db.TimeFrames.Remove(timeFrame);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListShowTimeFrame");
        }
    }
}