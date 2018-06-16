using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using GMS.Common.Log;
using System.Xml;

namespace OMH.WCS.Helper
{
    /// <summary>
    /// API帮助类
    /// </summary>
    public static class APIHelper
    {
        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static string PostHttp(string url,string request)
        {
            try
            {
                HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
                webrequest.Method = "post";
                webrequest.ContentType = "application/json; charset=utf-8";
                //webrequest.ContentType = "text/xml";
                webrequest.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.79 Safari/537.4");
                //string paraUrlCoded = System.Web.HttpUtility.UrlEncode("InPlanJson=");
                //paraUrlCoded += "" + System.Web.HttpUtility.UrlEncode("{\"InPlanJson\":\"" + request+"\"}");
                //string paraUrlCoded = System.Web.HttpUtility.UrlEncode(request);

                //request = System.Web.HttpUtility.UrlEncode(request) ;

                //string json = "{\"InPlanJson\":\"" + request + "\"}";

                byte[] postdatabyte = Encoding.UTF8.GetBytes(request);// "args ="+request);
                webrequest.ContentLength = postdatabyte.Length;

                Stream stream;
                stream = webrequest.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                //HttpWebResponse res;
                //try
                //{
                //    res = (HttpWebResponse)webrequest.GetResponse();
                //}
                //catch (WebException ex)
                //{
                //    res = (HttpWebResponse)ex.Response;
                //}

                //StreamReader s = new StreamReader(res.GetResponseStream());

                //string resss = s.ReadToEnd();

                using (var httpWebResponse = webrequest.GetResponse())
                using (StreamReader responseStream = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    String ret = responseStream.ReadToEnd();
                    return ret;
                }
            }
            catch(Exception ex)
            {
                LogFactory.GetLogger(typeof(APIHelper), ex.Message);
                LogFactory.GetLogger(typeof(APIHelper), ex.StackTrace);
                return "";
            }

        }
        /// <summary>
        /// Get提交数据
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static string GetHttp(string pic, float weight)
        {
            try
            {
                string url = "http://192.168.11.10:8080/wcsapi/SlidewayPort?";
                string para = "packsNo=" + pic.ToString() + "&weight=" + weight;
                var webRequest = (HttpWebRequest)WebRequest.Create(url + para);
                webRequest.KeepAlive = false;
                webRequest.Timeout = 300000;
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Accept = "application/json;charset=utf-8";
                webRequest.ContentLength = 0;

                //接收返回信息：
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                StreamReader sreader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return sreader.ReadToEnd();
            }
            catch
            {
                return "";
            }

        }


    }
}
