using CinemaBooking.Library;
using CinemaBooking.Models;
using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace CinemaBooking.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        CinemaBookingEntities db = new CinemaBookingEntities();

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apicontext, string redirectURl, string idghe, string idsc, string idtime)
        {

            var ItemLIst = new ItemList() { items = new List<PayPal.Api.Item>() };
            var sc = db.suat_chieu.Find(Convert.ToInt32(idsc));
            var time = db.TimeFrames.Find(Convert.ToInt32(idtime));

            List<String> idghengoi = new List<String>();
            string[] listid = idghe.Split(',');
            for (int i = 0; i < listid.Length; i++)
            {
                if (listid[i] != "")
                {
                    idghengoi.Add(listid[i]);
                }
            }
            double count = idghengoi.Count();
            string ghe = "Vé ";
            var dem = 0;
            foreach (var j in idghengoi)
            {
                dem++;
                var tengh = db.ghe_ngoi.Find(Convert.ToInt32(j));
                if (dem == count)
                {
                    ghe += tengh.Row + tengh.Col;
                }
                else
                {
                    ghe += tengh.Row + tengh.Col + ", ";
                }
            }

            double tygia = 23300;
            double thanhtien = Math.Round((75000 * count) / tygia, 2);
            string tien = thanhtien.ToString().Replace(",", ".");

            ItemLIst.items.Add(new PayPal.Api.Item()
            {
                name = ghe + " " + sc.phim.ten_phim + " " + time.Time,
                currency = "USD",
                price = tien,
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectURl + "&Cancel=true",
                return_url = redirectURl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = tien
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = tien, // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = Convert.ToString((new Random()).Next(100000000)), //Generate an Invoice No  
                amount = amount,
                item_list = ItemLIst
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apicontext);

        }
        public ActionResult PaymentWithPaypal(string Cancel = null)
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
            //getting the apiContext
            APIContext apicontext = PaypalConfiguration.GetAPIContext();
            try
            {

                string PayerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(PayerId))
                {
                    string baseURi = Request.Url.Scheme + "://" + Request.Url.Authority +
                                     "/Payment/PaymentWithPaypal?";

                    var Guid = Convert.ToString((new Random()).Next(100000000));
                    var createdPayment = this.CreatePayment(apicontext, baseURi + "guid=" + Guid, idghe, idsc, idtime);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectURL = string.Empty;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectURL = lnk.href;
                        }


                    }
                    Session.Add(Guid, createdPayment.id);
                    return Redirect(paypalRedirectURL);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPaymnt = ExecutePayment(apicontext, PayerId, Session[guid] as string);

                    if (executedPaymnt.state.ToString().ToLower() != "approved")
                    {
                        TempData["Error"] = "Lỗi thanh toán!";
                        return RedirectToAction("Index", "Home");
                        //fail
                    }

                }
            }
            catch (PayPalException)
            {
                TempData["Error"] = "Lỗi thanh toán!";
                return RedirectToAction("Index", "Home");
                //fail

                //throw;
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
            addorder.idtime = idtimechieu;
            addorder.pay_method = "Thanh toán Paypal";
            addorder.status = 1;
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
                TempData["Message"] = "Thanh toán thành công!";
                return RedirectToAction("TransHistory", "User", new { id = kh.id });
            }
            catch (Exception)
            {

                TempData["Message"] = "Thanh toán thành công!";
                return RedirectToAction("TransHistory", "User", new { id = kh.id });
            }
        }
    }
}