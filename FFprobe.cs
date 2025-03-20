using System.Diagnostics;

namespace TimelapseApp
{
    public static class FFprobe
    {
        public static bool Exists { get; set; }

        private static readonly ProcessStartInfo startInfo = new("ffprobe") { RedirectStandardOutput = true };

        private static string ProcessResult()
        {
            using Process ffprobe = new();
            ffprobe.StartInfo = startInfo;
            ffprobe.Start();
            ffprobe.WaitForExit();

            return ffprobe.StandardOutput.ReadToEnd().Replace('\n', '\0');
        }

        public class GetInfo
        {
            public static double Duration(string videoPath)
            {
                FFmpeg.Repair(videoPath);
                startInfo.Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 {videoPath}";
                try { return double.Parse(ProcessResult()); }
                catch { return 0; }
            }
        }
    }
}