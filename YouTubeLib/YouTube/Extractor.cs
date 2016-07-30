using System;
using System.Net;
using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using YouTubeLib.Utilitiy;

namespace YouTubeLib
{
    public static class Extractor
    {
        private const string RateBypass = "ratebypass";
        private const string Signature = "signature";
        private const string Fallback = "fallback_host";

        private const string UnAvailableVideo = "<div id=\"watch-player-unavailable\">";
        private const string VideoDataPattern = @"ytplayer.config ?= ?(\{.+?\});";

        #region 내부 함수
        private static bool IsVideoUnavailable(string html)
        {
            return html.Contains(UnAvailableVideo);
        }

        private static string ParseVideoData(string html)
        {
            if (Regex.IsMatch(html, VideoDataPattern))
                return Regex.Match(html, VideoDataPattern).Groups[1].Value;

            return null;
        }

        private static string ParseStream(JObject obj)
        {
            JToken jStream = obj["args"]["url_encoded_fmt_stream_map"];

            if (jStream != null)
            {
                string stream = jStream.Value<string>();

                if (stream.Contains("been+removed"))
                    throw new RemovedAudioException();

                return stream;
            }

            return null;
        }

        private static string ParseAdaptiveStream(JObject obj)
        {
            JToken jStream = obj["args"]["adaptive_fmts"];

            if (jStream == null)
                return obj["args"]["url_encoded_fmt_stream_map"].Value<string>();

            return jStream.Value<string>();
        }

        private static async Task<VideoData[]> ParseDownloadUrls(JObject obj)
        {
            var results = new List<VideoData>();

            string title = obj["args"]["title"].Value<string>();

            // 스트림 주소들
            string stream = ParseStream(obj);
            string aStream = ParseAdaptiveStream(obj);

            IEnumerable<string> rawQueries = stream.Split(',').Concat(aStream.Split(','));

            // rate-bypass는 한곳에만 존재해도 모든 주소에 적용가능
            bool hasBypass = rawQueries.Count(q => q.Contains(RateBypass)) > 0;

            foreach (var rawQuery in rawQueries)
            {
                var query = HttpHelper.ParseQueryString(rawQuery);

                string url = query["url"];
                bool isEncrypted = query.ContainsKey("s");

                // url 조합
                if (isEncrypted || query.ContainsKey("sig"))
                {
                    string signature = isEncrypted ? query["s"] : query["sig"];
                    bool hasFallback = query.ContainsKey(Fallback);

                    url = $"{url}&{Signature}={signature}";

                    if (hasFallback)
                        url = $"{url}&{Fallback}={query[Fallback]}";
                }

                url = HttpHelper.UrlDecode(url, 2);
                query = HttpHelper.ParseQueryString(url);

                var vd = new VideoData()
                {
                    Title = title,
                    Url = url,
                    IsEncrypted = isEncrypted
                };

                // 포맷 설정
                int formatCode;

                if (!int.TryParse(query["itag"], out formatCode))
                    throw new FormatNotFoundException();

                vd.SetFormat(formatCode);

                // 주소 복호화
                if (isEncrypted && query.ContainsKey(Signature))
                {
                    string jsUrl = $"http:{obj["assets"]["js"]?.Value<string>()}";

                    query[Signature] = await Decipher.Decrypt(query[Signature], jsUrl);
                }

                if (hasBypass)
                    query[RateBypass] = "yes";

                // 파라미터 빌드
                vd.Url = SetParams(vd.Url, query.ToArray());

                results.Add(vd);
            }

            return results.ToArray();
        }

        private static string SetParams(string url, params KeyValuePair<string, string>[] kvs)
        {
            var rBuilder = new UriBuilder(url);
            var rQuery = HttpUtility.ParseQueryString(rBuilder.Query);

            foreach (var kv in kvs)
                rQuery[kv.Key] = kv.Value;

            rBuilder.Query = rQuery.ToString();

            return rBuilder.ToString();
        }
        #endregion

        #region 사용자 함수
        public static async Task<VideoData[]> Extract(string url)
        {
            string vUrl;

            if (UrlResolver.TryNomalize(url, out vUrl))
            {
                string html;

                if (!await UrlResolver.IsValidUrl(vUrl))
                    throw new InvalidUrlException();

                // 캐싱
                if (NetCache.HasCache(vUrl))
                {
                    html = NetCache.Get(vUrl);
                }
                else
                {
                    var hp = new HttpHelper();
                    html = await hp.GET(vUrl);

                    NetCache.Set(vUrl, html);
                }

                // 정지된 유튜브 체크
                if (IsVideoUnavailable(html))
                    throw new UnableVideoException();

                // 스트림및 영상 데이터
                var obj = JObject.Parse(ParseVideoData(html));

                return await ParseDownloadUrls(obj);
            }
            else
            {
                throw new InvalidUrlException();
            }
        }
        #endregion
    }
}
