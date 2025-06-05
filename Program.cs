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
                if (new Ping().Send("www.google.com").Status != IPStatus.Success)
                    errorMessages.Add(Language.GetPhrase(30));
            }
            catch
            {
                errorMessages.Add(Language.GetPhrase(30));
            }

            string directory = "/bin";

            List<string> ffmpegSearch = Directory.EnumerateFiles(directory, "ffmpeg", SearchOption.AllDirectories).ToList();
            List<string> ffplaySearch = Directory.EnumerateFiles(directory, "ffplay", SearchOption.AllDirectories).ToList();
            List<string> ffprobeSearch = Directory.EnumerateFiles(directory, "ffprobe", SearchOption.AllDirectories).ToList();

            if (ffmpegSearch.Count > 0)
                FFmpeg.Exists = true;
            else
            {
                FFmpeg.Exists = false;
                errorMessages.Add(Language.GetPhrase(31));
            }

            if (ffplaySearch.Count > 0)
                FFplay.Exists = true;
            else
            {
                FFplay.Exists = false;
                errorMessages.Add(Language.GetPhrase(32));
            }

            if (ffprobeSearch.Count > 0)
                FFprobe.Exists = true;
            else
            {
                FFprobe.Exists = false;
                errorMessages.Add(Language.GetPhrase(33));
            }

            directory = "/var/spool/cron";

            List<string> crontab;
            try
            {
                crontab = Directory.EnumerateFiles(directory, "Environment.UserName", SearchOption.AllDirectories).ToList();
            }
            catch
            {
                //Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"chown -R {Environment.UserName} {directory}\"")).WaitForExit();
                crontab = Directory.EnumerateFiles(directory, Environment.UserName, SearchOption.AllDirectories).ToList();
            }

            if (!string.IsNullOrEmpty(crontab.FirstOrDefault()))
                Crontab.Exists = true;
            else errorMessages.Add(Language.GetPhrase(34));

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
                if (errorMessages.Count == 0)
                {
                    Interface.Init();
                    Application.Run();
                }
                else
                {
                    foreach (string error in errorMessages)
                        error.Message(false, false);
                    Application.Quit();
                }
            }
        }
    }
}