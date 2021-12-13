using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
            if (ModelState.IsValid)
            {
                ViewBag.id_rapchieu = new SelectList(db.rap_chieu.ToList().OrderBy(n => n.id), "id", "ten_rap");
                //Lấy ra phụ thu của loại ghế nhỏ nhất để tạo phòng có giá default
                var loaighe = db.loai_ghe.OrderBy(x => x.phu_thu).FirstOrDefault();
                string[] room = new string[5] { "A", "B", "C", "D", "E" };
                ghe_ngoi ghe = new ghe_ngoi();
                phongChieu.so_luong_cot = 50;
                phongChieu.status = 1;
                var checkopc = db.phong_chieu.Where(x => x.ten_phong == phongChieu.ten_phong && x.id_rapchieu == phongChieu.id_rapchieu).Count();
                if (checkopc != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 phòng chiếu giống phòng chiếu này. Vui lòng kiểm tra lại!";
                    return View();
                }
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
                        ghe.gia = 75000;
                        ghe.loai_ghe_id = loaighe.id;
                        ghe.status = 1;
                        db.ghe_ngoi.Add(ghe);
                        db.SaveChanges();
                    }
                }
                TempData["Message"] = "Tạo thành công!";
            }
            else
            {
                TempData["Warning"] = "Không thể để trống";
            }
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
                    return RedirectToAction("AError404", "Admin");
                }
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

                return View(phongChieu);
            }
            catch (Exception)
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
                var checkopc = db.phong_chieu.Where(x => x.ten_phong == phongChieu.ten_phong && x.id_rapchieu == phongChieu.id_rapchieu).Count();
                if (checkopc != 0)
                {
                    TempData["Warning"] = "Đã xảy ra lỗi! Đã tồn 1 phòng chiếu giống phòng chiếu này. Vui lòng kiểm tra lại!";
                    return RedirectToAction("ListCinemaRoom");
                }
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
                if (phongChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }

                if (phongChieu.status != 2)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                // bằng 2 thì đã dừng hoạt động
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
            catch (Exception)
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
                if (phong_Chieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
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
                //status = 2 là dừng hoạt động
                phong_Chieu.status = 2;
                db.Entry(phong_Chieu).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Dừng hoạt động thành công";
                return RedirectToAction("ListCinemaRoom");
            }
            catch (Exception)
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
                if (phong_Chieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }

                //Nếu không phải = 2 là sai
                if (phong_Chieu.status != 2)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                //status = 1 là hoạt động
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
            if (id == null || id < 1)
            {
                return RedirectToAction("AError404", "Admin");
            }
            try
            {
                phong_chieu phongChieu = db.phong_chieu.Find(id);
                if (phongChieu == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                if (phongChieu.status == 0)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                ViewBag.loai_ghe_id = db.loai_ghe.ToList().OrderBy(n => n.id);
                ViewBag.pc = phongChieu;
                var ghengoi = db.ghe_ngoi.Where(x => x.phong_chieu_id == id & x.status == 1).OrderBy(x => x.Row);
                ViewBag.ghe = ghengoi;
                return View(ghengoi);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }

        [HttpPost]
        public ActionResult CreateSeat(string hang, int ghe, int id, int idloaighe)
        {
            if (hang == null || ghe < 1 || id < 1)
            {
                return Json(new { checkr = false });
            }

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
            if (phongChieu.status == 0)
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
                        ghengoi.gia = 75000;
                        //chưa sửa chọn ghế : VIP| Thường ...
                        ghengoi.loai_ghe_id = idloaighe;
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
            var idLoaiGhe = db.ghe_ngoi.Where(x => x.Row == Row && x.phong_chieu_id == pcid && x.status == 1).FirstOrDefault();
            List<int?> idloaig = new List<int?>();
            List<string> tenloaig = new List<string>();
            idloaig.Add(idLoaiGhe.loai_ghe_id);
            tenloaig.Add(idLoaiGhe.loai_ghe.ten_ghe);
            foreach (var item in db.loai_ghe.ToList())
            {
                idloaig.Add(item.id);
                tenloaig.Add(item.ten_ghe);
            }
            var count = idloaig.Count();
            return Json(data: new { count, idloaig, tenloaig, seat, Row }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EditSeat(string hang, int ghe, int id, int idloaighe)
        {
            var ghn = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id && n.status == 1).ToList();
            var tong = ghn.Count();
            ghe_ngoi ghengoi = new ghe_ngoi();
            var checkghe = db.ghe_ngoi.Where(x => x.phong_chieu_id == id && x.status == 1).ToList();
            var checkpc = db.phong_chieu.Find(id);
            if (checkpc.status == 0)
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
            if (dem > 0)
            {
                return Json(new { success = false });
            }


            try
            {
                if (tong < ghe)
                {
                    checkpc.so_luong_cot = checkghe.Count() + (ghe - tong);
                    db.Entry(checkpc).State = EntityState.Modified;
                    //Sửa ghế trước rồi tạo mới ghế sau
                    foreach (var gheht in ghn)
                    {
                        ghe_ngoi seat = db.ghe_ngoi.Find(gheht.id);
                        seat.loai_ghe_id = idloaighe;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    for (int i = tong; i < ghe; i++)
                    {
                        ghengoi.Row = hang;
                        ghengoi.Col = i + 1;
                        ghengoi.phong_chieu_id = id;
                        ghengoi.gia = 75000;
                        //chưa sửa chọn ghế : VIP| Thường ...
                        ghengoi.loai_ghe_id = idloaighe;
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
                    var ghehientai = db.ghe_ngoi.Where(n => n.Row == hang && n.phong_chieu_id == id && n.status == 1).ToList();
                    foreach (var gheht in ghehientai)
                    {
                        ghe_ngoi seat = db.ghe_ngoi.Find(gheht.id);
                        seat.loai_ghe_id = idloaighe;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                }
                else if (tong == ghe)
                {
                    foreach (var idghe in ghn)
                    {
                        ghe_ngoi seat = db.ghe_ngoi.Find(idghe.id);
                        seat.loai_ghe_id = idloaighe;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
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
            if (checkpc.status == 0)
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
                    return RedirectToAction("AError404", "Admin");
                }
                return View(rapChieu);
            }
            catch (Exception)
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

        //Tạo loại ghế
        public ActionResult ListSeatType()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View(db.loai_ghe.ToList().OrderBy(n => n.id));
        }

        public ActionResult CreateSeatType()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateSeatType(loai_ghe loaiGhe)
        {
            if (ModelState.IsValid)
            {

                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    Random rd = new Random();
                    var numrd = rd.Next(1, 100).ToString();
                    string filename = "seattype" + DateTime.Now.ToString("MMMM'-'dd'-'yyyy") + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    loaiGhe.anh = filename;
                    string path = Server.MapPath("~/images/seattype/");
                    string StrPath = Path.Combine(path, filename);
                    file.SaveAs(StrPath);
                }
                loaiGhe.status = 1;
                db.loai_ghe.Add(loaiGhe);
                TempData["Message"] = "Tạo thành công!";
                db.SaveChanges();
                return RedirectToAction("ListSeatType");
            }
            return View(loaiGhe);
        }

        public ActionResult EditSeatType(int? id)
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

                loai_ghe loaiGhe = db.loai_ghe.Find(id);
                if (loaiGhe == null)
                {
                    return RedirectToAction("AError404", "Admin");
                }
                return View(loaiGhe);
            }
            catch (Exception)
            {
                return RedirectToAction("AError404", "Admin");
            }
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditSeatType(loai_ghe loaiGhe)
        {
            if (ModelState.IsValid)
            {

                var file = Request.Files["anh"];
                if (file != null && file.ContentLength > 0)
                {
                    Random rd = new Random();
                    var numrd = rd.Next(1, 100).ToString();
                    string filename = "seattype" + DateTime.Now.ToString("MMMM'-'dd'-'yyyy") + file.FileName.Substring(file.FileName.LastIndexOf("."));
                    loaiGhe.anh = filename;
                    string path = Server.MapPath("~/images/seattype/");
                    string StrPath = Path.Combine(path, filename);
                    file.SaveAs(StrPath);
                }
                db.Entry(loaiGhe).State = EntityState.Modified;
                TempData["Error"] = "Cập nhật thành công!";
                db.SaveChanges();
                return RedirectToAction("ListSeatType");
            }
            else
            {
                TempData["Error"] = "Cập nhật không thành công!";
            }
            return View(loaiGhe);
        }

        public ActionResult DeleteSeatType(int? id)
        {
            if (db.ghe_ngoi.Where(n => n.loai_ghe_id == id && n.phong_chieu.status != 0).ToList().Count() == 0)
            {
                loai_ghe loaiGhe = db.loai_ghe.Find(id);
                db.loai_ghe.Remove(loaiGhe);
                TempData["Message"] = "Xóa thành công!";
                db.SaveChanges();
                return RedirectToAction("CinemaList");
            }
            else
            {
                TempData["Error"] = "Không thể xóa vì loại ghế đang tồn tại trong rạp chiếu!";
                return RedirectToAction("CinemaList");
            }
        }

    }

}