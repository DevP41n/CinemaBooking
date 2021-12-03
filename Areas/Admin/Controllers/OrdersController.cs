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
            TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
            if (id == null)
            {
                return RedirectToAction("ListOrders");
            }
            try
            {
                order ord = db.orders.Find(id);
                if (ord == null)
                {
                    return RedirectToAction("ListOrders");
                }
                if(ord.status == 2 && ord.ngay_mua + tinhgio <= DateTime.Now)
                {
                    ord.status = 0;
                    db.Entry(ord).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewBag.detail = db.order_details.Where(n => n.id_orders == id);
                return View(ord);
            }
            catch(Exception)
            {
                return RedirectToAction("ListOrders");
            }
        }
        [HttpPost]
        public ActionResult confirmPay(string id)
        {
            var idord = Convert.ToInt32(id);
            order ord = db.orders.Find(idord);
            if(ord.status == 0)
            {
                return Json(new { success = false });
            }
            else if(ord.status == 1)
            {
                return Json(new { check = true });
            }
            else if(ord.status == 2)
            {
                ord.status = 1;
                db.Entry(ord).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult confirmCancel(string id)
        {
            var idord = Convert.ToInt32(id);
            order ord = db.orders.Find(idord);
            if (ord.status == 0)
            {
                return Json(new { success = false });
            }
            else if (ord.status == 1)
            {
                return Json(new { check = true });
            }
            else if (ord.status == 2)
            {
                ord.status = 0;
                db.Entry(ord).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new { success = true });
        }
    }
}