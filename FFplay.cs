using System.Diagnostics;

namespace TimelapseApp
{
    public class FFplay
    {
        private static string _ffplayPath;
        public static string Path
        {
            get { return _ffplayPath; }
            set { _ffplayPath = value; }
        }

        public static async void Play(string link)
        {
            string args = "-rtsp_transport tcp -b:v 16k -an -tune zerolatency -preset ultrafast -crf 25 " + link;
            ProcessStartInfo startInfo = new()
            {
                FileName = _ffplayPath,
                Arguments = args
            };

            var ffplay = Process.Start(startInfo)!;
                        
            await ffplay.WaitForExitAsync();
        }
    }
}