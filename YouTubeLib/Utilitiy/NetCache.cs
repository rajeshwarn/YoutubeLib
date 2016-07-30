using System.Collections.Generic;

namespace YouTubeLib.Utilitiy
{
    public static class NetCache
    {
        private static Dictionary<string, string> cache = new Dictionary<string, string>();

        public static bool HasCache(string url)
        {
            return cache.ContainsKey(url.ToLower());
        }

        public static void Set(string url, string data)
        {
            cache[url.ToLower()] = data;
        }

        public static string Get(string url)
        {
            if (HasCache(url))
                return cache[url.ToLower()];

            return null;
        }

        public static void Clear()
        {
            cache.Clear();
        }
    }
}
