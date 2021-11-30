using CinemaBooking.Models;
using System;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public ActionResult Dashboard()
        {
            if (Session["HoTen"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

        }

        public ActionResult Testtime()
        {
            CinemaBookingEntities db = new CinemaBookingEntities();
            var order = db.orders.Find(34);
            var time = order.ngay_mua - DateTime.Now;
            var ngay = order.ngay_mua.ToString();
            var tach  =   ngay.Split(' ');
            TimeSpan tinh = new TimeSpan(0,15,0);
            var test = order.ngay_mua + tinh;
            return View();
        }
    }
}