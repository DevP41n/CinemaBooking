using CinemaBooking.Library;
using CinemaBooking.Models;
using Facebook;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class UserController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        public static string urlpre = null;
        // GET: User
        public ActionResult SignIn(string url)
        {
            if (Session["TenCus"] != null)
            {
                if (url != null)
                {
                    return Redirect(url);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (url != null)
                {
                    urlpre = url;
                }

                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string Taikhoan, string Matkhau)
        {
            if (Session["TenCus"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

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
                    if (urlpre != null)
                    {
                        string url = urlpre;
                        urlpre = null; // gán lại cái urlpre null, nếu không thì khi user đăng xuất rồi đăng nhập tiếp thì nó lại vào cái urlpre này.
                        return Redirect(url);

                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
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
                if (check == null && checkuser == null)
                {

                    _user.password = MyString.ToMD5(_user.password);
                    _user.create_at = DateTime.Now;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.khach_hang.Add(_user);
                    TempData["Message"] = "Bạn đã tạo tài khoản thành công!";
                    db.SaveChanges();
                    return RedirectToAction("SignIn");
                }
                else
                {
                    if (check != null)
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
            Session["TenKH"] = null;
            Session["TenCus"] = null;
            Session["EmailCus"] = null;
            Session["MaKH"] = null;
            Session["SDT"] = null;
            return RedirectToAction("Index", "Home");
        }

        //public ActionResult ProfileAccount(int? id)
        //{
        //    var mak = Convert.ToInt32(Session["MaKH"]);
        //    if (mak != id)
        //    {
        //        TempData["Warning"] = "Không đúng tài khoản của bạn!";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    var kh = db.khach_hang.Where(x => x.id == id).FirstOrDefault();
        //    return View(kh);
        //}
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ProfileAccount(khach_hang kh)
        {
            if (kh.sdt.Length > 10)
            {
                TempData["Warning"] = "Cập nhật không thành công!";

                string ur = "/TransHistory/" + kh.id;
                return RedirectToAction(ur);
            }
            db.Configuration.ValidateOnSaveEnabled = false;
            kh.update_at = DateTime.Now;

            db.Entry(kh).State = EntityState.Modified;
            db.SaveChanges();
            Session["TenCus"] = kh.ho_ten;
            Session["EmailCus"] = kh.email;
            Session["SDT"] = kh.sdt;
            TempData["Message"] = "Cập nhật thành công!";

            string url = "/TransHistory/" + kh.id;
            return RedirectToAction(url);
        }
        //public ActionResult ChangePass(int? id)
        //{
        //    var mak = Convert.ToInt32(Session["MaKH"]);
        //    if (mak != id)
        //    {
        //        TempData["Warning"] = "Không đúng tài khoản của bạn!";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    var kh = db.khach_hang.Where(x => x.id == id).FirstOrDefault();
        //    return View(kh);
        //}
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(khach_hang kh, FormCollection f)
        {

            var mkc = Request.Form["mkcu"];
            var mkm = Request.Form["mkmoi"];
            var xnmk = Request.Form["xnmk"];
            if (mkc == "" || mkm == "" || xnmk == "")
            {
                string urlaz = "/TransHistory/" + kh.id;
                TempData["Warning"] = "Không được để trống!";
                return RedirectToAction(urlaz);
            }
            var mkcuu = MyString.ToMD5(mkc);
            if (ModelState.IsValid)
            {
                if (kh.password != mkcuu)
                {
                    TempData["Warning"] = "Sai mật khẩu cũ!";
                    string urla = "/TransHistory/" + kh.id;
                    return RedirectToAction(urla);
                }
                if (Request.Form["mkmoi"] != Request.Form["xnmk"])
                {
                    TempData["Warning"] = "Mật khẩu không khớp!";
                    string urlaz = "/TransHistory/" + kh.id;
                    return RedirectToAction(urlaz);
                }
                var matkhau = Request.Form["mkmoi"];
                kh.password = MyString.ToMD5(matkhau);
                kh.confirmpassword = MyString.ToMD5(matkhau);
                kh.update_at = DateTime.Now;
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(kh).State = EntityState.Modified;
                db.SaveChanges();
            }
            TempData["Message"] = "Đổi mật khẩu thành công!";
            string url = "/TransHistory/" + kh.id;
            return RedirectToAction(url);

        }
        public ActionResult Logout()
        {
            Session["TenKH"] = null;
            Session["TenCus"] = null;
            Session["EmailCus"] = null;
            Session["MaKH"] = null;
            Session["SDT"] = null;
            TempData["Message"] = "Đã đăng xuất!";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult TransHistory(int? id)
        {
            var idkh = Convert.ToInt32(Session["MaKH"]);
            if (idkh != id)
            {
                TempData["Warning"] = "Không đúng tài khoản của bạn!";
                return RedirectToAction("Index", "Home");
            }
            TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
            //Status 2: đang chờ thanh toán tại quầy
            var orderss = db.orders.Where(n => n.status == 2);
            foreach (var itemm in orderss)
            {
                if (itemm.ngay_mua + tinhgio <= DateTime.Now)
                {
                    itemm.status = 0;
                    db.Entry(itemm).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            var orders = db.orders.Where(n => n.id_khachhang == idkh).OrderByDescending(n => n.id).ToList();
            return View(orders);
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
        [HttpPost]
        public ActionResult Forgot(string email)
        {
            khach_hang KH = db.khach_hang.Where(x => x.email == email).FirstOrDefault();
            if (KH == null)
            {
                return Json(new { success = false });

            }
            var rsinsert = new khach_hang().InsertForgot(KH);
            if (rsinsert > 0)
            {
                var id = rsinsert;
                khach_hang khachhang = db.khach_hang.Find(id);
                Random rd = new Random();
                var numr = rd.Next(1, 100000).ToString();
                var numrd = rd.Next(1, 100000).ToString();
                string[] myIntArray = new string[10];
                string randomStr = "";
                for (int x = 0; x < 5; x++)
                {
                    myIntArray[x] = Convert.ToChar(Convert.ToInt32(rd.Next(65, 87))).ToString();
                    randomStr += (myIntArray[x].ToString());
                }
                string mk = "@Cinema" + numrd + randomStr + numr;
                khachhang.password = MyString.ToMD5(mk);
                khachhang.confirmpassword = khachhang.password;
                try
                {
                    string mail = System.IO.File.ReadAllText(Server.MapPath("~/Library/Forgot.html"));
                    string dt = DateTime.Now.ToString();
                    mail = mail.Replace("{{Name}}", khachhang.ho_ten);
                    mail = mail.Replace("{{Email}}", khachhang.email);
                    mail = mail.Replace("{{Pass}}", mk);
                    mail = mail.Replace("{{date}}", dt);
                    var num = rd.Next(1, 1000000).ToString();
                    new SendMail().SendMailTo(khachhang.email.ToString(), "Xác nhận mật khẩu [CINEMA]", mail);
                }
                catch (Exception)
                {
                    return Json(new { success = false });
                }
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Entry(khachhang).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true });
        }

        // hủy vé
        public ActionResult CancelTicket(int? id)
        {
            if (Session["MaKH"] == null)
            {
                TempData["Warning"] = "Bạn chưa đăng nhập!";
                return RedirectToAction("SignIn", "User");
            }
            var idkh = Convert.ToInt32(Session["MaKH"]);
            string idkhach = Session["MaKH"].ToString();
            string ur = "/TransHistory/" + idkhach;
            var ord = db.orders.Find(id);
            if (ord == null || idkh != ord.id_khachhang)
            {
                TempData["Warning"] = "Không đúng tài khoản của bạn!";
                return RedirectToAction(ur);
            }
            if (ord.status == 0)
            {
                TempData["Warning"] = "Vé đã hủy!";
                return RedirectToAction(ur);
            }
            else if (ord.status == 1)
            {
                TempData["Warning"] = "Vé đã thanh toán. Không thể hủy!";
                return RedirectToAction(ur);
            }
            ord.status = 0;
            db.Entry(ord).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Message"] = "Hủy thành công!";
            string url = "/TransHistory/" + ord.id_khachhang;
            return RedirectToAction(url);
        }

        [HttpPost]
        public ActionResult Check(string id)
        {
            int idord = Convert.ToInt32(id);
            order ord = db.orders.Find(idord);
            if (ord.status == 2)
            {
                ord.status = 0;
                db.Entry(ord).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult CheckSecond(string id)
        {
            int idord = Convert.ToInt32(id);
            order ord = db.orders.Find(idord);
            if (ord.status == 1)
            {
                return Json(new { success = true });
            }
            if (ord.status == 0)
            {
                return Json(new { success = false });
            }
            return Json(new { check = false });
        }
    }
}