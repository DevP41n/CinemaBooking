using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class NewsFeedController : Controller
    {
        // GET: News
        public ActionResult News()
        {
            return View();
        }
        public ActionResult NewsDetail()
        {
            return View();
        }
    }
}