using CinemaBooking.Library;
using CinemaBooking.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

            //tổng tiền
            decimal? price = 0;
            foreach (var i in idghengoi)
            {
                var priceghe = db.ghe_ngoi.Find(Convert.ToInt32(i));
                price += (priceghe.gia + priceghe.loai_ghe.phu_thu);
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
            addorder.tong_tien = price;
            addorder.so_luong_ve = idghengoi.Count();
            //code ticket
            Random random = new Random();

            bool check = false;
            while (check == false)
            {
                int length = 15;
                const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
                string codeticket = new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
                if (db.orders.Where(n => n.code_ticket == codeticket).Count() == 0)
                {
                    addorder.code_ticket = codeticket;
                    check = true;
                }
            }

            //Mã QR Code
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(addorder.code_ticket, QRCodeGenerator.ECCLevel.Q);

            string path = Server.MapPath("~/images/qrcode/");
            QrCodeInfo.SaveRawData(path + addorder.code_ticket + ".qrr", QRCodeData.Compression.Uncompressed);

            db.orders.Add(addorder);
            db.SaveChanges();
            //add order details
            int idorder = addorder.id;
            order_details addorderdetails = new order_details();
            foreach (var gheid in idghengoi)
            {
                var tiendetail = db.ghe_ngoi.Find(gheid);
                addorderdetails.id_ghe = gheid;
                addorderdetails.id_orders = idorder;
                addorderdetails.gia_ve = tiendetail.gia + tiendetail.loai_ghe.phu_thu;
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
                return RedirectToAction("Success", "ReceptionPayment", new { idord = addorder.id });
            }
            catch (Exception)
            {

                TempData["Message"] = "Đặt vé thành công!";
                return RedirectToAction("Success", "ReceptionPayment", new { idord = addorder.id });
            }
        }

        public ActionResult Success(int? idord)
        {
            if (Session["TenCus"] == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập";
                return RedirectToAction("SignIn", "User");
            }

            if (idord == null)
            {
                TempData["Warning"] = "Không phải tài khoản của bạn";
                return RedirectToAction("Index", "Home");
            }

            var makh = Convert.ToInt32(Session["MaKH"]);
            var ord = db.orders.Find(idord);
            if (ord.id_khachhang != makh)
            {
                TempData["Warning"] = "Không phải tài khoản của bạn";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.kh = db.khach_hang.Find(ord.id_khachhang);
            ViewBag.film = db.phims.Find(ord.id_phim);
            var dsghe = db.order_details.Where(x => x.id_orders == ord.id);
            int count = dsghe.Count();
            int dem = 0;
            string dayghe = "";
            foreach (var i in dsghe)
            {
                dem++;
                var ghe = db.ghe_ngoi.Find(i.id_ghe);
                if (dem == count)
                {
                    dayghe += ghe.Row + ghe.Col;
                }
                else
                {
                    dayghe += ghe.Row + ghe.Col + ", ";
                }
            }


            string qRCode = "Vé xem phim: ghế " + dayghe + ". Trạng thái chưa thanh toán!";


            string path = Server.MapPath("~/images/qrcode/");

            QRCodeData qrCodeData1 = new QRCodeData(path + ord.code_ticket + ".qrr", QRCodeData.Compression.Uncompressed);
            QRCode QrCode = new QRCode(qrCodeData1);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));

            ViewBag.QrCodeUri = QrUri;
            ViewBag.ghe = dayghe;
            TimeSpan tinhgio = new TimeSpan(0, 15, 0); // 15 phút
            var tinhthem = ord.ngay_mua + tinhgio;
            ViewBag.timetopay = Convert.ToDateTime(tinhthem).ToString("HH:mm");
            ViewBag.time = Convert.ToDateTime(tinhthem).ToString("MM/dd/yyyy HH:mm:ss");
            return View(ord);
        }

    }

    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {

            using (MemoryStream ms = new MemoryStream())
            {

                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }



}