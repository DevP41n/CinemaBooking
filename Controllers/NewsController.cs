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
        public ActionResult Index(string title, int? page)
        {
            ViewBag.titleDisplay = title;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(db.su_kien.OrderByDescending(s => s.create_at).ToPagedList(pageNumber, pageSize));
        }
        // GET: News/NewsDetail
        public ActionResult NewsDetail(String slug)
        {
            try
            {
                if (slug == null)
                {
                    return RedirectToAction("Error404", "Home");
                }
                return View(db.su_kien.SingleOrDefault(s => s.slug.Equals(slug)));
            }
            catch(Exception)
            {
                return RedirectToAction("Error404", "Home");
            }
        }
    }
}