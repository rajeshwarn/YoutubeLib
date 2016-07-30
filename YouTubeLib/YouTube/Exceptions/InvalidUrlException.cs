using System;

namespace YouTubeLib
{
    public class InvalidUrlException : Exception
    {
        public InvalidUrlException() : base("잘못된 주소입니다.")
        {

        }
    }
}
