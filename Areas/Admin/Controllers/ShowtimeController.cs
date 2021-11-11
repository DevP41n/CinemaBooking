﻿using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            return View(db.suat_chieu.ToList());
        }

        public ActionResult CreateShowTime()
        {
            ViewBag.Timeid = new SelectList(db.TimeFrames.ToList().OrderBy(n => n.id), "id", "Time");
            ViewBag.phim_id = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.phong_chieu_id = new SelectList(db.phong_chieu.ToList().OrderBy(n => n.id), "id", "ten_phong");

            return View();
        }
        [HttpPost]
        public ActionResult CreateShowTime(suat_chieu suatChieu, String[] timeeframe)
        {
            ViewBag.Timeid = new SelectList(db.TimeFrames.ToList().OrderBy(n => n.id), "id", "Time");
            ViewBag.phim_id = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.phong_chieu_id = new SelectList(db.phong_chieu.ToList().OrderBy(n => n.id), "id", "ten_phong");
            suatchieu_timeframe sctime = new suatchieu_timeframe();
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
                TempData["Message"] = "Tạo thành công!";
                return RedirectToAction("ListShowTime");
            }
            return View(suatChieu);
        }

        public ActionResult EditShowTime(int? id)
        {
            suat_chieu suatChieu = db.suat_chieu.Find(id);
            ViewBag.phim = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.phong_chieu = new SelectList(db.phong_chieu.ToList().OrderBy(n => n.id), "id", "ten_phong");
            if (suatChieu == null)
            {
                return HttpNotFound();
            }
            return View(suatChieu);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditShowTime(suat_chieu suatChieu)
        {
            ViewBag.phim = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.phong_chieu = new SelectList(db.phong_chieu.ToList().OrderBy(n => n.id), "id", "ten_phong");
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
            return View(suatChieu);
        }

        public ActionResult DeleteConfirmed(int? id)
        {
            var timesc = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == id).ToList();
            foreach(var item in timesc)
            {
                suatchieu_timeframe sctime = db.suatchieu_timeframe.Find(item.id);
                db.suatchieu_timeframe.Remove(sctime);
                db.SaveChanges();
            }
            suat_chieu suatChieu = db.suat_chieu.Find(id);
            db.suat_chieu.Remove(suatChieu);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListShowTime");
        }

        public ActionResult ShowDataTime(int? idsuatchieu)
        {
            var list = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == idsuatchieu).ToList();
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
            return Json(new { success = true} );
        }


        [HttpGet]
        public ActionResult ShowCreateTimeFr(int? id)
        {
            var time = db.suatchieu_timeframe.Where(x=>x.id_Suatchieu == id);
            var timeframe = db.TimeFrames.ToList();
            List<int> idtimes = new List<int>();
            List<String> times = new List<String>();
            foreach (var item in timeframe)
            {
                var dem = 0;
                foreach(var i in time)
                {
                    if(item.id == i.id_Timeframe)
                    {
                        dem++;
                    }
                }
                if(dem ==0)
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
            return View(db.TimeFrames.ToList());
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