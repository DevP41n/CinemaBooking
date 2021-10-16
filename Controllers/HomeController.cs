using CinemaBooking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class HomeController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult SearchMovie()
        {
           List<phim> film = db.phims.ToList();

            string value = string.Empty;
            value = JsonConvert.SerializeObject(film, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);
        }
    }
}