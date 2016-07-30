using System;
using System.Threading.Tasks;
using YouTubeLib;

namespace YouTubeLibSample
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Youtube URL: ");
                string url = Console.ReadLine();

                Task.WaitAll(extract(url));
            }
        }

        static async Task extract(string url)
        {
            try
            {
                var arr = await Extractor.Extract(url);

                if (arr.Length > 0)
                {
                    var fc = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;

                    foreach (var s in arr)
                    {
                        if (s.AdaptiveMime == AdaptiveMime.Audio)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Bitrate: {s.Bitrate}");
                            Console.WriteLine($"Format: {s.AdaptiveMime.ToString()}");
                            Console.WriteLine($"Mime: {s.VideoMime}");
                            Console.WriteLine(s.Url);
                        }
                    }

                    Console.ForegroundColor = fc;
                }
                else
                {
                    Console.WriteLine("추출 결과가 없습니다.");
                }
            }
            catch (InvalidUrlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (UnableVideoException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (RemovedAudioException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (FormatNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" > Unhandled Exception ");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
