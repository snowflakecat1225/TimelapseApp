using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TimelapseApp
{
    public class FFplay
    {
        public static bool Exists { get; set; }

        public static async Task<Task<Process>> Play(string link)
        {
            string args = "-rtsp_transport tcp -hide_banner -maxrate 500k -an -tune zerolatency -preset ultrafast -crf 25 " + link;

            using Process ffplay = new();
            ffplay.StartInfo = new ProcessStartInfo("ffplay", args) { RedirectStandardError = true };
            ffplay.Start();
            await ffplay.WaitForExitAsync();

            string errorMessage = ffplay.StandardError.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries)[^1];

            //if (errorMessage.Length < 200)
            if (!errorMessage.Contains("KB aq=") && !errorMessage.Contains("KB vq=") && !errorMessage.Contains("KB sq="))
                errorMessage.Message();

            return Task.FromResult(ffplay);
        }
    }
}