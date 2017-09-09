using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SophtronClient
{
    public abstract class BaseClient
    {
        public abstract string BaseEndpoint { get; protected set; }
        public Func<string, string, List<KeyValuePair<string, string>>> _headerFunc = null;

        public static string Post(string url, string data, string specificType = null, List<KeyValuePair<string, string>> headers = null)
        {
            return ApiCall(url, "POST", data, specificType, headers);
        }

        public static string Get(string url, List<KeyValuePair<string, string>> headers = null)
        {
            return ApiCall(url, "GET", null, null, headers);
        }

        private static string ApiCall(string url, string httpMethod, string payload, string specificContentType = null, List<KeyValuePair<string, string>> headers = null, int timeout = 120000)
        {
            try
            {
                var endpoint = new Uri(url);
                var payloadData = string.IsNullOrEmpty(payload) ? null : Encoding.UTF8.GetBytes(payload);
                httpMethod = httpMethod.ToUpper();

                WebRequest request = HttpWebRequest.Create(endpoint.AbsoluteUri);
                request.Method = httpMethod;
                request.ContentLength = payloadData == null ? 0 : payloadData.Length;
                request.ContentType = specificContentType ?? @"application/json";
                request.Timeout = timeout;
                ((HttpWebRequest)request).UserAgent = @"Corp v1.0.0";
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        request.Headers[header.Key] = header.Value;
                    }
                }
                if (payloadData != null && payloadData.Length > 0)
                {
                    using (var reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(payloadData, 0, payloadData.Length);
                    }
                }

                WebResponse response = null;
                try
                {
                    response = request.GetResponse();
                }
                catch (WebException webEx)
                {
                    throw webEx;
                }
                if (((HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                {
                    throw new WebException(string.Format("GET|{0}|{1}|{2}", endpoint.AbsoluteUri, ((HttpWebResponse)response).StatusCode, ((HttpWebResponse)response).StatusDescription));
                }

                using (var data = response.GetResponseStream())
                using (var mem = new MemoryStream())
                {
                    var buff = new byte[102400];
                    int read = 0;
                    while (0 != (read = data.Read(buff, 0, buff.Length)))
                    {
                        mem.Write(buff, 0, read);
                    }
                    var str = Encoding.UTF8.GetString(mem.ToArray());
                    return str;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
