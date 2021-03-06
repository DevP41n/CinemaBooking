using CinemaBooking.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/Auth
        public ActionResult Login()
        {
            if (Session["HoTen"] != null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (Session["HoTen"] != null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            if (ModelState.IsValid)
            {
                var f_password = MyString.ToMD5(password);
                var data = db.users.Where(s => s.username.Equals(username) && s.password.Equals(f_password)).ToList();

                if (data.Count() > 0)
                {

                    //add session
                    Session["HoTen"] = data.FirstOrDefault().ho_ten;
                    Session["Username"] = data.FirstOrDefault().username;
                    Session["Id"] = data.FirstOrDefault().id;
                    Session["Role"] = data.FirstOrDefault().role;
                    var ten = Convert.ToString(Session["HoTen"]);
                    TempData["Message"] = "Đăng nhập thành công!</br>" + "Xin chào " + ten;
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    TempData["Warning"] = "Sai Thông tin đăng nhập vui lòng nhập lại!";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        public ActionResult Logout()
        {
            Session["HoTen"] = null;
            Session["Username"] = null;
            Session["Id"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login");
        }
    }
}