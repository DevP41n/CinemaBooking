using CinemaBooking.Models;
using System;
using System.Collections.Generic;
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
            var order = db.orders.OrderByDescending(n => n.id);


            return View(order);
        }

        public ActionResult OrderDetail(int? idorder)
        {
            var detail = db.order_details.Where(n => n.id_orders == idorder).ToList();
            var Count = detail.Count();
            List<String> top = new List<String>();
            List<String> tien = new List<String>();
            foreach (var item in detail)
            {
                top.Add(item.ghe_ngoi.Row + item.ghe_ngoi.Col);
                tien.Add(item.gia_ve.ToString());
            }
            return Json(data: new { Count, top, tien }, JsonRequestBehavior.AllowGet);
        }


    }
}