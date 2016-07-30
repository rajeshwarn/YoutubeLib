using System;

namespace YouTubeLib
{
    public class UnableVideoException : Exception
    {
        public UnableVideoException() : base("삭제된 동영상 입니다.")
        {

        }
    }
}
