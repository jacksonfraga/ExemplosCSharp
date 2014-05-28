using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace jacksonf.Helpers
{
    public static class HttpREST
    {
        public static string Get(string url, Dictionary<string, string> headers, ProxyConfig proxyCfg)
        {
            HttpWebRequest req = CreateGetHttpWebRequest(url, proxyCfg);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    req.Headers.Add(item.Key, item.Value);
                }
            }

            string result = null;
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        public static string Post(string url, ProxyConfig proxyCfg)
        {
            return Post(url, string.Empty, null, proxyCfg);
        }

        public static string Post(string url, Dictionary<string, string> headers, ProxyConfig proxyCfg)
        {
            return Post(url, string.Empty, headers, proxyCfg);
        }

        public static string Post(string url, string jsonData, Dictionary<string, string> headers, ProxyConfig proxyCfg)
        {
            HttpWebRequest req = CreatePostHttpWebRequest(url, proxyCfg);
            string result = null;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    req.Headers.Add(item.Key, item.Value);
                }
            }

            using (StreamWriter streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)req.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    result = streamReader.ReadToEnd();
            }
            return result;
        }

        public static string Delete(string url, Dictionary<string, string> headers, ProxyConfig proxyCfg)
        {
            HttpWebRequest req = CreateDeleteHttpWebRequest(url, proxyCfg);
            string result = null;

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    req.Headers.Add(item.Key, item.Value);
                }
            }

            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }
            return result;
        }

        private static HttpWebRequest CreatePostHttpWebRequest(string url, ProxyConfig proxyCfg)
        {
            return CreateHttpWebRequest(url, "POST", proxyCfg);
        }

        private static HttpWebRequest CreateGetHttpWebRequest(string url, ProxyConfig proxyCfg)
        {
            return CreateHttpWebRequest(url, "GET", proxyCfg);
        }

        private static HttpWebRequest CreateDeleteHttpWebRequest(string url, ProxyConfig proxyCfg)
        {
            return CreateHttpWebRequest(url, "DELETE", proxyCfg);
        }

        private static HttpWebRequest CreateHttpWebRequest(string url, string method, ProxyConfig proxyCfg)
        {
            WebRequest request = HttpWebRequest.Create(new Uri(url));
            bool urlSpecific = false;
            if (proxyCfg != null)
                urlSpecific = !string.IsNullOrEmpty(proxyCfg.Ip) && proxyCfg.Port > 0;
            IWebProxy curProxy = request.Proxy;
            WebProxy newProxy = new WebProxy();
            if (proxyCfg != null && proxyCfg.Enabled)
            {
                if (urlSpecific)
                    newProxy.Address = new Uri(string.Format("http://{0}:{1}", proxyCfg.Ip, proxyCfg.Port));
                if (proxyCfg.RequireAuthentication)
                {
                    if (urlSpecific)
                        newProxy.Credentials = new NetworkCredential(proxyCfg.User, proxyCfg.Password, proxyCfg.Domain);
                    else
                        curProxy.Credentials = new NetworkCredential(proxyCfg.User, proxyCfg.Password, proxyCfg.Domain);
                }
                if (urlSpecific)
                    request.Proxy = newProxy;
            }
            request.Method = method;
            request.ContentType = "application/json";
            return (HttpWebRequest)request;
        }
    }
}
