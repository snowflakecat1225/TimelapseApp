using System;
using System.IO;

namespace TimelapseApp
{
    class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            if (args.Length == 3 && args.ContainsOnlyNumbers())
                TimelapseScript.Run(
                    int.Parse(args[0]),
                    int.Parse(args[1]),
                    int.Parse(args[2]));
            else MainWindow.Init();
            
            /*FFmpeg.Recording(
                "rtsp://link",
                "/home/snowflakecat/video.mp4",
                60);
            
            Task.Delay(5000).Wait();
            System.Console.WriteLine("I'm gay!!!");*/
        }
    }
}
