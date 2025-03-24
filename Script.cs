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
            Language.GetPhrase(50).Message(Environment.NewLine, false, false);
        }

        public static void Run(int day, int days, int delay)
        {
            if (day <= days)
            {
                $"{Language.GetPhrase(54)} {delay} {Language.GetPhrase(65)}...".Message(Environment.NewLine);
                Task.Delay(delay * 1000).Wait();

                string sourceLink = Config.GetSourceLink();
                string videoPath = Path.Combine(Temp.Path, $"video{day}.mkv");
                string shortVideoPath = Path.Combine(Temp.Path, $"shortVideo{day}.mp4");
                string concatedVideoPath = Path.Combine(Temp.Path, "concatedVideo.mp4");
                string concatedTodayVideoPath = Path.Combine(Temp.Path, $"concatedVideo{day}.mp4");

                List<string> videos;

                int allTime = 16 * 60 * 60;
                int videoTime = (int)((allTime / days) + Math.Round(allTime % days / (double)days));

                $"{Language.GetPhrase(66)} {videoTime} {Language.GetPhrase(65)}".Message(Environment.NewLine);
                while (!File.Exists(videoPath))
                {
                    Language.GetPhrase(51).Message();
                    FFmpeg.Record(sourceLink, videoPath, videoTime);
                    Language.GetPhrase(52).Message();
                }
                    
                int realVideoTime = (int)FFprobe.GetInfo.Duration(videoPath);
                if (realVideoTime < videoTime)
                {
                    $"{Language.GetPhrase(53)} ({realVideoTime} {Language.GetPhrase(65)})".Message(Environment.NewLine);

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

                        $"{Language.GetPhrase(55)} {attempt}".Message();
                        FFmpeg.Record(sourceLink, temporaryVideoPath, videoTime - realVideoTime);
                        Language.GetPhrase(56).Message();

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
                            Language.GetPhrase(57).Message();
                            FFmpeg.Concat(videos, videoPath);
                            Language.GetPhrase(58).Message();
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
                
                $"{Language.GetPhrase(59)} {videoTime} {Language.GetPhrase(65)}".Message(Environment.NewLine);
                
                double realShortVideoTime = FFprobe.GetInfo.Duration(shortVideoPath);
                int acceleration = 96;
                while (!(realShortVideoTime > 0 && (realShortVideoTime * 1.05 >= videoTime / acceleration || realShortVideoTime * 1.05 <= videoTime / acceleration)))
                {
                    Language.GetPhrase(60).Message();
                    FFmpeg.Accelerate(videoPath, shortVideoPath, acceleration);
                    Language.GetPhrase(61).Message();
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

                    Language.GetPhrase(62).Message(Environment.NewLine);
                    
                    double realConcatedVideoTime = FFprobe.GetInfo.Duration(concatedVideoPath);
                    double realConcatedTodayVideoTime = FFprobe.GetInfo.Duration(concatedTodayVideoPath);
                    while (realConcatedTodayVideoTime <= realConcatedVideoTime)
                    {
                        Language.GetPhrase(63).Message();
                        FFmpeg.Concat(videos, concatedTodayVideoPath);
                        Language.GetPhrase(58).Message();
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
                    Language.GetPhrase(64).Message(Environment.NewLine);
                }
            }
            else
            {
                Delete();
            }
        }
    }
}