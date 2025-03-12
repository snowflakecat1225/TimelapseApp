using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Gtk;

namespace TimelapseApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            using Mutex mutex = new(true, Process.GetCurrentProcess().ProcessName, out bool createdNew);
            if (!createdNew)
                return;

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
            List<string> ffprobeSearch = Directory.EnumerateFiles(directory, "ffprobe", SearchOption.AllDirectories).ToList();

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

            if (ffprobeSearch.Count > 0 && !string.IsNullOrEmpty(ffprobeSearch.First()))
                FFprobe.Exists = true;
            else
            {
                FFprobe.Exists = false;
                errorMessages.Add("There is not installed FFprobe");
            }

            directory = "/var/spool/cron";

            List<string> crontab;
            try
            {
                crontab = Directory.EnumerateFiles(directory, Environment.UserName, SearchOption.AllDirectories).ToList();
            }
            catch
            {

                Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"chown -R {Environment.UserName} {directory}\"")).WaitForExit();
                crontab = Directory.EnumerateFiles(directory, Environment.UserName, SearchOption.AllDirectories).ToList();
            }

            if (!string.IsNullOrEmpty(crontab.FirstOrDefault()))
                Crontab.Path = crontab.FirstOrDefault();
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