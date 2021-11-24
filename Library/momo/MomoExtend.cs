using Newtonsoft.Json.Linq;
using System;
using System.Configuration;

namespace CinemaOnline.Handler.Payment.Momo
{
    public class MomoExtend
    {
        public static string GenUrlPay(string amount)
        {
            //request params need to request to MoMo system



            string orderid = Guid.NewGuid().ToString();
            string endpoint = ConfigurationManager.AppSettings["endpoint"].ToString();

            const string partnerCode = "MOMOU01S20211118";
            string orderInfo = "DH" + DateTime.Now.ToString("yyyyMMHHmmss");
            const string accessKey = "yrBFMKMIhgzB9gQl";
            const string serectKey = "veQGtn0ePwp2ypgpzFRorGMk9aZiKPtk";
            string returnUrl = ConfigurationManager.AppSettings["returnUrl"].ToString();
            string notifyurl = ConfigurationManager.AppSettings["notifyUrl"].ToString();
            //Before sign HMAC SHA256 signature
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
            string signature = Cryto.SignSha256(rawHash, serectKey);

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

            string responseFromMomo = PaymentRequest.SendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return jmessage.GetValue("payUrl").ToString();

            //var crypto = new MoMoSecurity();
            ////sign signature SHA256
            //var signature = crypto.SignSha256(rawHash, serectKey);

            ////build body json request
            //var message = new JObject
            //{
            //    { "partnerCode", partnerCode },
            //    { "accessKey", accessKey },
            //    { "requestId", requestId },
            //    { "amount", amount },
            //    { "orderId", orderId },
            //    { "orderInfo", orderInfo },
            //    { "returnUrl", returnUrl },
            //    { "notifyUrl", notifyUrl },
            //    { "extraData", extraData },
            //    { "requestType", "captureMoMoWallet" },
            //    { "signature", signature }
            //};
            //var responseFromMomo = PaymentRequest.SendPaymentRequest(endPoint, message.ToString());

            //var jMessage = JObject.Parse(responseFromMomo);
            //return jMessage.GetValue("payUrl").ToString();
        }
    }
}