using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        public ActionResult Dashboard()
        {
            if (Session["HoTen"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }

        }
    }
}