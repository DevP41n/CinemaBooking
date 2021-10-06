using CinemaBooking.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class CinemaRoomController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/CinemaRoom
        public ActionResult ListCinemaRoom()
        {
            return View(db.phong_chieu.OrderByDescending(m => m.id));
        }

        //Thêm phòng chiếu mới
        public ActionResult CreateCinemaRoom()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCinemaRoom(phong_chieu phongChieu)
        {
            db.phong_chieu.Add(phongChieu);
            db.SaveChanges();
            TempData["Message"] = "Tạo thành công!";
            return RedirectToAction("ListCinemaRoom");
        }

        public ActionResult EditCinemaRoom(int? id)
        {
            phong_chieu phongChieu = db.phong_chieu.Find(id);
            if (phongChieu == null)
            {
                return HttpNotFound();
            }
            return View(phongChieu);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCinemaRoom(phong_chieu phongChieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListCinemaRoom");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(phongChieu);
        }

        public ActionResult DeleteCinemaRoom(int? id)
        {
            phong_chieu phongChieu = db.phong_chieu.Find(id);
            db.phong_chieu.Remove(phongChieu);
            db.SaveChanges();
            TempData["Message"] = "Xóa thành công!";
            db.SaveChanges();
            return RedirectToAction("ListCinemaRoom");
        }


    }
}