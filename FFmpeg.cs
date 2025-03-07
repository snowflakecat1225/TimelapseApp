using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TimelapseApp
{
    public static class FFmpeg
    {
        public static bool Exists { get; set; }

        public static void Record(string sourceLink, string outputPath, int recordingTime)
        {
            bool localTimeChecked = Config.GetTimestampChecked();

            string localTimeString = "-vf drawtext=\"font=Arial:fontsize=70:fontcolor=white:x=0:y=0:text='%{localtime\\:%b %d %Y %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.4:boxborderw=5\" ";
            string args =
                $"-y -rtsp_transport tcp -hide_banner  -i {sourceLink} -c:v libx264 -b:v 16k -r 1 -an -tune zerolatency -preset ultrafast -crf 25 " +
                $"{(localTimeChecked ? localTimeString : "")}" +
                $"-t {recordingTime} {outputPath}";

            ProcessStartInfo startInfo = new("ffmpeg") { Arguments = args };

            using Process ffmpeg = Process.Start(startInfo);
            ffmpeg.WaitForExit();
        }

        public static void Accelerate(string sourcePath, string resultPath, int acceleration)
        {
            string args = $"-y -i {sourcePath} -vf setpts=PTS/{acceleration} -an {resultPath}";

            ProcessStartInfo startInfo = new("ffmpeg") { Arguments = args };

            using Process ffmpeg = Process.Start(startInfo);
            ffmpeg.WaitForExit();
        }

        public static void Concat(List<string> sourseVideos, string resultPath)
        {
            sourseVideos.Sort();

            string concatFilePath = Path.Combine(Temp.Path, "concat.txt");
            using (StreamWriter sw = new(concatFilePath))
            {
                foreach (string video in sourseVideos)
                    sw.WriteLine($"file '{video}'");
            }

            string args = $"-y -f concat -safe 0 -i {concatFilePath} -c copy {resultPath}";

            ProcessStartInfo startInfo = new("ffmpeg") { Arguments = args };

            using Process ffmpeg = Process.Start(startInfo);
            ffmpeg.WaitForExit();

            File.Delete(concatFilePath);
        }
    }
}