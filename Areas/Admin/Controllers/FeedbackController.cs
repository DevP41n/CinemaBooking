using CinemaBooking.Library;
using CinemaBooking.Models;
using System;
using System.Configuration;
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
        public ActionResult Reply(lien_he lienHe, FormCollection f)
        {

            db.Configuration.ValidateOnSaveEnabled = false;
            ////Send Mail
            try
            {

                var tieu_de = Request.Form["tieu_de"];
                string mail = System.IO.File.ReadAllText(Server.MapPath("~/Library/MailFeedback.html"));
                mail = mail.Replace("{{Name}}", lienHe.ho_ten.ToString());
                mail = mail.Replace("{{Email}}", lienHe.email.ToString());
                mail = mail.Replace("{{Tieude}}", tieu_de.ToString());
                mail = mail.Replace("{{Noidung}}", lienHe.noi_dung.ToString());
                mail = mail.Replace("{{Reply}}", lienHe.tra_loi.ToString());
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                Random rd = new Random();
                var numrd = rd.Next(1, 1000000).ToString();
                new SendMail().SendMailTo(lienHe.email.ToString(), "Trả lời khách hàng", mail);
                if (ModelState.IsValid)
                {
                    lienHe.update_at = DateTime.Now;
                    lienHe.status = 2;
                    db.Entry(lienHe).State = EntityState.Modified;
                    db.SaveChanges();
                }
                TempData["Message"] = "Gửi email thành công!";
                return RedirectToAction("ListFeedback");
            }
            catch (Exception)
            {
                TempData["Error"] = "Gửi email không thành công!";
                return RedirectToAction("ListFeedback");
            }
        }

        public ActionResult Delete(int id)
        {
            lien_he fb = db.lien_he.Find(id);
            fb.status = 0;
            db.Entry(fb).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Listfeedback");

        }
        public ActionResult Undo(int id)
        {
            lien_he fb = db.lien_he.Find(id);
            fb.status = 1;
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