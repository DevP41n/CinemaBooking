using CinemaBooking.Models;
using HyperGear;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/Movie
        //list phim
        public ActionResult ListMovie()
        {
            ViewBag.trash = db.phims.Where(m => m.status == 0).Count();
            return View(db.phims.OrderByDescending(m => m.id).ToList());
        }
        //Tạo phim mới
        public ActionResult CreateMovie()
        {
            ViewBag.theloai = new SelectList(db.the_loai_phim.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovie(phim Phim)
        {
            ViewBag.theloai = new SelectList(db.the_loai_phim.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(Phim.ten_phim) + numrd + Phim.id;
                Phim.slug = strSlug;
                Phim.create_at = DateTime.Now;
                Phim.update_at = DateTime.Now;
                Phim.status = 1;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    Phim.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/movies/"), filename);
                    file.SaveAs(StrPath);
                }
                db.phims.Add(Phim);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListMovie");
            }
            return View(Phim);
        }
        //Edit phim mới
        public ActionResult EditMovie(int? id)
        {
            ViewBag.Movie = new SelectList(db.the_loai_phim.ToList(), "id", "ten_the_loai");
            phim Phim = db.phims.Find(id);
            if (Phim == null)
            {
                return RedirectToAction("ListMovie", "Movie");
            }
            return View(Phim);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditMovie(phim Phim)
        {
            ViewBag.Movie = new SelectList(db.the_loai_phim.ToList(), "id", "ten_the_loai");
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(Phim.ten_phim) + numrd + Phim.id;
                Phim.slug = strSlug;
                Phim.update_at = DateTime.Now;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    Phim.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/movies/"));
                    file.SaveAs(Path.Combine(StrPath, filename));
                }
                db.Entry(Phim).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListMovie");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(Phim);
        }

        [HttpPost]
        public JsonResult changeStatus(int id)
        {
            phim Phim = db.phims.Find(id);
            Phim.status = (Phim.status == 1) ? 2 : 1;
            //if (mProduct.Status==1)
            //{
            //    mProduct.Status = 2;
            //}
            //else
            //{
            //    mProduct.Status = 1;
            //}

            Phim.update_at = DateTime.Now;
            //Phim.update_by = (Session["Username"].ToString());
            db.Entry(Phim).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = Phim.status });
        }
        public ActionResult DelToTrash(int? id)
        {
            //if (Session["HoTen"] == null)
            //{
            //    return RedirectToAction("Login", "Auth");
            //}
            phim Phim = db.phims.Find(id);
            Phim.status = 0;

            Phim.update_at = DateTime.Now;
            //Phim.update_by = (Session["Username"].ToString());
            db.Entry(Phim).State = EntityState.Modified;
            TempData["Message"] = "Đã chuyển vào thùng rác!";
            db.SaveChanges();
            return RedirectToAction("ListMovie");
        }

        public ActionResult Undo(int? id)
        {
            //if (Session["HoTen"] == null)
            //{
            //    return RedirectToAction("Login", "Auth");
            //}
            phim Phim = db.phims.Find(id);
            Phim.status = 2;

            Phim.update_at = DateTime.Now;
            //Phim.update_by = (Session["Username"].ToString());
            db.Entry(Phim).State = EntityState.Modified;
            TempData["Message"] = "Khôi phục thành công!";
            db.SaveChanges();
            return RedirectToAction("ListMovie");
        }


        // POST: Admin/Product/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            //if (Session["HoTen"] == null)
            //{
            //    return RedirectToAction("Login", "Auth");
            //}
            phim Phim = db.phims.Find(id);
            db.phims.Remove(Phim);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("List");
        }
    }
}