using CinemaBooking.Models;
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
            var order = db.orders.Find(id);
            ViewBag.detail = db.order_details.Where(n => n.id_orders == id);

            return View(order);
        }


    }
}