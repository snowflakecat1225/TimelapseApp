using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                crons = crons.Concat(process.StandardOutput.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries)).ToList();
            }

            return crons;
        }

        public static string Get(string cron)
        {
            if (!string.IsNullOrEmpty(cron))
            {
                var crons = GetAll();

                foreach (var cronie in crons)
                {
                    if (cronie.Contains(cron))
                        return cronie;
                }
            }

            return string.Empty;
        }

        public static void Add(string cron)
        {
            if (string.IsNullOrEmpty(Get(Environment.ProcessPath)))
            {
                try
                {
                    string crons = string.Concat(GetAll()) + '\n' + cron;
                    Process.Start(new ProcessStartInfo("bash", $"-c \"echo '{crons}' | crontab -\"")).WaitForExit();
                }
                catch (UnauthorizedAccessException)
                {
                    Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"chown -R {Environment.UserName} /var/spool/cron\"")).WaitForExit();
                    Add(cron);
                }
                catch (Exception ex)
                {
                    ("[Crontab.Add()]: " + ex.Message).Message(1);
                }
            }
            else Change(Get(Environment.ProcessPath), cron);
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

                try
                {
                    Process.Start(new ProcessStartInfo("bash", $"-c \"echo '{allCrons}' | crontab -\"")).WaitForExit();
                }
                catch (UnauthorizedAccessException)
                {
                    Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"chown -R {Environment.UserName} /var/spool/cron\"")).WaitForExit();
                    Remove(cron);
                }
                catch (Exception ex)
                {
                    ("[Crontab.Remove()]: " + ex.Message).Message(1);
                }
            }
        }

        public static void Change(string oldCron, string newCron)
        {
            var crons = GetAll();
            oldCron = Get(oldCron);

            if (!string.IsNullOrEmpty(oldCron))
            {
                string allCrons = string.Empty;

                foreach (var cron in crons)
                    allCrons += cron != oldCron ? cron + '\n' : newCron + '\n';

                try
                {
                    Process.Start(new ProcessStartInfo("bash", $"-c \"echo '{allCrons}' | crontab -\"")).WaitForExit();
                }
                catch (UnauthorizedAccessException)
                {
                    Process.Start(new ProcessStartInfo("pkexec", $"bash -c \"chown -R {Environment.UserName} /var/spool/cron\"")).WaitForExit();
                    Change(oldCron, newCron);
                }
                catch (Exception ex)
                {
                    ("[Crontab.Change()]: " + ex.Message).Message(1);
                }
            }
            else Add(newCron);
        }
    }
}