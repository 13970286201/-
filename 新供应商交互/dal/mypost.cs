using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace http请求服务
{
    class mypost
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受   
        }

        public static string GetHttpsWebRequest_ickd_cn(string url, CookieContainer cookies)
        {
            string result = "";

            HttpWebRequest req = null;
            //HTTPSQ请求
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);

            req = WebRequest.Create(url) as HttpWebRequest;
            req.ProtocolVersion = HttpVersion.Version10;
            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            // req.Host = "www.tianyancha.com";
            req.Referer = url;

            req.Method = "GET";
            req.UserAgent = DefaultUserAgent;
            req.Timeout = 8000;//设置请求超时时间，单位为毫秒

            req.ContentType = "text/html; charset=utf-8";

            req.CookieContainer = cookies;
            req.CookieContainer = new CookieContainer();

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();

            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            resp.Close();
            //cookies = req.CookieContainer;
            return result;
        }

        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode, CookieContainer cookies)
        {
            string ret = string.Empty;
            try
            {

                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                //multipart/form-data;
                webReq.Timeout = 300 * 1000;
                webReq.ReadWriteTimeout = 300 * 1000;
                webReq.CookieContainer = cookies;
                webReq.Accept = "*/*";
                // webReq.Connection = "keep-alive";
                webReq.Host = "117.40.178.51:8088";
                webReq.Referer = "http://117.40.178.51:8088/jjfsptweb/index.jsp";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

    }
}
