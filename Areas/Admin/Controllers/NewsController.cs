using CinemaBooking.Models;
using CinemaBooking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/News
        public ActionResult ListNews()
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
            ViewBag.trash = db.su_kien.Where(m => m.status == 0).Count();
            return View(db.su_kien.OrderByDescending(s => s.create_at));
        }
        //Tạo bài viết  
        public ActionResult CreateNews()
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
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNews(su_kien CreateSukien)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(CreateSukien.tieu_de) + numrd + CreateSukien.id;
                CreateSukien.slug = strSlug;
                CreateSukien.create_at = DateTime.Now;
                CreateSukien.update_at = DateTime.Now;
                CreateSukien.status = 1;
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    CreateSukien.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/news/"), filename);
                    file.SaveAs(StrPath);
                }
                db.su_kien.Add(CreateSukien);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListNews");
            }
            return View(CreateSukien);
        }
        //Admin/News/Edit
        public ActionResult EditNews(int? id)
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
            su_kien Sukien = db.su_kien.Find(id);
            if (Sukien == null)
            {
                return RedirectToAction("ListNews", "News");
            }
            return View(Sukien);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(su_kien EditSukien)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(EditSukien.tieu_de) + numrd + EditSukien.id;
                EditSukien.slug = strSlug;
                EditSukien.update_at = DateTime.Now;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    EditSukien.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/news/"));
                    file.SaveAs(Path.Combine(StrPath, filename));
                }
                db.Entry(EditSukien).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListNews");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(EditSukien);
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            su_kien Sukien = db.su_kien.Find(id);
            Sukien.status = (Sukien.status == 1) ? 2 : 1;
            Sukien.update_at = DateTime.Now;
            db.Entry(Sukien).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = Sukien.status });
        }
        public ActionResult DelToTrash(int? id)
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
            su_kien Sukien = db.su_kien.Find(id);
            Sukien.status = 0;
            Sukien.update_at = DateTime.Now;
            db.Entry(Sukien).State = EntityState.Modified;
            TempData["Message"] = "Đã chuyển vào thùng rác!";
            db.SaveChanges();
            return RedirectToAction("ListNews");
        }

        public ActionResult Undo(int? id)
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
            su_kien Sukien = db.su_kien.Find(id);
            Sukien.status = 2;
            Sukien.update_at = DateTime.Now;
            db.Entry(Sukien).State = EntityState.Modified;
            TempData["Message"] = "Khôi phục thành công!";
            db.SaveChanges();
            return RedirectToAction("ListNews");
        }


        // POST: Admin/News/Delete
        public ActionResult DeleteConfirmed(int id)
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
            su_kien Sukien = db.su_kien.Find(id);
            db.su_kien.Remove(Sukien);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListNews");
        }
    }
}