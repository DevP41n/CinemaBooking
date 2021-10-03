using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class InformationController : Controller
    {
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
        public ActionResult Contact()
        {
            return View();
        }
    }
}
