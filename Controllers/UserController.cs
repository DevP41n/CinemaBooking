using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class UserController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: User
        public ActionResult SignIn()
        {
            if (Session["TenCus"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string Taikhoan, string Matkhau)
        {
            if (ModelState.IsValid)
            {
                var f_password = MyString.ToMD5(Matkhau);
                var data = db.khach_hang.Where(s => s.username.Equals(Taikhoan) && s.password.Equals(f_password)).ToList();
                if (data.Count() > 0)
                {
                    //add session
                    Session["TenKH"] = data;
                    Session["TenCus"] = data.FirstOrDefault().ho_ten;
                    Session["EmailCus"] = data.FirstOrDefault().email;
                    Session["MaKH"] = data.FirstOrDefault().id;
                    Session["SDT"] = data.FirstOrDefault().sdt;
                    TempData["Message"] = "Đăng nhập thành công";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Warning"] = "Sai tài khoản hoặc mật khẩu vui lòng nhập lại!";
                    return RedirectToAction("CusLogin");
                }
            }
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CaptchaValidationActionFilter("CaptchaCode", "regCaptcha", "Sai mã xác nhận vui lòng nhập lại!")]
        public ActionResult SignUp(khach_hang _user)
        {
            if (ModelState.IsValid)
            {
                var check = db.khach_hang.FirstOrDefault(s => s.email == _user.email || s.username == _user.username);
                var checkuser = db.khach_hang.FirstOrDefault(s => s.username == _user.username);
                if (check == null && checkuser ==null)
                {

                    _user.password = MyString.ToMD5(_user.password);
                    _user.ngay_sinh = DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.khach_hang.Add(_user);
                    TempData["Message"] = "Bạn đã tạo tài khoản thành công!";
                    db.SaveChanges();
                    return RedirectToAction("SignIn");
                }
                else
                {
                    if(check!=null)
                    ViewBag.error = "Email này đã đăng ký bằng email khác, vui lòng nhập email khác!";
                    else if (checkuser != null)
                    ViewBag.error = "Tài khoản này đã đăng ký bằng tài khoản khác, vui lòng nhập tài khoản khác!";
                    return View();
                }


            }
            return View();
        }

        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
