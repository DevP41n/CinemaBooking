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
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.khd = db.phong_chieu.Where(x => x.status == 2).Count();
            return View(db.phong_chieu.OrderByDescending(m => m.id));
        }

        //Thêm phòng chiếu mới
        public ActionResult CreateCinemaRoom()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
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
            phongChieu.status = 1;
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
                    ghe.status = 1;
                    db.ghe_ngoi.Add(ghe);
                    db.SaveChanges();
                }
            }
            TempData["Message"] = "Tạo thành công!";
            return RedirectToAction("ListCinemaRoom");
        }

        public ActionResult EditCinemaRoom(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {
                ViewBag.id_rapc = new SelectList(db.rap_chieu.ToList().OrderBy(n => n.id), "id", "ten_rap");
                phong_chieu phongChieu = db.phong_chieu.Find(id);
                // status = 0 là đã xóa
                if (phongChieu == null || phongChieu.status == 0)
                {
                    return RedirectToAction("AError404","Admin");
                }
                return View(phongChieu);
            }catch(Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
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

            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {
                phong_chieu phongChieu = db.phong_chieu.Find(id);
                if(phongChieu.status == 1 || phongChieu.status == 0)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                // bằng 0 thì đã dừng hoạt động
                var check = db.suat_chieu.Where(x => x.phong_chieu_id == id && x.status == 1);
                TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
                int dem = 0;
                foreach (var item in check)
                {
                    if(item.ngay_chieu + timecheck > DateTime.Now)
                    {
                        dem++;
                    }

                }

                if(dem >0)
                {
                    TempData["Warning"] = "Hiện tại không thể xóa vì phòng này tồn tại suất chiếu hoặc chưa hết ngày chiếu!";
                    return RedirectToAction("ListCinemaRoom");
                }

                //Xóa = ẩn nó đi tại vì nó ràng buộc nhiều, không thể xóa
                phongChieu.status = 0;
                db.Entry(phongChieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Xóa thành công!";
                return RedirectToAction("ListCinemaRoom");
            }
            catch(Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }

        //sửa status room
        public ActionResult changeStatusRoom(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {

                phong_chieu phong_Chieu = db.phong_chieu.Find(id);
                var check = db.suat_chieu.Where(x => x.phong_chieu_id == id && x.status == 1);
                TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
                int dem = 0;
                foreach (var item in check)
                {
                    if (item.ngay_chieu + timecheck > DateTime.Now)
                    {
                        dem++;
                    }

                }

                if (dem > 0)
                {
                    TempData["Warning"] = "Hiện tại không thể dừng hoạt động vì phòng này tồn tại suất chiếu hoặc chưa hết ngày chiếu!";
                    return RedirectToAction("ListCinemaRoom");
                }

                //Nếu không phải = 1 là sai
                if (phong_Chieu.status != 1)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                phong_Chieu.status = 2;
                db.Entry(phong_Chieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Dừng hoạt động thành công";
                return RedirectToAction("ListCinemaRoom");
            }catch(Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }

        //Undo phòng không hoạt động
        public ActionResult UndoStatusRoom(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }

            try
            {

                phong_chieu phong_Chieu = db.phong_chieu.Find(id);
               

                //Nếu không phải = 2 là sai
                if (phong_Chieu.status != 2)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                phong_Chieu.status = 1;
                db.Entry(phong_Chieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Đã bật trạng thái hoạt động";
                return RedirectToAction("ListCinemaRoom");
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }

        public ActionResult SeatRoom(int? id)
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (id == null || id <1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {
                phong_chieu phongChieu = db.phong_chieu.Find(id);
                if(phongChieu.status == 0)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                ViewBag.pc = phongChieu;
                var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == id & x.status == 1).OrderBy(x => x.Row);
                ViewBag.ghe = ghengoi;
                return View(ghengoi);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404","Admin");
            }
        }

        [HttpPost]
        public ActionResult CreateSeat(string hang, int ghe, int id)
        {
            var check = db.suat_chieu.Where(x => x.phong_chieu_id == id && x.status == 1);
            TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
            int dem = 0;
            foreach (var item in check)
            {
                if (item.ngay_chieu + timecheck > DateTime.Now)
                {
                    dem++;
                }

            }
            //không cho thêm thêm nếu có suất chiếu
            if (dem > 0)
            {
                return Json(new { checkr = false });
            }
            //check xem phòng đã xóa chưa
            phong_chieu phongChieu = db.phong_chieu.Find(id);
            if(phongChieu.status == 0)
            {
                return Json(new { checkr = false });
            }
            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id && n.status == 1).Count();
            ghe_ngoi ghengoi = new ghe_ngoi();
            if (ghn != 0)
            {
                return Json(new { success = false });
            }
            else
            {
                var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
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
                        ghengoi.status = 1;
                        db.ghe_ngoi.Add(ghengoi);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    return Json(new { checkr = false });
                }
            }
            return Json(new { success = true });
        }

        public ActionResult GetByRow(string Row, int? pcid)
        {
            var seat = db.ghe_ngoi.Where(x => x.Row == Row && x.phong_chieu_id == pcid && x.status == 1).Count();
            return Json(data: new { seat, Row }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditSeat(string hang, int ghe, int id)
        {
            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id && n.status == 1).ToList();
            var tong = ghn.Count();
            ghe_ngoi ghengoi = new ghe_ngoi();
            var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
            var checkpc = db.phong_chieu.Find(id);
            if(checkpc.status == 0)
            {
                return Json(new { success = false });
            }

            var check = db.suat_chieu.Where(x => x.phong_chieu_id == id && x.status == 1);
            TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
            int dem = 0;
            foreach (var item in check)
            {
                // check suất chiếu xem hết chưa
                if (item.ngay_chieu + timecheck > DateTime.Now)
                {
                    dem++;
                }

            }
            //không cho thêm sửa nếu có suất chiếu
            if(dem >0)
            {
                return Json(new { success = false });
            }


            try
            {
                if (tong < ghe)
                {
                    checkpc.so_luong_cot = checkghe.Count() + (ghe - tong);
                    db.Entry(checkpc).State = EntityState.Modified;                    
                    for (int i = tong; i < ghe; i++)
                    {
                        ghengoi.Row = hang;
                        ghengoi.Col = i + 1;
                        ghengoi.phong_chieu_id = id;
                        ghengoi.status = 1;
                        db.ghe_ngoi.Add(ghengoi);
                        db.SaveChanges();
                    }
                    db.SaveChanges();
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
                            seat.status = 0;
                            db.Entry(seat).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    db.SaveChanges();
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

            var check = db.suat_chieu.Where(x => x.phong_chieu_id == id && x.status == 1);
            TimeSpan timecheck = new TimeSpan(1, 0, 0, 0);
            int dem = 0;
            foreach (var item in check)
            {
                // check suất chiếu xem hết chưa
                if (item.ngay_chieu + timecheck > DateTime.Now)
                {
                    dem++;
                }

            }
            //không cho thêm sửa nếu có suất chiếu
            if (dem > 0)
            {
                return Json(new { success = false });
            }

            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id && n.status == 1).ToList();  
            ghe_ngoi ghengoi = new ghe_ngoi();
            var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
            var checkpc = db.phong_chieu.Find(id);
            if(checkpc.status == 0)
            {
                return Json(new { success = false });
            }
            try
            {
                foreach (var item in ghn)
                {
                    ghe_ngoi seat = db.ghe_ngoi.Find(item.id);
                    seat.status = 0;
                    db.Entry(seat).State = EntityState.Modified;
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
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(db.rap_chieu.OrderByDescending(n => n.id).ToList());
        }

        //Tạo rạp chiếu phim
        public ActionResult CreateCinema()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
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
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {

                rap_chieu rapChieu = db.rap_chieu.Find(id);
                if (rapChieu == null)
                {
                    return RedirectToAction("CinemaList");
                }
                return View(rapChieu);
            }
            catch(Exception)
            {
                    return RedirectToAction("AError404", "Admin");
            }
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
                TempData["Error"] = "Không thể xóa vì đang có phòng chiếu tại nơi này!";
                return RedirectToAction("CinemaList");
            }
        }
    }
}