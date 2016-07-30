using System.Collections.Generic;

namespace YouTubeLib
{
    public class VideoData
    {
        #region 포맷 데이터
        private static Dictionary<int, int[]> Presets =
            new Dictionary<int, int[]>()
            {
                {5, new int[] { (int)VideoMime.Flash, 240, 0, (int)AudioMime.MP3, 64, (int)AdaptiveMime.None } },
                {6, new int[] { (int)VideoMime.Flash, 270, 0, (int)AudioMime.MP3, 64, (int)AdaptiveMime.None } },
                {13, new int[] { (int)VideoMime.Mobile, 0, 0, (int)AudioMime.AAC, 0, (int)AdaptiveMime.None } },
                {17, new int[] { (int)VideoMime.Mobile, 144, 0, (int)AudioMime.AAC, 24, (int)AdaptiveMime.None } },
                {18, new int[] { (int)VideoMime.Mp4, 360, 0, (int)AudioMime.AAC, 96, (int)AdaptiveMime.None } },
                {22, new int[] { (int)VideoMime.Mp4, 720, 0, (int)AudioMime.AAC, 192, (int)AdaptiveMime.None } },
                {34, new int[] { (int)VideoMime.Flash, 360, 0, (int)AudioMime.AAC, 128, (int)AdaptiveMime.None } },
                {35, new int[] { (int)VideoMime.Flash, 480, 0, (int)AudioMime.AAC, 128, (int)AdaptiveMime.None } },
                {36, new int[] { (int)VideoMime.Mobile, 240, 0, (int)AudioMime.AAC, 38, (int)AdaptiveMime.None } },
                {37, new int[] { (int)VideoMime.Mp4, 1080, 0, (int)AudioMime.AAC, 192, (int)AdaptiveMime.None } },
                {38, new int[] { (int)VideoMime.Mp4, 3072, 0, (int)AudioMime.AAC, 192, (int)AdaptiveMime.None } },
                {43, new int[] { (int)VideoMime.WebM, 360, 0, (int)AudioMime.Vorbis, 128, (int)AdaptiveMime.None } },
                {44, new int[] { (int)VideoMime.WebM, 480, 0, (int)AudioMime.Vorbis, 128, (int)AdaptiveMime.None } },
                {45, new int[] { (int)VideoMime.WebM, 720, 0, (int)AudioMime.Vorbis, 192, (int)AdaptiveMime.None } },
                {46, new int[] { (int)VideoMime.WebM, 1080, 0, (int)AudioMime.Vorbis, 192, (int)AdaptiveMime.None } },

                {82, new int[] { (int)VideoMime.Mp4, 360, 1, (int)AudioMime.AAC, 96, (int)AdaptiveMime.None } },
                {83, new int[] { (int)VideoMime.Mp4, 240, 1, (int)AudioMime.AAC, 96, (int)AdaptiveMime.None } },
                {84, new int[] { (int)VideoMime.Mp4, 720, 1, (int)AudioMime.AAC, 152, (int)AdaptiveMime.None } },
                {85, new int[] { (int)VideoMime.Mp4, 520, 1, (int)AudioMime.AAC, 152, (int)AdaptiveMime.None } },
                {100, new int[] { (int)VideoMime.WebM, 360, 1, (int)AudioMime.Vorbis, 128, (int)AdaptiveMime.None } },
                {101, new int[] { (int)VideoMime.WebM, 360, 1, (int)AudioMime.Vorbis, 192, (int)AdaptiveMime.None } },
                {102, new int[] { (int)VideoMime.WebM, 720, 1, (int)AudioMime.Vorbis, 192, (int)AdaptiveMime.None } },

                {133, new int[] { (int)VideoMime.Mp4, 240, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {134, new int[] { (int)VideoMime.Mp4, 360, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {135, new int[] { (int)VideoMime.Mp4, 480, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {136, new int[] { (int)VideoMime.Mp4, 720, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {137, new int[] { (int)VideoMime.Mp4, 1080, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {138, new int[] { (int)VideoMime.Mp4, 2160, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {160, new int[] { (int)VideoMime.Mp4, 144, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {242, new int[] { (int)VideoMime.WebM, 240, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {243, new int[] { (int)VideoMime.WebM, 360, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {244, new int[] { (int)VideoMime.WebM, 480, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {247, new int[] { (int)VideoMime.WebM, 720, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {248, new int[] { (int)VideoMime.WebM, 1080, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {264, new int[] { (int)VideoMime.Mp4, 1440, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {271, new int[] { (int)VideoMime.WebM, 1440, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {272, new int[] { (int)VideoMime.WebM, 2160, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },
                {278, new int[] { (int)VideoMime.WebM, 144, 0, (int)AudioMime.Unknown, 0, (int)AdaptiveMime.Video } },

                {139, new int[] { (int)VideoMime.Mp4, 0, 0, (int)AudioMime.AAC, 48, (int)AdaptiveMime.Audio } },
                {140, new int[] { (int)VideoMime.Mp4, 0, 0, (int)AudioMime.AAC, 128, (int)AdaptiveMime.Audio } },
                {141, new int[] { (int)VideoMime.Mp4, 0, 0, (int)AudioMime.AAC, 256, (int)AdaptiveMime.Audio } },
                {171, new int[] { (int)VideoMime.WebM, 0, 0, (int)AudioMime.Vorbis, 128, (int)AdaptiveMime.Audio } },
                {172, new int[] { (int)VideoMime.WebM, 0, 0, (int)AudioMime.Vorbis, 192, (int)AdaptiveMime.Audio } },
            };
        #endregion

        internal string Signature { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public bool IsEncrypted { get; set; }

        public int FormatCode { get; set; }

        public VideoMime VideoMime { get; set; }

        public AudioMime AudioMime { get; set; }

        public AdaptiveMime AdaptiveMime { get; set; }

        public int Resolution { get; set; }

        public int Bitrate { get; set; }

        public bool Is3D { get; set; }

        public void SetFormat(int formatCode)
        {
            FormatCode = formatCode;

            if (Presets.ContainsKey(formatCode))
            {
                Binding(Presets[formatCode]);
            }
        }

        protected void Binding(int[] datas)
        {
            if (datas.Length == 6)
            {
                VideoMime = (VideoMime)datas[0];
                Resolution = datas[1];
                Is3D = (datas[2] == 1);
                AudioMime = (AudioMime)datas[3];
                Bitrate = datas[4];
                AdaptiveMime = (AdaptiveMime)datas[5];
            }
        }
    }
}
