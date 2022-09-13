using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Instagram_Bot
{
    internal class HttpRequests
    {
        //Reading response Text
        public static string ReadResponse(HttpWebResponse response)
        {
            using(StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        //making a GET request to a url and returns the response
        public static HttpWebResponse Get(string url, Dictionary<string,string> headers = null, CookieContainer cookies = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.114 Safari/537.36";
            request.CookieContainer = cookies == null ? new CookieContainer() : cookies;

            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]); 
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        //making a POST request to a url and returns the response
        public static HttpWebResponse Post(string url, string data, Dictionary<string, string> headers = null, CookieContainer cookies = null)
        {
            HttpWebRequest request =  (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.CookieContainer = cookies == null ? new CookieContainer() : cookies;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.5060.114 Safari/537.36";
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if(key == "Content-Type")
                        request.ContentType = headers[key];
                    else if(key == "Referer")
                        request.Referer = headers[key];
                    else
                        request.Headers.Add(key, headers[key]);
                }
            }
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = dataBytes.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(dataBytes, 0, dataBytes.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        public static Dictionary<string, string> GetCookies(HttpWebResponse response)
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();

            foreach (Cookie cookie in response.Cookies)
            {
                try
                {
                    if (string.IsNullOrEmpty(cookie.Value))
                        continue;
                    cookies.Add(cookie.Name, cookie.Value);
                }
                catch { continue; }
            }

            return cookies;
        }

        public static CookieContainer ConvertToCookieContainer(Dictionary<string,string> cookies)
        {
            CookieContainer container = new CookieContainer();
            foreach (string key in cookies.Keys)
            {
                if (key.Trim().Length == 0 || cookies[key].Trim().Length == 0 || key == "sessionid")
                    continue;
                Cookie cookie = new Cookie(key, cookies[key]) { Domain = "www.instagram.com" };
                container.Add(cookie);
            }
            return container;
        }
    }
}
