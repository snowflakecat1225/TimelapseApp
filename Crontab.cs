using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TimelapseApp
{
    public class Crontab
    {
        public static string Path { get; set; }

        public static bool FileExists => !string.IsNullOrEmpty(Path);

        private static List<string> GetAll()
        {
            List<string> crons = new();

            using (var process = Process.Start(new ProcessStartInfo("bash", "-c \"crontab -l\"") 
                { RedirectStandardOutput = true }))
            {
                process.WaitForExit();
                _ = crons.Concat(process.StandardOutput.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries));
            }

            return crons;
        }

        private static string Get(string cron)
        {
            var crons = GetAll();

            foreach (var cronie in crons)
                if (cronie.Contains(cron))
                    return cronie;

            return string.Empty;
        }

        public static void Add(string cron)
        {
            if (string.IsNullOrEmpty(Get(cron)))
            {
                string command = $"echo '{cron}' >> {Path}";

                try
                {
                    Process.Start(new ProcessStartInfo("bash", $"-c \"{command}\"")).WaitForExit();
                }
                catch
                {
                    Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"{command}\"")).WaitForExit();
                }
            }
            else "Crontab note already exists".Message(Interface.Main);
        }

        public static void Remove(string cron)
        {
            var crons = GetAll();
            cron = Get(cron);

            if (!string.IsNullOrEmpty(cron))
            {
                string allCrons = string.Empty;

                foreach (var cronie in crons)
                    if (cronie != cron)
                        allCrons += cronie + '\n';
                
                var command = $"echo '{allCrons}' > {Path}";

                try
                {
                    Process.Start(new ProcessStartInfo("bash", $"-c \"{command}\"")).WaitForExit();
                }
                catch
                {
                    Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"{command}\"")).WaitForExit();
                }
            }
        }
    }
}