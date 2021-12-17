using CinemaBooking.Models;
using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
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

        public ActionResult TurnOver()
        {
            if (Session["HoTen"] == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            if (Convert.ToInt32(Session["Role"]) != 1)
            {
                TempData["Warning"] = "Bạn không phải là admin!";
                return RedirectToAction("Dashboard", "Admin");
            }

            var order = db.orders.Where(n => n.status == 1);
            var datenow = DateTime.Now.ToString("yyyy");

            var ticket = new ArrayList();
            for (int i = 1; i <= 12; i++)
            {
                string dateorder = i + "/" + datenow;

                int ticketOfMonth = 0;
                foreach (var item in order)
                {
                    var ord = Convert.ToDateTime(item.ngay_mua).ToString("MM/yyyy");
                    if (ord == dateorder)
                    {

                        ticketOfMonth += Convert.ToInt32(item.so_luong_ve);
                    }

                }
                ticket.Add(ticketOfMonth);

            }
            var month = DateTime.Now.ToString("MM/yyyy");

            var monthpre = DateTime.Now.AddMonths(-1).ToString("MM/yyyy");
            double tienthangnay = 0;
            double tienthangtruoc = 0;

            foreach (var item in order)
            {
                var ord = Convert.ToDateTime(item.ngay_mua).ToString("MM/yyyy");
                if (ord == month)
                {
                    tienthangnay += (double)item.tong_tien;

                }

            }
            foreach (var item in order)
            {
                var ord = Convert.ToDateTime(item.ngay_mua).ToString("MM/yyyy");
                if (ord == monthpre)
                {
                    tienthangtruoc += (double)item.tong_tien;

                }

            }


            ViewBag.tienthangnay = tienthangnay;
            ViewBag.tienthangtruoc = tienthangtruoc;
            ViewBag.TicketOfMonth = ticket;
            return View();
        }
        //public ActionResult Testtime()
        //{
        //    CinemaBookingEntities db = new CinemaBookingEntities();
        //    var order = db.orders.Find(34);
        //    var time = order.ngay_mua - DateTime.Now;
        //    var ngay = order.ngay_mua.ToString();
        //    var tach  =   ngay.Split(' ');
        //    TimeSpan tinh = new TimeSpan(0,15,0);
        //    var test = order.ngay_mua + tinh;
        //    return View();
        //}

        public ActionResult TestRandom()
        {
            Random random = new Random();
            int length = 15;
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            string codeticket = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return Redirect("Dashboard");
        }

        public ActionResult AError404()
        {
            return View();
        }
    }
}