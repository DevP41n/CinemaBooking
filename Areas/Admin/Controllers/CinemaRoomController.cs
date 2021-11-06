using CinemaBooking.Models;
using System;
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
                TempData["Error"] = "Cập nhật không thành công!";
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

        public ActionResult SeatRoom(int? id)
        {
            phong_chieu phongChieu = db.phong_chieu.Find(id);
            ViewBag.pc = phongChieu;
            var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == id).ToList();
            ViewBag.ghe = ghengoi;
            return View(ghengoi);
        }

        [HttpPost]
        public ActionResult CreateSeat(string hang, int ghe, int id)
        {
            phong_chieu phongChieu = db.phong_chieu.Find(id);
            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id).Count();
            ghe_ngoi ghengoi = new ghe_ngoi();
            if (ghn != 0)
            {
                return Json(new { success = false });
            }
            else
            {
                try
                {
                    for (int i = 0; i < ghe; i++)
                    {
                        ghengoi.Row = hang;
                        ghengoi.Col = i + 1;
                        ghengoi.phong_chieu_id = id;
                        db.ghe_ngoi.Add(ghengoi);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
            }
            return Json(new { success = true });
        }

        public ActionResult GetByRow(string Row, int? pcid)
        {
            var seat = db.ghe_ngoi.Where(x => x.Row == Row && x.phong_chieu_id == pcid).Count();
            return Json(data: new { seat, Row }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditSeat(string hang, int ghe, int id)
        {
            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id).ToList();
            var tong = ghn.Count();
            ghe_ngoi ghengoi = new ghe_ngoi();
            try
            {
                if (tong < ghe)
                {
                    for (int i = tong; i <ghe; i++)
                    {
                        ghengoi.Row = hang;
                        ghengoi.Col = i + 1;
                        ghengoi.phong_chieu_id = id;
                        db.ghe_ngoi.Add(ghengoi);
                        db.SaveChanges();
                    }
                }
                else if(tong > ghe)
                {
                    foreach(var item in ghn)
                    {
                        if(item.Col>ghe)
                        {
                            ghe_ngoi seat = db.ghe_ngoi.Find(item.id);
                            db.ghe_ngoi.Remove(seat);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }


        [HttpPost]
        public JsonResult DeleteSeat(string hang, int id)
        {
            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id).ToList();
            ghe_ngoi ghengoi = new ghe_ngoi();
            try
            {
                foreach(var item in ghn)
                {
                    ghe_ngoi seat = db.ghe_ngoi.Find(item.id);
                    db.ghe_ngoi.Remove(seat);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }
    }
}