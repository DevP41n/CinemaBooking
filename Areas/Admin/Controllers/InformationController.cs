using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class InformationController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/Information
        public ActionResult ListActor()
        {
            return View(db.dien_vien.OrderByDescending(s => s.ho_ten));
        }
        public ActionResult ListDirector()
        {
            return View(db.dao_dien.OrderByDescending(s => s.ho_ten));
        }
        //Tạo dien vien /dao dien
        public ActionResult CreateActor()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateActor(dien_vien CreateActor)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(CreateActor.ho_ten) + numrd + CreateActor.id;
                CreateActor.slug = strSlug;
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    CreateActor.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/information/"), filename);
                    file.SaveAs(StrPath);
                }
                db.dien_vien.Add(CreateActor);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListActor");
            }
            return View(CreateActor);
        }

        public ActionResult CreateDirector()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDirector(dao_dien CreateDirector)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(CreateDirector.ho_ten) + numrd + CreateDirector.id;
                CreateDirector.slug = strSlug;
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    CreateDirector.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/information/"), filename);
                    file.SaveAs(StrPath);
                }
                db.dao_dien.Add(CreateDirector);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListDirector");
            }
            return View(CreateDirector);
        }
        //Admin/Information/Edit dien vien/ dao dien
        public ActionResult EditActor(int? id)
        {
            dien_vien dienvien = db.dien_vien.Find(id);
            if (dienvien == null)
            {
                return RedirectToAction("ListActor", "Information");
            }
            return View(dienvien);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditActor(dien_vien EditActor)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(EditActor.ho_ten) + numrd + EditActor.id;
                EditActor.slug = strSlug;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    EditActor.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/information/"));
                    file.SaveAs(Path.Combine(StrPath, filename));
                }
                db.Entry(EditActor).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListActor");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(EditActor);
        }

        public ActionResult EditDirector(int? id)
        {
            dao_dien daodien = db.dao_dien.Find(id);
            if (daodien == null)
            {
                return RedirectToAction("ListDirector", "Information");
            }
            return View(daodien);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditDirector(dao_dien EditDirector)
        {
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                String strSlug = MyString.ToAscii(EditDirector.ho_ten) + numrd + EditDirector.id;
                EditDirector.slug = strSlug;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    String filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    EditDirector.anh = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/information/"));
                    file.SaveAs(Path.Combine(StrPath, filename));
                }
                db.Entry(EditDirector).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListDirector");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(EditDirector);
        }
        // POST: Admin/Information/Delete
        public ActionResult DeleteActorConfirmed(int id)
        {
            dien_vien dienvien = db.dien_vien.Find(id);
            db.dien_vien.Remove(dienvien);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListActor");
        }
        public ActionResult DeleteDirectorConfirmed(int id)
        {
            dao_dien daodien= db.dao_dien.Find(id);
            db.dao_dien.Remove(daodien);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListDirector");
        }
    }
}