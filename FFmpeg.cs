using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TimelapseApp
{
    public static class FFmpeg
    {
        public static bool Exists { get; set; }

        public static Task Record(string sourceLink, string outputPath, int recordingTime)
        {
            bool localTimeChecked = Config.GetTimestampChecked();

            string localTimeString = "-vf drawtext=\"font=Arial:fontsize=70:fontcolor=white:x=0:y=0:text='%{localtime\\:%b %d %Y %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.4:boxborderw=5\" ";
            string args =
                $"-y -rtsp_transport tcp -hide_banner -i {sourceLink} -movflags faststart -c:v libx264 -maxrate 1000k -bufsize 2000k -r 1 -an -tune zerolatency -preset ultrafast -crf 25 " +
                $"{(localTimeChecked ? localTimeString : "")}" +
                $"-t {recordingTime} {outputPath}";

            using Process record = Process.Start(new ProcessStartInfo("ffmpeg", args));
            record.WaitForExit();

            return Task.FromResult(record);
        }

        public static Task Accelerate(string sourcePath, string resultPath, int acceleration)
        {
            string args = $"-y -hide_banner -i {sourcePath} -c:v libx264 -r 25 -vf setpts=PTS/{acceleration} -an {resultPath}";

            using Process accelerate = Process.Start(new ProcessStartInfo("ffmpeg", args));
            accelerate.WaitForExit();

            return Task.FromResult(accelerate);
        }

        public static Task Concat(List<string> sourseVideos, string resultPath)
        {
            string concatFilePath = Path.Combine(Temp.Path, "concat.txt");
            using (StreamWriter sw = new(concatFilePath))
            {
                foreach (string video in sourseVideos)
                    sw.WriteLine($"file '{video}'");
            }

            string args = $"-y -hide_banner -f concat -safe 0 -i {concatFilePath} -c copy {resultPath}";

            using Process concat = Process.Start(new ProcessStartInfo("ffmpeg", args));
            concat.WaitForExit();

            File.Delete(concatFilePath);

            return Task.FromResult(concat);
        }

        public static Task Repair(string sourcePath)
        {
            string repairedVideoPath = Path.Combine(Temp.Path, "repairedVideo.mp4");
            string args = $"-y -hide_banner -i {sourcePath} -ss 0 -c:v copy -an -copyts -start_number 0 {repairedVideoPath}";

            using Process repair = Process.Start(new ProcessStartInfo("ffmpeg", args));
            repair.WaitForExit();

            try
            {
                File.Move(repairedVideoPath, sourcePath, true);
            }
            catch (Exception ex)
            {
                ("[FFmpeg.Repair.File.Move()]: " + ex.Message).Message();
            }

            return Task.FromResult(repair);
        }
    }
}