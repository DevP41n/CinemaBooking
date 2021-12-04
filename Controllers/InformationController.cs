using CinemaBooking.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CinemaBooking.Controllers
{
    public class InformationController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Information
        public ActionResult Actor(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(db.dien_vien.OrderByDescending(a =>a.ho_ten).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ActorDetail(string id)
        {
            return View(db.dien_vien.SingleOrDefault(s => s.slug.Equals(id)));
        }
        public ActionResult Director(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            return View(db.dao_dien.OrderByDescending(a => a.ho_ten).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult DirectorDetail(string id)
        {
            return View(db.dao_dien.SingleOrDefault(s => s.slug.Equals(id)));
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
        public ActionResult SearchDirector()
        {
            var result = from a in db.dao_dien
                         select new { a.ho_ten, a.anh, a.slug };
            List<dao_dien> director = result.AsEnumerable()
                          .Select(o => new dao_dien
                          {
                              ho_ten = o.ho_ten,
                              anh = o.anh,
                              slug = o.slug
                          }).ToList();
            string value = string.Empty;
            value = JsonConvert.SerializeObject(director, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Content(value);

        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult TermConditions()
        {
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        public ActionResult PaymentPolicy()
        {
            return View();
        }

        //Phản hổi của khách hàng.
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(lien_he lienHe)
        {
            if (ModelState.IsValid)
            {
                lienHe.create_at = DateTime.Now;
                lienHe.status = 1;
                db.lien_he.Add(lienHe);
                db.SaveChanges();
                TempData["Message"] = "Gửi phản hồi thành công";
                return View();
            }
            return View();
        }
    }
}
