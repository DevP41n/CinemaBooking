using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User
        private CinemaBookingEntities db = new CinemaBookingEntities();
        //list nhan vien
        public ActionResult ListUser()
        {
            return View(db.users.OrderByDescending(m => m.id).ToList());
        }
        public ActionResult ListKH()
        {
            return View(db.khach_hang.OrderByDescending(m => m.create_at).ToList());
        }
        //Tạo nhan vien
        public ActionResult CreateUser()
        {
            List<SelectListItem> gender = new List<SelectListItem>() {
            new SelectListItem {
                Text = "Nam", Value = "true"
            },
            new SelectListItem {
                Text = "Nữ", Value = "false"
            },
            };
            ViewBag.gender = gender;
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                Text = "Admin", Value = "1"
            },
            new SelectListItem {
                Text = "Nhân Viên", Value = "2"
            },
            };
            ViewBag.role = roleUser;
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(user uSer)
        {
            List<SelectListItem> gender = new List<SelectListItem>() {
            new SelectListItem {
                Text = "Nam", Value = "true"
            },
            new SelectListItem {
                Text = "Nữ", Value = "false"
            },
            };
            ViewBag.gender = gender;
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                Text = "Admin", Value = "1"
            },
            new SelectListItem {
                Text = "Nhân Viên", Value = "2"
            },
            };
            ViewBag.role = roleUser;
            if (ModelState.IsValid)
            {
                var check = db.users.FirstOrDefault(s => s.email == uSer.email || s.username == uSer.username);
                if (check == null)
                {

                    uSer.password = MyString.ToMD5(uSer.password);
                    uSer.create_at = DateTime.Now;
                    uSer.update_at = DateTime.Now;
                    uSer.ngay_vao_lam = DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.users.Add(uSer);
                    TempData["Message"] = "Bạn đã tạo tài khoản thành công!";
                    db.SaveChanges();
                    return RedirectToAction("ListUser");
                }
                else
                {
                    TempData["Error"] = "Email này đã đăng ký bằng tài khoản khác, vui lòng nhập email khác!";
                    ViewBag.error = "Email này đã đăng ký bằng tài khoản khác, vui lòng nhập email khác!";
                    return View();
                }
            }
            return View(uSer);
        }

        public ActionResult EditUser(int? id)
        {
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                Value = "1", Text = "Admin"
            },
            new SelectListItem {
                Value = "2", Text = "Nhân Viên"
            },
            };
            ViewBag.role = roleUser;
            user uSer = db.users.Find(id);
            if (User == null)
            {
                return RedirectToAction("ListUser", "User");
            }
            return View(uSer);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(user uSer)
        {
            List<SelectListItem> roleUser = new List<SelectListItem>() {
            new SelectListItem {
                 Value = "1", Text = "Admin"
            },
            new SelectListItem {
                Value = "2", Text = "Nhân Viên"
            },
            };
            ViewBag.role = roleUser;
            if (ModelState.IsValid)
            {
                uSer.update_at = DateTime.Now;                
                db.Entry(uSer).State = EntityState.Modified;               
                db.SaveChanges();
                TempData["Message"] = "Cập nhật thành công!";
                return RedirectToAction("ListUser");
            }
            else
            {
                TempData["Error"] = "Cập nhập không thành công!";
            }
            return View(uSer);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            //if (Session["HoTen"] == null)
            //{
            //    return RedirectToAction("Login", "Auth");
            //}
            user uSer = db.users.Find(id);
            db.users.Remove(uSer);
            db.SaveChanges();
            TempData["Message"] = "Xóa thành công!";
            return RedirectToAction("ListUser");
        }
    }
}