using System;
using System.Diagnostics;
using System.IO;

namespace TimelapseApp
{
    public static class TimelapseScript
    {
        public static void Create(string sourceLink, string resultPath, bool timestampChecked)
        {
            Config.Create(sourceLink, resultPath, timestampChecked);

            Temp.Create();

            Crontab.Add($"0 7 * * * {Environment.ProcessPath}");
        }

        public static void Delete()
        {
            Config.Delete();
            
            Temp.Delete();

            Crontab.Remove("0 7 * * *");
        }

        public static void Run(int day, int days)
        {
            if (day <= days)
            {
                int allTime = 16 * 60 * 60;
                int videoTime = (int)(allTime / days + Math.Round(allTime % days / (double)days));
                int minutes = videoTime / 60;
                int v = videoTime - minutes * 60;

                File.Create("1225.text");

                //System.Console.WriteLine((allTime / days) + " " + (allTime % days) + " " + ((allTime - allTime / days * days) / Convert.ToDouble(days)) + " " + (allTime % days / Convert.ToDouble(days)) + " " + videoTime + " " + (videoTime * days));
            }
            else
            {
                Delete();
            }
        }
    }
}
