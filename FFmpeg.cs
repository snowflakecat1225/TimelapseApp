using System.Diagnostics;

namespace TimelapseApp
{
    public static class FFmpeg
    {
        public static bool Exists { get; set; }

        public static void Recording(string sourceLink, string outputPath, int recordingTime)
        {
            bool localTimeChecked = Config.GetTimestampChecked();
            string args = 
                $"-y -rtsp_transport tcp -hide_banner  -i {sourceLink} -c:v libx264 -b:v 16k -r 1 -an -tune zerolatency -preset ultrafast -crf 25" +
                $"{(localTimeChecked ? "-vf drawtext=\"font=Arial:fontsize=70:fontcolor=white:x=0:y=0:text='%{localtime\\:%b %d %Y %H\\\\\\\\\\:%M\\\\\\\\\\:%S}':box=1:boxcolor=black@0.4:boxborderw=5\" " : "")}" +
                $"-t {recordingTime} {outputPath}";
            
            ProcessStartInfo startInfo = new("ffmpeg")
            {
                Arguments = args,
                RedirectStandardError = true
            };

            using var ffmpeg = Process.Start(startInfo);
            ffmpeg.WaitForExit();
        }
    }
}
