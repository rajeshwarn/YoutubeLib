using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using YouTubeLib.Utilitiy;

namespace YouTubeLib
{
    public static class UrlResolver
    {
        private const string TokenPattern = @"([A-Za-z0-9-_=]{11})$";
        private const string NomalizedPattern = @"(?:http:\/\/)?youtube\.com\/watch\?v=" + TokenPattern;
        private const string TitlePattern = @"<title>(.+?)<\/title>";

        #region 내부 함수
        private static bool IsValidToken(string vToken)
        {
            return Regex.IsMatch(vToken, TokenPattern);
        }
        #endregion

        #region 사용자 함수
        public static async Task<bool> IsValidUrl(string url)
        {
            if (!Regex.IsMatch(url, NomalizedPattern))
                return false;

            var hp = new HttpHelper();
            string html = null;

            if (!NetCache.HasCache(url))
            {
                html = await hp.GET(url, encoding: Encoding.UTF8);

                NetCache.Set(url, html);
            }
            else
            {
                html = NetCache.Get(url);
            }

            if (!html.IsEmpty(true) && Regex.IsMatch(html, TitlePattern))
                return !Regex.Match(html, TitlePattern).Groups[1].Value.AnyEquals("Youtube");

            return false;
        }

        public static string Nomalize(string url)
        {
            var uri = new Uri(url.Trim());
            var query = HttpHelper.ParseQueryString(uri.Query);

            string host = uri.DnsSafeHost;
            string local = uri.LocalPath;
            string vToken = null;

            if ((host.AnyEquals("youtube.com") || host.AnyEquals("www.youtube.com")) &&
                    local.AnyEquals("/watch"))
            {
                if (query.ContainsKey("v"))
                {
                    vToken = query["v"];
                }
            }
            else if (host.AnyEquals("youtu.be"))
            {
                vToken = local.Substring(1);
            }
            else if (host.AnyEquals("www.youtube.com"))
            {
                if (local.AnyStartsWith("/embed/"))
                {
                    vToken = local.Split('/')[2];
                }
            }

            if (!vToken.IsEmpty() && IsValidToken(vToken))
            {
                var ub = new UriBuilder("http", "youtube.com")
                {
                    Path = "watch",
                    Query = $"v={vToken}"
                };

                return ub.ToString();
            }

            return null;
        }

        public static bool TryNomalize(string url, out string result)
        {
            return !(result = Nomalize(url)).IsEmpty();
        }
        #endregion
    }
}
