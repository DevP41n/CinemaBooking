using CinemaBooking.Models;
using CinemaOnline.Handler.Payment.Momo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using MoMoSecurity = CinemaBooking.Library.MoMoSecurity;

namespace CinemaBooking.Controllers
{
    public class PaymentMoMoController : Controller
    {
        private CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: PaymentMoMo
        public ActionResult Momo()
        {
            string idghe = TempData["idghe"].ToString();
            string idsc = TempData["idsuatc"].ToString();
            string idtime = TempData["idtime"].ToString();
            int idsuatchieu = Convert.ToInt32(idsc);
            int idtimechieu = Convert.ToInt32(idtime);
            var checkorder = db.orders.Where(x => x.suatchieu_id == idsuatchieu && x.status == idtimechieu).ToList();
            List<int> idghengoi = new List<int>();
            string[] listid = idghe.Split(',');
            for (int i = 0; i < listid.Length; i++)
            {
                if (listid[i] != "")
                {
                    idghengoi.Add(Convert.ToInt32(listid[i]));
                }
            }
            double tien = 75000 * idghengoi.Count();
            string tongtien = tien.ToString();
            var url = MomoExtend.GenUrlPay(tongtien);
            return Redirect(url);

        }

        public ActionResult ReturnUrl()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectKey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string signature = crypto.signSHA256(param, serectKey);

            if (signature != Request["signature".ToString()])
            {
                TempData["Error"] = "Thanh toán thất bại";
            }
            if (!Request.QueryString["errorCode"].Equals("0"))
            {
                TempData["Error"] = "Thanh toán thất bại";
            }
            else
            {
                TempData["Message"] = "Thanh toán thành công";
                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        public ActionResult NotifyUrl()
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectKey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string signature = crypto.signSHA256(param, serectKey);

            if (signature != Request["signature".ToString()])
            {

            }
            string status_code = Request["status_code"].ToString();


            return Json("", JsonRequestBehavior.AllowGet);
        }



    }
}