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
            ViewBag.id_rapchieu = new SelectList(db.rap_chieu.ToList().OrderBy(n => n.id), "id", "ten_rap");
            return View();
        }
        [HttpPost]
        public ActionResult CreateCinemaRoom(phong_chieu phongChieu)
        {
            ViewBag.id_rapchieu = new SelectList(db.rap_chieu.ToList().OrderBy(n => n.id), "id", "ten_rap");
            string[] room = new string[5] { "A", "B", "C", "D", "E" };
            ghe_ngoi ghe = new ghe_ngoi();
            phongChieu.so_luong_cot = 10;
            phongChieu.so_luong_day = 5;
            db.phong_chieu.Add(phongChieu);
            db.SaveChanges();
            var id = phongChieu.id;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    ghe.Row = room[i];
                    ghe.Col = j + 1;
                    ghe.phong_chieu_id = id;
                    db.ghe_ngoi.Add(ghe);
                    db.SaveChanges();
                }
            }
            TempData["Message"] = "Tạo thành công!";
            return RedirectToAction("ListCinemaRoom");
        }

        public ActionResult EditCinemaRoom(int? id)
        {
            ViewBag.id_rapc = new SelectList(db.rap_chieu.ToList().OrderBy(n => n.id), "id", "ten_rap");
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
            ViewBag.id_rapc = new SelectList(db.rap_chieu.ToList().OrderBy(n => n.id), "id", "ten_rap");
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
            var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == id).OrderBy(x=>x.Row);
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
                var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id).ToList();
                var pc = db.phong_chieu.Find(id);
                pc.so_luong_cot = checkghe.Count() + ghe;
                db.Entry(pc).State = EntityState.Modified;
                db.SaveChanges();
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
            var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id).ToList();
            var checkpc = db.phong_chieu.Find(id);
            try
            {
                if (tong < ghe)
                {
                    checkpc.so_luong_cot = checkghe.Count() + (ghe - tong);
                    db.Entry(checkpc).State = EntityState.Modified;
                    db.SaveChanges();
                    for (int i = tong; i < ghe; i++)
                    {
                        ghengoi.Row = hang;
                        ghengoi.Col = i + 1;
                        ghengoi.phong_chieu_id = id;
                        db.ghe_ngoi.Add(ghengoi);
                        db.SaveChanges();
                    }
                }
                else if (tong > ghe)
                {
                    checkpc.so_luong_cot = checkghe.Count() - (tong - ghe);
                    db.Entry(checkpc).State = EntityState.Modified;
                    foreach (var item in ghn)
                    {
                        if (item.Col > ghe)
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
            var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id).ToList();
            var checkpc = db.phong_chieu.Find(id);
            try
            {
                foreach (var item in ghn)
                {
                    ghe_ngoi seat = db.ghe_ngoi.Find(item.id);
                    db.ghe_ngoi.Remove(seat);
                    db.SaveChanges();
                }
                checkpc.so_luong_cot = checkghe.Count() - ghn.Count();
                db.Entry(checkpc).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        //Danh sách các rạp chiếu
        public ActionResult CinemaList()
        {
            return View(db.rap_chieu.OrderByDescending(n => n.id).ToList());
        }

        //Tạo rạp chiếu phim
        public ActionResult CreateCinema()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCinema(rap_chieu rapChieu)
        {
            if (ModelState.IsValid)
            {
                rapChieu.create_at = DateTime.Now;
                //rapChieu.create_by = Session["HoTen"].ToString();
                db.rap_chieu.Add(rapChieu);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("CinemaList");
            }
            return View(rapChieu);
        }
        public ActionResult EditCinema(int? id)
        {
            rap_chieu rapChieu = db.rap_chieu.Find(id);
            if (rapChieu == null)
            {
                return RedirectToAction("CinemaList");
            }
            return View(rapChieu);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditCinema(rap_chieu rapChieu)
        {
            if (ModelState.IsValid)
            {
                rapChieu.update_at = DateTime.Now;
                //rapChieu.update_by = Session["HoTen"].ToString();

                db.Entry(rapChieu).State = EntityState.Modified;
                TempData["Message"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("CinemaList");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(rapChieu);
        }


        public ActionResult DeleteCinema(int? id)
        {
            if (db.phong_chieu.Where(n => n.id_rapchieu == id).ToList().Count() == 0)
            {
                rap_chieu rapChieu = db.rap_chieu.Find(id);
                db.rap_chieu.Remove(rapChieu);
                TempData["Message"] = "Xóa thành công!";
                db.SaveChanges();
                return RedirectToAction("CinemaList");
            }
            else
            {
                TempData["Error"] = "Không thể xóa !";
                return RedirectToAction("CinemaList");
            }
        }
    }
}