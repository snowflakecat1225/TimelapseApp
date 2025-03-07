using System;
using System.Diagnostics;

namespace TimelapseApp
{
    public class FFplay
    {
        public static bool Exists { get; set; }

        public static async void Play(string link)
        {
            string args = "-rtsp_transport tcp -hide_banner -b:v 16k -an -tune zerolatency -preset ultrafast -crf 25 " + link;

            ProcessStartInfo startInfo = new("ffplay")
            {
                Arguments = args,
                RedirectStandardError = true
            };

            using Process ffplay = Process.Start(startInfo);
            await ffplay.WaitForExitAsync();

            string errorMessage = ffplay.StandardError.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries)[^1];

            //if (errorMessage.Length < 200)
            if (!errorMessage.Contains("KB aq=") && !errorMessage.Contains("KB vq=") && !errorMessage.Contains("KB sq="))
                errorMessage.Message(1);
        }
    }
}