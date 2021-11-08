using CinemaBooking.Models;
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
            ViewBag.phim_id = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.phong_chieu_id = new SelectList(db.phong_chieu.ToList().OrderBy(n => n.id), "id", "ten_phong");

            return View();
        }
        [HttpPost]
        public ActionResult CreateShowTime(suat_chieu suatChieu)
        {
            ViewBag.phim_id = new SelectList(db.phims.ToList().OrderBy(n => n.id), "id", "ten_phim");
            ViewBag.phong_chieu_id = new SelectList(db.phong_chieu.ToList().OrderBy(n => n.id), "id", "ten_phong");
            if (ModelState.IsValid)
            {
                db.suat_chieu.Add(suatChieu);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
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
            suat_chieu suatChieu = db.suat_chieu.Find(id);
            db.suat_chieu.Remove(suatChieu);
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListShowTime");
        }

    }
}