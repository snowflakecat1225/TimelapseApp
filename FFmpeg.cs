using System.Diagnostics;

namespace TimelapseApp
{
    public static class FFmpeg
    {
        private static string _ffmpegPath;
        public static string Path
        {
            get { return _ffmpegPath; }
            set { _ffmpegPath = value; }
        }
        
        public static void Recording(string sourceLink, string outputPath, int recordingTime)
        {
            bool localTimeChecked = Config.GetTimestampChecked();
            string args = 
                $"-y -rtsp_transport tcp -i {sourceLink} -c:v libx264 -b:v 16k -r 1 -an -tune zerolatency -preset ultrafast -crf 25" +
                $"{(localTimeChecked ? "-vf drawtext=\"font=Arial:fontsize=70:fontcolor=white:x=0:y=0:text='%{localtime\\:%b %d %Y %H\\\\\\\\\\:%M\\\\\\\\\\:%S}':box=1:boxcolor=black@0.4:boxborderw=5\" " : "")}" +
                $"-t {recordingTime} {outputPath}";
            
            ProcessStartInfo startInfo = new()
            {
                FileName = _ffmpegPath,
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
            };

            var ffmpeg = Process.Start(startInfo)!;
            
            ffmpeg.WaitForExit();
        }
    }
}
