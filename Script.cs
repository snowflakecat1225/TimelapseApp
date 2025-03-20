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
            if (File.Exists(Path.Combine(Temp.Path, "concatedVideo.mp4")))
            {
                try
                {
                    File.Move(Path.Combine(Temp.Path, "concatedVideo.mp4"), Path.Combine(Config.GetResultPath(), $"Timelapse({DateTime.Now}).mp4"));
                }
                catch (Exception ex)
                {
                    ("[Script.File.Move()]: " + ex.Message).Message();
                }
            }
            Config.Delete();
            Temp.Delete();
            Crontab.Remove(Environment.ProcessPath);
            "I have finished all my work. I hope we will meet again :3".Message('\n', false, false);
        }

        public static void Run(int day, int days, int delay)
        {
            if (day <= days)
            {
                $"I am waiting for {delay} seconds...".Message('\n');
                Task.Delay(delay * 1000).Wait();

                string sourceLink = Config.GetSourceLink();
                string videoPath = Path.Combine(Temp.Path, $"video{day}.mkv");
                string shortVideoPath = Path.Combine(Temp.Path, $"shortVideo{day}.mp4");
                string concatedVideoPath = Path.Combine(Temp.Path, "concatedVideo.mp4");
                string concatedTodayVideoPath = Path.Combine(Temp.Path, $"concatedVideo{day}.mp4");

                List<string> videos;

                int allTime = 16 * 60 * 60;
                int videoTime = (int)((allTime / days) + Math.Round(allTime % days / (double)days));

                "It is time to start!".Message('\n');
                while (!File.Exists(videoPath))
                {
                    "Recording is started".Message();
                    FFmpeg.Record(sourceLink, videoPath, videoTime);
                    "Recording have finished".Message();
                }
                    
                int realVideoTime = (int)FFprobe.GetInfo.Duration(videoPath);
                if (realVideoTime < videoTime)
                {
                    "This video is shorter than required".Message('\n');

                    try
                    {
                        File.Move(videoPath, Path.Combine(Temp.Path, $"temporaryVideo{day}(1).mkv"), true);
                    }
                    catch (Exception ex)
                    {
                        ("[Script.File.Move()]: " + ex.Message).Message();
                    }

                    float attempt = 2;
                    while (realVideoTime < videoTime)
                    {
                        string temporaryVideoPath = Path.Combine(Temp.Path, $"temporaryVideo{day}({attempt}).mkv");

                        "Recording continues".Message();
                        FFmpeg.Record(sourceLink, temporaryVideoPath, videoTime - realVideoTime);
                        "Recording have finished again".Message();

                        if (File.Exists(temporaryVideoPath))
                        {
                            realVideoTime += (int)FFprobe.GetInfo.Duration(temporaryVideoPath);
                            attempt++;
                        }
                    }

                    videos = Directory.GetFiles(Temp.Path, $"*temporaryVideo{day}*", SearchOption.TopDirectoryOnly).ToList();
                    if (videos.Count > 0)
                    {
                        videos.Sort((a, b) => 
                        {
                            int numA = a.ExtractNumber(true);
                            int numB = b.ExtractNumber(true);
                            return numA.CompareTo(numB);
                        });

                        while (!File.Exists(videoPath))
                        {
                            "Today's videos are concating".Message();
                            FFmpeg.Concat(videos, videoPath);
                            "Concating have done".Message();
                        }

                        if ((int)FFprobe.GetInfo.Duration(videoPath) >= videoTime)
                        {
                            videos = Directory.GetFiles(Temp.Path, "*temporaryVideo*", SearchOption.TopDirectoryOnly).ToList();
                            foreach (string video in videos)
                            {
                                try
                                {
                                    File.Delete(video);
                                }
                                catch (Exception ex)
                                {
                                    ("[Script.File.Delete()]: " + ex.Message).Message();
                                }
                            }
                        }
                    }
                }
                
                $"Yes, video file exists and it's duration is {videoTime}".Message('\n');
                
                double realShortVideoTime = FFprobe.GetInfo.Duration(shortVideoPath);
                int acceleration = 96;
                while (!(realShortVideoTime > 0 && (realShortVideoTime * 1.05 >= videoTime / acceleration || realShortVideoTime * 1.05 <= videoTime / acceleration)))
                {
                    "Accelerating is started".Message();
                    FFmpeg.Accelerate(videoPath, shortVideoPath, acceleration);
                    "Accelerating have done".Message();
                    realShortVideoTime = FFprobe.GetInfo.Duration(shortVideoPath);
                    acceleration++;
                }

                if (File.Exists(videoPath))
                {
                    try
                    {
                        File.Delete(videoPath);
                    }
                    catch (Exception ex)
                    {
                        ("[Script.File.Delete()]: " + ex.Message).Message();
                    }
                }

                if (day == 1)
                {
                    try
                    {
                        File.Move(shortVideoPath, concatedVideoPath, true);
                    }
                    catch (Exception ex)
                    {
                        ("[Script.File.Move()]: " + ex.Message).Message();
                    }
                }
                
                if (File.Exists(shortVideoPath) && File.Exists(concatedVideoPath))
                {
                    videos = new() { concatedVideoPath, shortVideoPath };

                    "Yes, short video files exist, and I am starting to concat them".Message('\n');
                    
                    double realConcatedVideoTime = FFprobe.GetInfo.Duration(concatedVideoPath);
                    double realConcatedTodayVideoTime = FFprobe.GetInfo.Duration(concatedTodayVideoPath);
                    while (realConcatedTodayVideoTime <= realConcatedVideoTime)
                    {
                        "Concating is started".Message();
                        FFmpeg.Concat(videos, concatedTodayVideoPath);
                        "Concating have done".Message();
                        realConcatedTodayVideoTime = FFprobe.GetInfo.Duration(concatedTodayVideoPath);
                    }

                    videos = Directory.GetFiles(Temp.Path, "*shortVideo*", SearchOption.TopDirectoryOnly).ToList();
                    foreach (string video in videos)
                    {
                        try
                        {
                            File.Delete(video);
                        }
                        catch (Exception ex)
                        {
                            ("[Script.File.Delete()]: " + ex.Message).Message();
                        }
                    }

                    try
                    {
                        File.Move(concatedTodayVideoPath, concatedVideoPath, true);
                    }
                    catch (Exception ex)
                    {
                        ("[Script.File.Move()]: " + ex.Message).Message();
                    }
                }

                if (File.Exists(concatedVideoPath))
                {
                    DateTime cronTime = new(1, 1, 1, 7, 0, 0);
                    cronTime = cronTime.AddSeconds(videoTime * day);
                    string cron = $"{cronTime.Minute} {cronTime.Hour} * * * {Environment.ProcessPath} {day + 1} {days} {cronTime.Second}";
                    Crontab.Change(Environment.ProcessPath, cron);
                    "Cron record have changed".Message('\n');
                }
            }
            else
            {
                Delete();
            }
        }
    }
}