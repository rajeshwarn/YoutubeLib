using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace System.Net
{
    public class HttpHelper
    {
        private const string QueryPattern = @"(\w+)\=([^&]+)";

        private static HttpRequestHeader[] knownHeaders =
            new HttpRequestHeader[]
            {
                HttpRequestHeader.KeepAlive,
                HttpRequestHeader.ContentLength,
                HttpRequestHeader.Host,
                HttpRequestHeader.ContentType,
                HttpRequestHeader.TransferEncoding,
                HttpRequestHeader.Connection,
                HttpRequestHeader.Accept,
                HttpRequestHeader.Referer,
                HttpRequestHeader.UserAgent,
                HttpRequestHeader.Expect,
                HttpRequestHeader.IfModifiedSince,
                HttpRequestHeader.Date
            };

        private static Dictionary<HttpRequestHeader, PropertyInfo> headerProps =
            typeof(HttpWebRequest)
                .GetProperties()
                .Where(pi => pi.Name.ToEnum<HttpRequestHeader>().HasValue)
                .ToDictionary(pi => pi.Name.ToEnum<HttpRequestHeader>().Value);

        private CookieContainer cookie;

        public bool CookieMaintain { get; }

        #region 생성자
        public HttpHelper(bool cookieMaintain)
        {
            cookie = new CookieContainer();

            this.CookieMaintain = cookieMaintain;
        }

        public HttpHelper() : this(true)
        {
        }
        #endregion

        #region 사용자 함수
        public async Task<string> POST(string uri, byte[] postData, WebHeaderCollection whc = null, Encoding encoding = null)
        {
            return await Request(uri, "POST", postData, whc, encoding);
        }

        public async Task<string> GET(string uri, WebHeaderCollection whc = null, Encoding encoding = null)
        {
            return await Request(uri, "GET", null, whc, encoding);
        }

        private async Task<string> Request(string uri, string method, byte[] postData = null, WebHeaderCollection whc = null, Encoding encoding = null)
        {
            try
            {
                encoding = encoding ?? Encoding.Default;

                var req = WebRequest.Create(uri);

                req.Method = method;

                if (CookieMaintain)
                    (req as HttpWebRequest).CookieContainer = cookie;

                if (whc != null)
                    SetHeaderCollection(req, whc);

                if (method.AnyEquals("post") && postData != null)
                {
                    req.ContentLength = postData.Length;

                    var req_s = await req.GetRequestStreamAsync();
                    await req_s.WriteAsync(postData, 0, postData.Length);
                }

                var res = await req.GetResponseAsync();

                if (CookieMaintain)
                {
                    foreach (Cookie c in (res as HttpWebResponse).Cookies)
                    {
                        cookie.Add(c);
                    }
                }

                using (var sr = new StreamReader(res.GetResponseStream(), encoding))
                {
                    return await sr.ReadToEndAsync();
                }
            }
            catch
            {
                return null;
            }
        }

        private void SetHeaderCollection(WebRequest req, WebHeaderCollection whc)
        {
            foreach (var header in whc.AllKeys)
            {
                if (HttpKnownHeaderNames.ValueNames.ContainsKey(header))
                {
                    var reqHeader = HttpKnownHeaderNames.ValueNames[header];

                    if (knownHeaders.Contains(reqHeader) && headerProps.ContainsKey(reqHeader))
                    {
                        headerProps[reqHeader].SetValue(req, whc[header]);
                    }
                    else
                    {
                        req.Headers.Add(whc[header]);
                    }
                }
                else
                {
                    req.Headers.Add(whc[header]);
                }
            }
        }
        #endregion

        #region 공용 함수
        public static Dictionary<string, string> ParseQueryString(string data)
        {
            var dict = new Dictionary<string, string>();

            foreach (Match m in Regex.Matches(data, QueryPattern))
            {
                dict[m.Groups[1].Value] = m.Groups[2].Value;
            }

            return dict;
        }

        public static string UrlDecode(string encodedValue, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                encodedValue = HttpUtility.UrlDecode(encodedValue);
            }

            return encodedValue;
        }

        public static string UrlEncode(string value, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                value = HttpUtility.UrlEncode(value);
            }

            return value;
        }
        #endregion
    }
}