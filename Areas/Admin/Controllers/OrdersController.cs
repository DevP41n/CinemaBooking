using CinemaBooking.Models;
using System;
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