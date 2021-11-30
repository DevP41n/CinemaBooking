using CinemaBooking.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/Orders
        public ActionResult ListOrders()
        {
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
            return View(db.orders.OrderByDescending(n => n.ngay_mua).ToList());
        }

        public ActionResult OrdDetail(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("ListOrders");
            }
            try
            {
                var order = db.orders.Find(id);
                if (order == null)
                {
                    return RedirectToAction("ListOrders");
                }
                ViewBag.detail = db.order_details.Where(n => n.id_orders == id);
                return View(order);
            }
            catch(Exception)
            {
                return RedirectToAction("ListOrders");
            }
        }


    }
}