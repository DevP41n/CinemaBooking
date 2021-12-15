﻿using CinemaBooking.Models;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class HomeController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.sk = db.su_kien.FirstOrDefault();
            ViewBag.cinema = db.rap_chieu.Where(x => x.status == 1);
            return View();
        }
        public ActionResult SearchMovie()
        {
            var result = from a in db.phims
                         select new { a.ten_phim, a.anh, a.slug };
            List<phim> film = result.AsEnumerable()
                          .Select(o => new phim
                          {
                              ten_phim = o.ten_phim,
                              anh = o.anh,
                              slug = o.slug
                          }).ToList();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(film, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(value);
        }
        public ActionResult Search(string name)
        {
            if (name == null || name == "")
            {
                TempData["Error"] = "Hãy nhập từ khóa tìm kiếm!";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.tukhoa = name;
            try
            {
                return View(db.phims.Where(p => p.ten_phim.Contains(name)).OrderByDescending(x => x.ngay_cong_chieu));
            }
            catch (Exception)
            {
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult PaySuccess(int? id)
        {
            var order = db.orders.Find(id);
            var orderdetails = db.order_details.Where(n => n.id_orders == id);
            var ghemua = "";
            var dem = 1;
            var count = orderdetails.Count();
            foreach (var item in orderdetails)
            {
                if (dem == count)
                {
                    ghemua += item.ghe_ngoi.Row + item.ghe_ngoi.Col;
                }
                else
                {
                    ghemua += item.ghe_ngoi.Row + item.ghe_ngoi.Col + ", ";
                }
                dem++;
            }
            //Ghế đã mua
            ViewBag.ghedamua = ghemua;
            string path = Server.MapPath("~/images/qrcode/");

            QRCodeData qrCodeData1 = new QRCodeData(path + order.code_ticket + ".qrr", QRCodeData.Compression.Uncompressed);
            QRCode QrCode = new QRCode(qrCodeData1);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            //Ảnh qr code
            ViewBag.QrCodeUri = QrUri;
            return View(order);
        }

        //load film 
        [HttpGet]
        public JsonResult loadFilm(int id)
        {
            List<int> idfilm = new List<int>();
            List<int> idfilmDistinct = new List<int>();
            List<String> filmname = new List<string>();
            var listsc = db.suat_chieu.Where(x => x.phong_chieu.rap_chieu.id == id && x.status == 1).ToList();
            foreach (var item in listsc)
            {
                idfilm.Add(Convert.ToInt32(item.phim_id));
            }
            foreach (var i in idfilm.Distinct())
            {
                phim p = db.phims.Find(i);
                idfilmDistinct.Add(i);
                filmname.Add(p.ten_phim);
            }
            var count = idfilmDistinct.Count();
            return Json(new { idfilm = idfilmDistinct, filmname = filmname, count }, JsonRequestBehavior.AllowGet);
        }

        //load date
        [HttpGet]
        public JsonResult loadDate(int id)
        {
            List<String> date = new List<string>();
            List<int> sc = new List<int>();
            var listsc = db.suat_chieu.Where(x => x.phim_id == id && x.status == 1).ToList();
            foreach (var item in listsc)
            {
                date.Add(Convert.ToDateTime(item.ngay_chieu).ToString("dddd, dd/MM/yyyy"));
                sc.Add(item.id);
            }
            var count = sc.Count();
            return Json(new {idsc = sc, date = date, count }, JsonRequestBehavior.AllowGet);
        }

        //load time
        [HttpGet]
        public JsonResult loadTime(int id)
        {
            List<String> time = new List<string>();
            List<int> idtime = new List<int>();
            var list = db.suatchieu_timeframe.Where(x => x.id_Suatchieu == id).ToList();
            foreach (var item in list)
            {
                idtime.Add(Convert.ToInt32(item.id_Timeframe));
            }
            foreach(var i in idtime)
            {
                var stringtime = db.TimeFrames.Find(i);
                string[] tach = stringtime.Time.ToString().Split(':');
                time.Add(tach[0] + ":" + tach[1]);
            }
            var count = idtime.Count();
            return Json(new { idtime = idtime, time = time, count }, JsonRequestBehavior.AllowGet);
        }


        //check book seat
        [HttpPost]
        public ActionResult checkLogin(string url)
        {
            if (Session["TenCus"] == null)
            {
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = true });
            }
        }
    }
}