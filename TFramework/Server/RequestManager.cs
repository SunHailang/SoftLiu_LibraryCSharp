using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TFramework.Server
{
    public static class RequestManager
    {
        public static HttpWebResponse RequestMethod(string url, string method, Dictionary<string, object> headers = null, Dictionary<string, object> cookies = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = method;
            request.Timeout = 30;
            if (headers != null)
            {
                foreach (KeyValuePair<string, object> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value.ToString());
                }
            }
            if (cookies != null && cookies.Count > 0)
            {
                CookieContainer cookieCon = new CookieContainer();
                foreach (KeyValuePair<string, object> cookie in cookies)
                {
                    Cookie ck = new Cookie(cookie.Key, cookie.Value.ToString());
                    cookieCon.Add(ck);
                }
                request.CookieContainer = cookieCon;
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }
        /// <summary>
        /// 获取"Content-disposition"的内的 name 的值
        /// </summary>
        /// <param name="disposition">"Content-disposition" 的信息</param>
        /// <param name="name">key值</param>
        /// <returns></returns>
        public static string GetContentDispositionByName(string disposition, string name)
        {
            string result = null;
            if (!string.IsNullOrEmpty(disposition) && !string.IsNullOrEmpty(name))
            {
                string[] disArray = disposition.Split(';');
                for (int i = 0; i < disArray.Length; i++)
                {
                    string[] keyvalues = disArray[i].Split('=');
                    if (keyvalues.Length == 2)
                    {
                        string key = keyvalues[0].Trim();
                        string value = keyvalues[1].Trim();
                        if (key.Equals(name))
                        {
                            result = value;
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
