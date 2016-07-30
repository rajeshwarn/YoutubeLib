using System;

namespace YouTubeLib
{
    public class FormatNotFoundException : Exception
    {
        public FormatNotFoundException() : base("포맷을 찾을 수 없습니다")
        {

        }
    }
}
