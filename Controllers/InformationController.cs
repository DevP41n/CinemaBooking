using CinemaBooking.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using PagedList;

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
        public ActionResult AboutUs()
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
