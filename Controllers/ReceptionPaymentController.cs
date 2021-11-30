using CinemaBooking.Library;
using CinemaBooking.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class ReceptionPaymentController : Controller
    {
        CinemaBookingEntities db = new CinemaBookingEntities();
        // GET: ReceptionPayment
        public ActionResult withReceptionPay()
        {
            string idghe = TempData["idghe"].ToString();
            string idsc = TempData["idsuatc"].ToString();
            string idtime = TempData["idtime"].ToString();
            string idkh = TempData["idkh"].ToString();

            int idsuatchieu = Convert.ToInt32(idsc);
            int idtimechieu = Convert.ToInt32(idtime);
            //check
            var checkorder = db.orders.Where(x => x.suatchieu_id == idsuatchieu && x.idtime == idtimechieu).ToList();
            List<int> idghengoi = new List<int>();
            string[] listid = idghe.Split(',');
            for (int i = 0; i < listid.Length; i++)
            {
                if (listid[i] != "")
                {
                    idghengoi.Add(Convert.ToInt32(listid[i]));
                }
            }
            int dem = 0;
            foreach (var item in idghengoi)
            {
                foreach (var i in checkorder)
                {
                    var checkdetails = db.order_details.Where(x => x.id_ghe == item && x.id_orders == i.id);
                    if (checkdetails.Count() != 0)
                    {
                        dem++;
                    }
                }
            }
            if (dem > 0)
            {
                TempData["Error"] = "Ghế đã có người vừa đặt, Vui lòng chọn ghế khác!";
                return RedirectToAction("Index", "Home");
            }
            //thanh toan
            var sc = db.suat_chieu.Find(idsuatchieu);
            order addorder = new order();
            addorder.id_khachhang = Convert.ToInt32(TempData["idkh"].ToString());
            addorder.id_phim = sc.phim_id;
            addorder.ten_phim = sc.phim.ten_phim;
            addorder.id_phong_chieu = sc.phong_chieu_id;
            addorder.ten_phong_chieu = sc.phong_chieu.ten_phong;
            addorder.suatchieu_id = idsuatchieu;
            addorder.ngay_mua = DateTime.Now;
            addorder.idtime = idtimechieu;
            addorder.pay_method = "Thanh toán tại quầy";
            //Chờ thanh toán
            addorder.status = 2;
            addorder.tong_tien = idghengoi.Count() * 75000;
            addorder.so_luong_ve = idghengoi.Count();
            //addorder.code_ticket = "";
            db.orders.Add(addorder);
            db.SaveChanges();

            //add order details
            int idorder = addorder.id;
            order_details addorderdetails = new order_details();
            foreach (var gheid in idghengoi)
            {
                addorderdetails.id_ghe = gheid;
                addorderdetails.id_orders = idorder;
                addorderdetails.gia_ve = 75000;
                db.order_details.Add(addorderdetails);
                db.SaveChanges();
            }
            //send
            var kh = db.khach_hang.Find(Convert.ToInt32(TempData["idkh"]));
            var timechieu = db.TimeFrames.Find(idtimechieu);
            var orderformail = db.orders.Find(idorder);
            var phimfind = db.phims.Find(orderformail.id_phim);
            var pcfind = db.phong_chieu.Find(orderformail.id_phong_chieu);
            string phim = phimfind.ten_phim;
            string phongchieu = pcfind.ten_phong;
            string ghee = "Vé ";
            var demm = 0;
            foreach (var j in idghengoi)
            {
                demm++;
                var tenghh = db.ghe_ngoi.Find(Convert.ToInt32(j));
                if (demm == idghengoi.Count())
                {
                    ghee += tenghh.Row + tenghh.Col;
                }
                else
                {
                    ghee += tenghh.Row + tenghh.Col + ", ";
                }
            }
            try
            {
                string mail = System.IO.File.ReadAllText(Server.MapPath("~/Library/ReplyMail.html"));
                string dt = DateTime.Now.ToString();
                mail = mail.Replace("{{Name}}", kh.ho_ten.ToString());
                mail = mail.Replace("{{Phone}}", kh.sdt.ToString());
                mail = mail.Replace("{{Email}}", kh.email.ToString());
                mail = mail.Replace("{{Phim}}", phim);
                mail = mail.Replace("{{Suatchieu}}", timechieu.Time.ToString());
                mail = mail.Replace("{{Rap}}", phongchieu);
                mail = mail.Replace("{{Ve}}", ghee);
                mail = mail.Replace("{{Amount}}", String.Format("{0:0,0}", orderformail.tong_tien));
                mail = mail.Replace("{{date}}", dt);
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                Random rd = new Random();
                var numrd = rd.Next(1, 1000000).ToString();
                new SendMail().SendMailTo(kh.email.ToString(), "Xác nhận thanh toán [CINEMA" + numrd + "]", mail);
                TempData["Message"] = "Đặt vé thành công!";
                return RedirectToAction("Success", "ReceptionPayment", new { id = addorder.id });
            }
            catch (Exception)
            {

                TempData["Message"] = "Đặt vé thành công!";
                return RedirectToAction("Success", "ReceptionPayment", new { id = addorder.id });
            }
        }

        public ActionResult Success(int? idord)
        {
            var ord = db.orders.Find(idord);
            return View(ord);
        }

    }
}