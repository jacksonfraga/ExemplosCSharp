using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace jacksonf.Helpers
{
    public static class HttpREST
    {
        public static string Get(string url, Dictionary<string, string> headers)
        {
            HttpWebRequest req = CreateGetHttpWebRequest(url);

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

        public static string Post(string url)
        {
            return Post(url, string.Empty, null);
        }

        public static string Post(string url, Dictionary<string, string> headers)
        {
            return Post(url, string.Empty, headers);
        }

        public static string Post(string url, string jsonData, Dictionary<string, string> headers)
        {
            HttpWebRequest req = CreatePostHttpWebRequest(url);
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

        public static string Delete(string url, Dictionary<string, string> headers)
        {
            HttpWebRequest req = CreateDeleteHttpWebRequest(url);
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

        private static HttpWebRequest CreatePostHttpWebRequest(string url)
        {
            return CreateHttpWebRequest(url, "POST");
        }

        private static HttpWebRequest CreateGetHttpWebRequest(string url)
        {
            return CreateHttpWebRequest(url, "GET");
        }

        private static HttpWebRequest CreateDeleteHttpWebRequest(string url)
        {
            return CreateHttpWebRequest(url, "DELETE");
        }

        private static HttpWebRequest CreateHttpWebRequest(string url, string method)
        {
            WebRequest request = HttpWebRequest.Create(new Uri(url));
            request.Method = method;
            request.ContentType = "application/json";
            return (HttpWebRequest)request;
        }
    }
}
