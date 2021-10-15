using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace CinemaBooking.Controllers
{
    public class NewsController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: News
        public ActionResult News(string title, int? page)
        {
            ViewBag.titleDisplay = title;
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(db.su_kien.OrderByDescending(s => s.create_at).ToPagedList(pageNumber, pageSize));
        }
        // GET: News/NewsDetail
        public ActionResult NewsDetail(int id)
        {
            return View(db.su_kien.SingleOrDefault(s =>s.id.Equals(id)));
        }
    }
}