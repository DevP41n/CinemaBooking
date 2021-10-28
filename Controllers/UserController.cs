using CinemaBooking.Models;
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
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
                    return RedirectToAction("SignIn");
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

        public ActionResult ProfileAccount(int? id)
        {
            var mak = Convert.ToInt32(Session["MaKH"]);
            if (mak != id)
            {
                TempData["Warning"] = "Không đúng tài khoản của bạn!";
                return RedirectToAction("Index", "Home");
            }
            var kh = db.khach_hang.Where(x => x.id == id).FirstOrDefault();
            return View(kh);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileAccount(khach_hang kh)
        {
            db.Configuration.ValidateOnSaveEnabled = false;
            db.Entry(kh).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Message"] = "Cập nhật thành công!";
            string url = "/ProfileAccount/" + kh.id;
            return RedirectToAction(url);
        }
        public ActionResult ChangePass(int? id)
        {
            var mak = Convert.ToInt32(Session["MaKH"]);
            if (mak != id)
            {
                TempData["Warning"] = "Không đúng tài khoản của bạn!";
                return RedirectToAction("Index", "Home");
            }
            var kh = db.khach_hang.Where(x => x.id == id).FirstOrDefault();
            return View(kh);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(khach_hang kh, FormCollection f)
        {
            var mkc = Request.Form["mkcu"];
            var mkcuu = MyString.ToMD5(mkc);
            if (kh.password != mkcuu)
            {
                TempData["Warning"] = "Sai mật khẩu cũ!";
                string urla = "/ChangePass/" + kh.id;
                return RedirectToAction(urla);
            }
            if (Request.Form["mkmoi"] != Request.Form["xnmk"])
            {
                TempData["Warning"] = "Mật khẩu không khớp!";
                string urlaz = "/ChangePass/" + kh.id;
                return RedirectToAction(urlaz);
            }
            var matkhau = Request.Form["mkmoi"];
            kh.password = MyString.ToMD5(matkhau);
            kh.confirmpassword = MyString.ToMD5(matkhau);
            db.Configuration.ValidateOnSaveEnabled = false;
            db.Entry(kh).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Message"] = "Đổi mật khẩu thành công!";
            string url = "/ProfileAccount/" + kh.id;
            return RedirectToAction(url);
        }
        //Facebook
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginWithFB()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });


            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                // Get the user's information, like email,name etc
                dynamic me = fb.Get("me?fields=name,id,email");
                string email = me.email;
                string userName = me.email;
                string hoten = me.name;


                var user = new khach_hang();
                user.email = email;
                user.username = userName;
                user.ho_ten = hoten;
                user.gioi_tinh = true;
                user.update_at = DateTime.Now;
                user.ngay_sinh = null;
                user.sdt = "0123456789";
                user.password = "@Cinema123";
                user.confirmpassword = "@Cinema123";
                var resultInsert = new khach_hang().InsertForFacebook(user);
                if (resultInsert > 0)
                {
                    Session["MaKH"] = resultInsert;
                    Session["TenCus"] = user.ho_ten;
                    Session["EmailCus"] = user.email;
                }
                TempData["Message"] = "Đăng nhập thành công";
            }
            return Redirect("/");
        }

    }
}
