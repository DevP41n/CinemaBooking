using CinemaBooking.Models;
using CinemaBooking;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Text.RegularExpressions;

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
            ViewBag.dao_dien_id = new SelectList(db.dao_dien.ToList().OrderBy(n => n.id), "id", "ho_ten");
            ViewBag.dien_vien_id = new SelectList(db.dien_vien.ToList().OrderBy(n => n.id), "id", "ho_ten");
            ViewBag.id_content_rating = new SelectList(db.content_rating.ToList().OrderBy(n => n.ID), "ID", "ten_rating");
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovie(phim Phim,list_phim_dienvien listdienvien,list_phim_theloai listtheloai,String[] theloaiarray,String[] dienvienarray)
        {
            ViewBag.the_loai_phim_id = new SelectList(db.the_loai_phim.ToList().OrderBy(n => n.id), "id", "ten_the_loai");
            ViewBag.dao_dien_id = new SelectList(db.dao_dien.ToList().OrderBy(n => n.id), "id", "ho_ten");
            ViewBag.dien_vien_id = new SelectList(db.dien_vien.ToList().OrderBy(n => n.id), "id", "ho_ten");
            ViewBag.id_content_rating = new SelectList(db.content_rating.ToList().OrderBy(n => n.ID), "ID", "ten_rating");
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                string except = " ";
                string strResult = Regex.Replace(Phim.ten_phim, @"[^a-zA-Z0-9" + except + "]+", string.Empty);
                string strSlug = MyString.ToAscii(strResult) + numrd + Phim.id;
                Phim.slug = strSlug;
                Phim.create_at = DateTime.Now;
                Phim.update_at = DateTime.Now;
                Phim.status = 2;
                Phim.loai_phim_chieu = 2;
                //Upload File
                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {

                    string filename = strSlug + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    Phim.anh = filename;
                    string path = Server.MapPath("~/images/movies/");
                    string StrPath = Path.Combine(path, filename);
                    file.SaveAs(StrPath);
                }
                var file1 = Request.Files["anhbackground"];
                if (file1 != null && file1.ContentLength > 0)
                {
                    String filename = strSlug + file1.FileName.Substring(file1.FileName.LastIndexOf("."));
                    Phim.anhbackground = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/movies/background/"));
                    file1.SaveAs(Path.Combine(StrPath, filename));
                }
                string theloai = "";
                foreach (string theloaiid in theloaiarray)
                {
                    the_loai_phim selecttheloai = db.the_loai_phim.ToList().Find(p => p.id.ToString() == theloaiid);
                    theloai += selecttheloai.id;
                }
                int theloailist = Int32.Parse(theloai);
                Phim.theloaichinh = theloailist;
                db.phims.Add(Phim);
                db.SaveChanges();
                TempData["Message"] = "Tạo thành công!";

                foreach (string dienvienid in dienvienarray)
                {
                    dien_vien selectdienvien = db.dien_vien.ToList().Find(p => p.id.ToString() == dienvienid);
                    listdienvien.id_phim = Phim.id;
                    listdienvien.id_dienvien = selectdienvien.id;
                    db.list_phim_dienvien.Add(listdienvien);
                    db.SaveChanges();
                }
                
                foreach (string theloaiid in theloaiarray)
                {
                    the_loai_phim selecttheloai = db.the_loai_phim.ToList().Find(p => p.id.ToString() == theloaiid);
                    listtheloai.id_phim = Phim.id;
                    listtheloai.id_theloai = selecttheloai.id;
                    db.list_phim_theloai.Add(listtheloai);
                    db.SaveChanges();
                }
                return RedirectToAction("ListMovie");
            }
            return View(Phim);
        }
        //Edit phim mới
        public ActionResult EditMovie(int? id)
        {
            ViewBag.the_loai_phim_id = new MultiSelectList(db.the_loai_phim.ToList(), "id", "ten_the_loai");
            ViewBag.dao_dien_id = new SelectList(db.dao_dien.ToList().OrderBy(n => n.id), "id", "ho_ten");
            ViewBag.dien_vien_id = new SelectList(db.dien_vien.ToList(), "id", "ho_ten");
            ViewBag.id_content_rating = new SelectList(db.content_rating.ToList().OrderBy(n => n.ID), "ID", "ten_rating");
            phim Phim = db.phims.Find(id);
            if (Phim == null)
            {
                return RedirectToAction("ListMovie", "Movie");
            }
            return View(Phim);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditMovie(phim Phim, list_phim_dienvien listdienvien, list_phim_theloai listtheloai, String[] theloaiarray, String[] dienvienarray)
        {
            ViewBag.the_loai_phim_id = new SelectList(db.the_loai_phim.ToList(), "id", "ten_the_loai");
            ViewBag.dao_dien_id = new SelectList(db.dao_dien.ToList().OrderBy(n => n.id), "id", "ho_ten");
            ViewBag.dien_vien_id = new SelectList(db.dien_vien.ToList(), "id", "ho_ten");
            ViewBag.id_content_rating = new SelectList(db.content_rating.ToList().OrderBy(n => n.ID), "ID", "ten_rating");
            if (ModelState.IsValid)
            {
                Random rd = new Random();
                var numrd = rd.Next(1, 100).ToString();
                string except = " ";
                string strResult = Regex.Replace(Phim.ten_phim, @"[^a-zA-Z0-9" + except + "]+", string.Empty);
                String strSlug = MyString.ToAscii(strResult) + numrd + Phim.id;
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
                var file1 = Request.Files["anhbackground"];
                if (file1 != null && file1.ContentLength > 0)
                {
                    String filename = strSlug + file1.FileName.Substring(file1.FileName.LastIndexOf("."));
                    Phim.anhbackground = filename;
                    String StrPath = Path.Combine(Server.MapPath("~/images/movies/background/"));
                    file1.SaveAs(Path.Combine(StrPath, filename));
                }
                if (theloaiarray != null)
                {
                    string theloai = "";
                    foreach (string theloaiid in theloaiarray)
                    {
                        the_loai_phim selecttheloai = db.the_loai_phim.ToList().Find(p => p.id.ToString() == theloaiid);
                        theloai += selecttheloai.id;
                    }
                    int theloailist = Int32.Parse(theloai);
                    Phim.theloaichinh = theloailist;
                }
                db.Entry(Phim).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();

                //Edit dien vien
                if (dienvienarray != null)
                {
                    foreach (var item in db.list_phim_dienvien.Where(x=>x.id_phim==Phim.id).ToList())
                    {
                        db.list_phim_dienvien.Remove(item);
                        db.SaveChanges();
                    }
                    foreach (string dienvienid in dienvienarray)
                    {
                        dien_vien selectdienvien = db.dien_vien.ToList().Find(p => p.id.ToString() == dienvienid);
                        listdienvien.id_phim = Phim.id;
                        listdienvien.id_dienvien = selectdienvien.id;
                        db.list_phim_dienvien.Add(listdienvien);
                        db.SaveChanges();
                    }
                }
                //Edit the loai
                if (theloaiarray != null)
                {
                    foreach (var item in db.list_phim_theloai.Where(x => x.id_phim == Phim.id).ToList())
                    {
                        db.list_phim_theloai.Remove(item);
                        db.SaveChanges();
                    }
                    foreach (string theloaiid in theloaiarray)
                    {
                        the_loai_phim selecttheloai = db.the_loai_phim.ToList().Find(p => p.id.ToString() == theloaiid);
                        listtheloai.id_phim = Phim.id;
                        listtheloai.id_theloai = selecttheloai.id;
                        db.list_phim_theloai.Add(listtheloai);
                        db.SaveChanges();
                    }
                }
                
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
        [HttpPost]
        public JsonResult ChangeRelease(int id)
        {
            phim Phim = db.phims.Find(id);
            Phim.loai_phim_chieu = (Phim.loai_phim_chieu == 1) ? 2 : 1;
            db.Entry(Phim).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { Status = Phim.loai_phim_chieu });
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
            var list = db.list_phim_theloai.Where(x => x.id_phim == id).ToList() ;
            foreach (list_phim_theloai item in list)
            {
                db.list_phim_theloai.Remove(item);
                db.SaveChanges();
            }
            var list1 = db.list_phim_dienvien.Where(x => x.id_phim == id).ToList();
            foreach (list_phim_dienvien item1 in list1)
            {
                db.list_phim_dienvien.Remove(item1);
                db.SaveChanges();
            }
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
            var del = from dele in db.list_phim_theloai
                      where dele.id_theloai == id
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
                TempData["Warning"] = "Không thể xóa vì đang có phim tồn tại trong mục này!";
            }
            return RedirectToAction("ListCate");
        }



        //Đánh giá nội dung phim
        public ActionResult ListContentRating()
        {
            return View(db.content_rating.OrderByDescending(m => m.ID));
        }

        //Thêm loại
        public ActionResult CreateContentRating()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateContentRating(content_rating ctRating)
        {

            var del = from dele in db.content_rating
                      where dele.ID == ctRating.ID
                      select dele;
            var coundel = del.Count();

            if (coundel == 0)
            {
                db.content_rating.Add(ctRating);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
            }
            else
            {
                TempData["Warning"] = "Không thể tạo vì loại id này đã tồn tại";
            }
            return RedirectToAction("ListContentRating");
        }

        //Sửa loại đánh giá nội dung
        public ActionResult EditContentRating(int? id)
        {
            content_rating ctRating = db.content_rating.Find(id);
            if (ctRating == null)
            {
                return HttpNotFound();
            }
            return View(ctRating);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditContentRating(content_rating ctRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ctRating).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListContentRating");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(ctRating);
        }

        //Xóa loại đánh giá nội dung
        public ActionResult DeleteContentRating(int id)
        {
            var del = from dele in db.phims
                      where dele.id_content_rating == id
                      select dele;
            var coundel = del.Count();

            if (coundel == 0)
            {
                content_rating ctRating = db.content_rating.Find(id);
                db.content_rating.Remove(ctRating);
                TempData["Message"] = "Xóa thành công!";
                db.SaveChanges();
            }
            else
            {
                TempData["Warning"] = "Không thể xóa vì vẫn có phim tồn tại loại này";
            }
            return RedirectToAction("ListContentRating");
        }
    }
}