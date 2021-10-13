using CinemaBooking.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Areas.Admin.Controllers
{
    public class FeedbackController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: Admin/Feedback
        public ActionResult ListFeedback()
        {
            ViewBag.count = db.lien_he.Count();
            ViewBag.count1 = db.lien_he.Where(n => n.status == 1).Count();
            ViewBag.count2 = db.lien_he.Where(n => n.status == 2).Count();
            ViewBag.count0 = db.lien_he.Where(n => n.status == 0).Count();
            return View(db.lien_he.ToList());
        }

        public ActionResult Detail(int? id)
        {
            ViewBag.count = db.lien_he.Count();
            ViewBag.count1 = db.lien_he.Where(n => n.status == 1).Count();
            ViewBag.count2 = db.lien_he.Where(n => n.status == 2).Count();
            ViewBag.count0 = db.lien_he.Where(n => n.status == 0).Count();
            var details = db.lien_he.Find(id);
            return View(details);
        }

        public ActionResult Reply(int? id)
        {
            ViewBag.count = db.lien_he.Count();
            ViewBag.count1 = db.lien_he.Where(n => n.status == 1).Count();
            ViewBag.count2 = db.lien_he.Where(n => n.status == 2).Count();
            ViewBag.count0 = db.lien_he.Where(n => n.status == 0).Count();
            var details = db.lien_he.Find(id);
            return View(details);
        }
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Reply(lien_he lienHe)
        {
            if (ModelState.IsValid)
            {
                lienHe.update_at = DateTime.Now;
                lienHe.status = 2;
                db.Entry(lienHe).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.Configuration.ValidateOnSaveEnabled = false;
            ////Send Mail
            //string mail = System.IO.File.ReadAllText(Server.MapPath("~/template/mail/MailReply.html"));
            //string dt = DateTime.Now.ToString();
            //mail = mail.Replace("{{Name}}", fb.Reply);

            ////mail = mail.Replace("{{Phone}}", Request.Form["sdt"]);
            //mail = mail.Replace("{{Email}}", Request.Form["email"]);
            ////mail = mail.Replace("{{Address}}", Request.Form["dc"]);
            //Random rd = new Random();
            //var numrd = rd.Next(1, 1000000).ToString();
            //new SendMailOrder().SendMailTo(fb.Email, "Đơn hàng mới từ HYPER GEAR [HYPER" + numrd + "]", mail);
            //save
            return RedirectToAction("ListFeedback");
        }

        public ActionResult Delete(int id)
        {
            lien_he fb = db.lien_he.Find(id);
            fb.status = 0;
            db.Entry(fb).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Listfeedback");

        }

        public ActionResult DeleteConfirm(int id)
        {
            lien_he fb = db.lien_he.Find(id);
            db.lien_he.Remove(fb);
            db.SaveChanges();
            return RedirectToAction("ListFeedback");

        }
    }
}