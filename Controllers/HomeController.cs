using CinemaBooking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public ActionResult SearchActor()
        {
            var result = from a in db.dien_vien
                         select new { a.ho_ten, a.anh, a.slug };
            List<dien_vien> actor = result.AsEnumerable()
                          .Select(o => new dien_vien
                          {
                              ho_ten = o.ho_ten,
                              anh = o.anh,
                              slug = o.slug
                          }).ToList();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(actor, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(value);
        }
        public ActionResult SearchMovie()
        {
            var result = from a in db.phims
                         select new { a.ten_phim, a.anh, a.slug };
            List<phim> film = result.AsEnumerable()
                          .Select(o => new phim
                          {
                              ten_phim = o.ten_phim,
                              anh = o.anh,
                              slug = o.slug
                          }).ToList();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(film, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(value);
        }
        [HttpPost]
        public ActionResult Search(string name)
        {
            return View(db.phims.Where(p => p.ten_phim.Contains(name)).OrderByDescending(x => x.ten_phim));
        }
    }
}