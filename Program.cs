using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Gtk;

namespace TimelapseApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            List<string> errorMessages = new();

            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable()) errorMessages.Add("There is no Internet connection");
            }
            catch (Exception ex)
            {
                errorMessages.Add($"[NetworkInterface.GetIsNetworkAvailable()]: {ex.Message}");
            }

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

            string crontabPath = Path.Combine(directory, Environment.UserName);
            if (Directory.Exists(directory) && File.Exists(crontabPath))
                Crontab.Path = crontabPath;
            else errorMessages.Add("There is not installed Crontab");

            if (args.Length == 3 && args.ContainsOnlyNumbers())
            {
                if (errorMessages.Count == 0)
                    Script.Run(
                        int.Parse(args[0]),
                        int.Parse(args[1]),
                        int.Parse(args[2]));
                else
                    foreach (string error in errorMessages)
                        error.Message();
            }
            else
            {
                Application.Init();
                Interface.IsInitialised = true;
                foreach (string error in errorMessages)
                    error.Message(1);
                Interface.Init();
                Application.Run();
            }
        }
    }
}