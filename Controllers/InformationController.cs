using CinemaBooking.Models;
using System;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class InformationController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Information
        public ActionResult Actor()
        {
            return View();
        }
        public ActionResult ActorDetail()
        {
            return View();
        }
        public ActionResult Director()
        {
            return View();
        }
        public ActionResult DirectorDetail()
        {
            return View();
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
