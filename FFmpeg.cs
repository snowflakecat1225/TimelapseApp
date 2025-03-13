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
            if (!sourceLink.IsRtspLinkValid())
                return Task.FromResult(new InvalidDataException("RTSP-link is invalid"));

            if (recordingTime < 1)
            {
                $"[FFmpeg.Record()]: The video time index {recordingTime} is lower than it could be".Message();
                return Task.FromException(new IndexOutOfRangeException($"The acceleration index {recordingTime} is lower than it could be"));
            }

            bool localTimeChecked = Config.GetTimestampChecked();

            string localTimeString = "-vf drawtext=\"font=Arial:fontsize=70:fontcolor=white:x=0:y=0:text='%{localtime\\:%b %d %Y %H\\\\\\:%M\\\\\\:%S}':box=1:boxcolor=black@0.4:boxborderw=5\" ";
            string args =
                $"-y -rtsp_transport tcp -hide_banner -i {sourceLink} -movflags faststart -c:v libx264 -maxrate 1000k -bufsize 2000k -r 1 -an -tune zerolatency -preset ultrafast -crf 25 " +
                $"{(localTimeChecked ? localTimeString : "")}" +
                $"-t {recordingTime} {outputPath}";

            using Process record = new();
            record.StartInfo = new("ffmpeg", args);
            record.Start();
            record.WaitForExit();

            if (!File.Exists(outputPath))
                $"[FFmpeg.Record()]: Video was not recorded".Message();

            return Task.FromResult(record);
        }

        public static Task Accelerate(string sourcePath, string outputPath, int acceleration)
        {
            if (!File.Exists(sourcePath))
            {
                $"[FFmpeg.Accelerate()]: File {sourcePath} was not found".Message();
                return Task.FromException(new FileNotFoundException("File was not found", sourcePath));
            }

            if (acceleration < 1)
            {
                $"[FFmpeg.Accelerate()]: The acceleration index {acceleration} is lower than it could be".Message();
                return Task.FromException(new IndexOutOfRangeException($"The acceleration index {acceleration} is lower than it could be"));
            }

            string args = $"-y -hide_banner -i {sourcePath} -c:v libx264 -r 24 -vf setpts=PTS/{acceleration} -an {outputPath}";

            using Process accelerate = new();
            accelerate.StartInfo = new("ffmpeg", args);
            accelerate.Start();
            accelerate.WaitForExit();

            if (!File.Exists(outputPath))
                $"[FFmpeg.Accelerate()]: Video was not accelerated".Message();

            return Task.FromResult(accelerate);
        }

        public static Task Concat(List<string> sourseVideos, string outputPath)
        {
            if (sourseVideos.Count == 0)
            {
                $"[FFmpeg.Concat()]: There is no videos to concat".Message();
                return Task.FromException(new FileNotFoundException("File was not found", outputPath));
            }
            else
            {
                foreach (string sourseVideo in sourseVideos)
                {
                    if (!File.Exists(sourseVideo))
                    {
                        $"[FFmpeg.Concat()]: File {sourseVideo} was not found".Message();
                        return Task.FromException(new FileNotFoundException("File was not found", sourseVideo));
                    }
                }
            }

            string concatFilePath = Path.Combine(Temp.Path, "concat.txt");
            using (StreamWriter sw = new(concatFilePath))
            {
                foreach (string video in sourseVideos)
                    sw.WriteLine($"file '{video}'");
            }

            string args = $"-y -hide_banner -f concat -safe 0 -i {concatFilePath} -c copy {outputPath}";

            using Process concat = new();
            concat.StartInfo = new("ffmpeg", args);
            concat.Start();
            concat.WaitForExit();

            if (File.Exists(outputPath))
            {
                try
                {
                    File.Delete(concatFilePath);
                }
                catch (Exception ex)
                {
                    ("[FFmpeg.Concat.File.Delete()]: " + ex.Message).Message();
                }
            }
            else "[FFmpeg.Concat()]: The videos were not concated".Message();

            return Task.FromResult(concat);
        }

        public static Task Repair(string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                $"[FFmpeg.Repair()]: File {sourcePath} was not found".Message();
                return Task.FromException(new FileNotFoundException("File was not found", sourcePath));
            }

            string repairedVideoPath = Path.Combine(Temp.Path, "repairedVideo.mp4");
            string args = $"-y -hide_banner -i {sourcePath} -c:v copy -an -copyts -start_number 0 {repairedVideoPath}";

            using Process repair = new();
            repair.StartInfo = new("ffmpeg", args);
            repair.Start();
            repair.WaitForExit();

            if (File.Exists(repairedVideoPath))
            {
                try
                {
                    File.Move(repairedVideoPath, sourcePath, true);
                }
                catch (Exception ex)
                {
                    ("[FFmpeg.Repair.File.Move()]: " + ex.Message).Message();
                }
            }
            else "[FFmpeg.Repair()]: The video was not repaired".Message();

            return Task.FromResult(repair.ExitCode);
        }
    }
}