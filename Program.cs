using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gtk;

namespace TimelapseApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<string> errorMessages = new();

            string directory = "/bin";

            List<string> ffmpegSearch = Directory.EnumerateFiles(directory, "ffmpeg", SearchOption.AllDirectories).ToList();
            List<string> ffplaySearch = Directory.EnumerateFiles(directory, "ffplay", SearchOption.AllDirectories).ToList();

            if (ffmpegSearch.Count > 0 && !string.IsNullOrEmpty(ffmpegSearch.First()))
                FFmpeg.Exists = true;
            else
            {
                FFmpeg.Exists = false;
                errorMessages.Add("There is not installed FFmpeg");
            }

            if (ffplaySearch.Count > 0 && !string.IsNullOrEmpty(ffplaySearch.First()))
                FFplay.Exists = true;
            else
            {
                FFplay.Exists = false;
                errorMessages.Add("There is not installed FFplay");
            }

            directory = "/var/spool/cron";

            var crontabPath = Path.Combine(directory, Environment.UserName);
            if (Directory.Exists(directory) && File.Exists(crontabPath))
                Crontab.Path = crontabPath;
            else errorMessages.Add("There is not installed Crontab");

            Temp.Path = Temp.GetDefaultPath();

            if (args.Length == 2 && args.ContainsOnlyNumbers())
            {
                if (errorMessages.Count == 0)
                    TimelapseScript.Run(
                        int.Parse(args[0]),
                        int.Parse(args[1]));
                else
                    foreach (var error in errorMessages)
                        Console.WriteLine(error);
            }
            else
            {
                Application.Init();

                foreach (var error in errorMessages)
                    error.Message(Interface.Main);
                
                Interface.Init();

                Application.Run();
            }

            //File.Create("/home/snowflakecat/1225.text");
        }
    }
}