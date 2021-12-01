using CinemaBooking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class HomeController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Home
        public ActionResult Index()
        {
            // dòng này để sql ko bị null. có thể xóa
            foreach (var sc in db.suat_chieu.Where(s => s.status == null))
            {
                    suat_chieu suatchieu = db.suat_chieu.Find(sc.id);
                    suatchieu.status = "1";
                    db.Entry(suatchieu).State = EntityState.Modified;
            }
            db.SaveChanges();
            //kiem tra trang thai cua suat chieu ( theo date)
            foreach (var sc in db.suat_chieu.Where(s => s.status == "1"))
            {
                if (sc.ngay_chieu <= DateTime.Today)
                {
                    suat_chieu suatchieu = db.suat_chieu.Find(sc.id);
                    suatchieu.status = "2";
                    db.Entry(suatchieu).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            foreach (var sc in db.suat_chieu.Where(s => s.status == "2"))
            {
                if (sc.ngay_chieu > DateTime.Today)
                {
                    suat_chieu suatchieu = db.suat_chieu.Find(sc.id);
                    suatchieu.status = "1";
                    db.Entry(suatchieu).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            int count = 0;
            // phim: công chiếu phim nếu đã có suất chiếu 
            foreach (var item in db.phims.Where(x => x.loai_phim_chieu == 2 && x.status == 1))
            {
                if (item.ngay_cong_chieu <= DateTime.Today)
                {
                    foreach (var sc in db.suat_chieu.Where(s => s.phim_id == item.id))
                    {
                        if(sc.status == "1")
                        {
                            phim Phim = db.phims.Find(item.id);
                            Phim.loai_phim_chieu = 1;
                            db.Entry(Phim).State = EntityState.Modified;
                        }
                    }
                }
            }
            db.SaveChanges();   
            //phim: nếu ko có suất nào thì vẫn để trong sắp chiếu và thông báo chưa có suất
            foreach (var item in db.phims.Where(x => x.loai_phim_chieu == 1 && x.status == 1))
            {
                if (item.ngay_cong_chieu <= DateTime.Today)
                {
                    if(db.suat_chieu.Any(s => s.phim_id == item.id))
                    {

                    }
                    else
                    {
                        phim Phim = db.phims.Find(item.id);
                        Phim.loai_phim_chieu = 2;
                        db.Entry(Phim).State = EntityState.Modified;
                    }
                    
                }
            }
            db.SaveChanges();
            //kiểm tra có phim nào lỗi chiếu sớm thì tắt
            foreach (var item in db.phims.Where(x => x.loai_phim_chieu == 1))
            {
                if (item.ngay_cong_chieu > DateTime.Today)
                {
                    phim Phim = db.phims.Find(item.id);
                    Phim.loai_phim_chieu = 2;
                    db.Entry(Phim).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            //Kiểm tra xem những phim nào đã hết suất chiếu thì ẩn đi
            foreach (var item1 in db.phims.Where(x => x.loai_phim_chieu == 1 && x.status==1))
            {
                count = 0;
                if (item1.ngay_cong_chieu <= DateTime.Today)
                {
                    int countsc = db.suat_chieu.Where(s => s.phim_id == item1.id).Count();
                    foreach (var sc in db.suat_chieu.Where(s => s.phim_id == item1.id))
                    {
                        if (sc.status == "2")
                        {
                            count+=1;
                        }
                        if (countsc == count)
                        {
                            phim Phim = db.phims.Find(item1.id);
                            Phim.loai_phim_chieu = 2;
                            Phim.status = 0;
                            db.Entry(Phim).State = EntityState.Modified;
                        }

                    }


                }
            }
            db.SaveChanges();
            ViewBag.sk = db.su_kien.Find(1);
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
            ViewBag.tukhoa = name;
            return View(db.phims.Where(p => p.ten_phim.Contains(name)).OrderByDescending(x => x.ten_phim));
        }
    }
}