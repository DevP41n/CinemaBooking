using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class FeedbackController : Controller
    {
        // GET: Admin/Feedback
        public ActionResult ListFeedback()
        {
            return View();
        }

        public ActionResult Detail()
        {
            return View();
        }

        public ActionResult Reply()
        {
            return View();
        }

        public ActionResult Delete()
        {
            return View();
        }
        public ActionResult DeleteConfirm()
        {
            return View();
        }
    }
}