using CinemaBooking.Library;
using CinemaBooking.Models;

using Newtonsoft.Json.Linq;
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

            Session["idghe"] = idghe;
            Session["idsc"] = idsc;
            Session["idtime"] = idtime;

            string endpoint = ConfigurationManager.AppSettings["endpoint"].ToString();
            string accessKey = ConfigurationManager.AppSettings["accessKey"].ToString();
            string serectKey = ConfigurationManager.AppSettings["serectKey"].ToString();
            string orderInfo = "DH" + DateTime.Now.ToString("yyyyMMHHmmss");
            string returnUrl = ConfigurationManager.AppSettings["returnUrl"].ToString();
            string notifyurl = ConfigurationManager.AppSettings["notifyUrl"].ToString();
            string partnerCode = ConfigurationManager.AppSettings["partnerCode"].ToString();

            double tien = 75000 * idghengoi.Count();
            string tongtien = tien.ToString();

            string amount = tongtien;
            string orderid = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

            MoMoSecurity Cryto = new MoMoSecurity();
            string signature = Cryto.signSHA256(rawHash, serectKey);

            JObject message = new JObject
                {
                    { "partnerCode", partnerCode },
                    { "accessKey", accessKey },
                    { "requestId", requestId },
                    { "amount", amount},
                    { "orderId", orderid },
                    { "orderInfo", orderInfo },
                    { "returnUrl", returnUrl },
                    { "notifyUrl", notifyurl },
                    { "extraData", extraData },
                    { "requestType", "captureMoMoWallet" },
                    { "signature", signature }
                };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            return Redirect(jmessage.GetValue("payUrl").ToString());

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
                Session["idghe"] = null;
                Session["idsc"] = null;
                Session["idtime"] = null;
                TempData["Error"] = "Thanh toán thất bại";
                return RedirectToAction("Index", "Home");
            }
            if (!Request.QueryString["errorCode"].Equals("0"))
            {
                Session["idghe"] = null;
                Session["idsc"] = null;
                Session["idtime"] = null;
                TempData["Error"] = "Thanh toán thất bại";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string idghe = Session["idghe"].ToString();
                string idsc = Session["idsc"].ToString();
                string idtime = Session["idtime"].ToString();

                int idsuatchieu = Convert.ToInt32(idsc);
                int idtimechieu = Convert.ToInt32(idtime);
                var id = Convert.ToInt32(Session["MaKh"]);

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
                        var checkdetails = db.order_details.Where(x => x.id_ghe == item && x.id_orders == i.id && i.status >= 1);
                        if (checkdetails.Count() != 0)
                        {
                            dem++;
                        }
                    }
                }
                if (dem > 0)
                {
                    Session["idghe"] = null;
                    Session["idsc"] = null;
                    Session["idtime"] = null;
                    TempData["Error"] = "Ghế đã có người vừa đặt, Vui lòng chọn ghế khác!";
                    return RedirectToAction("Index", "Home");
                }

                //add order
                var sc = db.suat_chieu.Find(idsuatchieu);
                order addorder = new order();
                addorder.id_khachhang = Convert.ToInt32(TempData["idkh"].ToString());
                addorder.id_phim = sc.phim_id;
                addorder.ten_phim = sc.phim.ten_phim;
                addorder.id_phong_chieu = sc.phong_chieu_id;
                addorder.ten_phong_chieu = sc.phong_chieu.ten_phong;
                addorder.suatchieu_id = idsuatchieu;
                addorder.ngay_mua = DateTime.Now;
                addorder.pay_method = "Thanh toán Momo";
                addorder.status = 1;
                addorder.idtime = idtimechieu;
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
                Session["idghe"] = null;
                Session["idsc"] = null;
                Session["idtime"] = null;


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
                    TempData["Message"] = "Thanh toán thành công!";
                    return RedirectToAction("TransHistory", "User", new { id = kh.id });
                }
                catch (Exception)
                {

                    TempData["Message"] = "Thanh toán thành công!";
                    return RedirectToAction("TransHistory", "User", new { id = kh.id });
                }

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