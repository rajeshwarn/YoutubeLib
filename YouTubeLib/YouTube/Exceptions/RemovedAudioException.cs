using System;

namespace YouTubeLib
{
    public class RemovedAudioException : Exception
    {
        public RemovedAudioException() : base("음원이 삭제된 영상입니다.")
        {

        }
    }
}
