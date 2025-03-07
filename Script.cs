using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TimelapseApp
{
    public static class Script
    {
        public static void Create(string sourceLink, string resultPath, bool timestampChecked, int days, string tempPath)
        {
            Config.Create(sourceLink, resultPath, timestampChecked, tempPath);
            Temp.Create();
            Crontab.Add($"0 7 * * * {Environment.ProcessPath} 1 {days} 0");
        }

        public static void Delete()
        {
            try
            {
                File.Move(Path.Combine(Temp.Path, "concatedVideo.mp4"), Config.GetResultPath());
            }
            catch (Exception ex)
            {
                ("[Script.File.Move()]: " + ex.Message).Message();
            }
            Config.Delete();
            Temp.Delete();
            Crontab.Remove(Environment.ProcessPath);
            "I have finished my work. I hope we will meet again :3".Message();
        }

        public static void Run(int day, int days, int delay)
        {
            if (day <= days)
            {
                $"I am waiting for {delay} seconds...".Message();
                Task.Delay(delay * 1000).Wait();
                "Everything is alright!".Message();

                string sourceLink = Config.GetSourceLink();
                string videoPath = Path.Combine(Temp.Path, $"video{day}.mp4");
                string shortVideoPath = Path.Combine(Temp.Path, $"shortVideo{day}.mp4");
                string concatedVideoPath = Path.Combine(Temp.Path, "concatedVideo.mp4");
                string concatedTodayVideoPath = Path.Combine(Temp.Path, $"concatedVideo{day}.mp4");

                int allTime = 16 * 60 * 60;
                int videoTime = (int)((allTime / days) + Math.Round(allTime % days / (double)days));

                "Recording is started".Message();
                FFmpeg.Record(sourceLink, videoPath, videoTime);
                "Recording have finished".Message();
                Task.Delay(2000).Wait();

                if (File.Exists(videoPath))
                {
                    "Yes, video file exists, and I am starting to accelerate it".Message();
                    FFmpeg.Accelerate(videoPath, shortVideoPath, 96 + new Random().Next(2));
                    "Accelerating have done".Message();
                    Task.Delay(2000).Wait();

                    try
                    {
                        File.Delete(videoPath);
                    }
                    catch (Exception ex)
                    {
                        ("[Script.File.Delete()]: " + ex.Message).Message();
                    }

                    if (File.Exists(shortVideoPath))
                    {
                        if (day > 1)
                        {
                            List<string> shortVideos = Directory.GetFiles(Temp.Path, "*shortVideo*", SearchOption.TopDirectoryOnly).ToList();
                            List<string> videos = shortVideos;

                            if (File.Exists(concatedVideoPath))
                                videos.Add(concatedVideoPath);

                            "Yes, short video files exist, and I am starting to concat them".Message();
                            FFmpeg.Concat(videos, concatedTodayVideoPath);
                            "Concating have done".Message();
                            Task.Delay(2000).Wait();

                            foreach (string shortVideo in shortVideos)
                                File.Delete(shortVideo);

                            try
                            {
                                File.Move(concatedTodayVideoPath, concatedVideoPath, true);
                            }
                            catch (Exception ex)
                            {
                                ("[Script.File.Move()]: " + ex.Message).Message();
                            }
                        }
                    }
                }

                if (File.Exists(shortVideoPath) || File.Exists(concatedVideoPath))
                {
                    DateTime cronTime = new(1, 1, 1, 7, 0, 0);
                    cronTime = cronTime.AddSeconds(videoTime * day);
                    string cron = $"{cronTime.Minute} {cronTime.Hour} * * * {Environment.ProcessPath} {day + 1} {days} {cronTime.Second}";
                    Crontab.Change(Environment.ProcessPath, cron);
                    "Cron record have changed".Message();
                }
            }
            else
            {
                Delete();
            }
        }
    }
}