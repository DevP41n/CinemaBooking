using CinemaBooking.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
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
            ViewBag.the_loai_phim_id = new SelectList(db.the_loai_phim.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovie(phim Phim)
        {
            ViewBag.the_loai_phim_id = new SelectList(db.the_loai_phim.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
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
            ViewBag.the_loai_phim = new SelectList(db.the_loai_phim.ToList(), "id", "ten_the_loai");
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
            ViewBag.the_loai_phim = new SelectList(db.the_loai_phim.ToList(), "id", "ten_the_loai");
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
            return RedirectToAction("ListMovie");
        }

        // Danh sách thể loại phim
        public ActionResult ListCate()
        {
            //Gọi danh sách thể loại từ bảng thể loại phim và sắp xếp tăng dần theo id
            return View(db.the_loai_phim.OrderByDescending(m => m.id));
        }

        //Thêm thể loại phim
        public ActionResult CreateCate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCate(the_loai_phim TheLoai)
        {
            String strSlug = MyString.ToAscii(TheLoai.ten_the_loai); // Tạo slug cho tên thể loại phim
            TheLoai.slug = strSlug;
            TheLoai.create_at = DateTime.Now;
            TheLoai.update_at = DateTime.Now;
            db.the_loai_phim.Add(TheLoai);
            TempData["Message"] = "Tạo thành công!";
            db.SaveChanges();
            return RedirectToAction("ListCate");
        }

        //Sửa thể loại
        public ActionResult EditCate(int? id)
        {
            the_loai_phim TheLoai = db.the_loai_phim.Find(id);
            if (TheLoai == null)
            {
                return HttpNotFound();
            }
            return View(TheLoai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCate(the_loai_phim TheLoai)
        {
            if (ModelState.IsValid)
            {
                String strSlug = MyString.ToAscii(TheLoai.ten_the_loai); // Tạo slug cho tên thể loại phim
                TheLoai.slug = strSlug;
                TheLoai.update_at = DateTime.Now;
                db.Entry(TheLoai).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListCate");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(TheLoai);
        }

        //Xóa thể loại
        public ActionResult DeleteCate(int id)
        {
            //Tìm kiếm id thể loại phim có tồn tại trong phim nào không
            var del = from dele in db.phims
                      where dele.the_loai_phim_id == id
                      select dele;
            var coundel = del.Count(); //Đếm số lượng id thể loại phim có trong phim

            if (coundel == 0) //Nếu không thì tiến hành xóa thể loại này
            {
                the_loai_phim TheLoai = db.the_loai_phim.Find(id);
                db.the_loai_phim.Remove(TheLoai);
                TempData["Message"] = "Xóa thành công!";
                db.SaveChanges();
            }
            else
            {
                TempData["Warning"] = "Không thể xóa vì đang có phim tồn tại trong menu!";
            }
            return RedirectToAction("ListCate");
        }
    }
}