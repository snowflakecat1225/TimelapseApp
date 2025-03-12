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
            "I have finished all my work. I hope we will meet again :3".Message();
        }

        public static void Run(int day, int days, int delay)
        {
            if (day <= days)
            {
                $"I am waiting for {delay} seconds...".Message();
                Task.Delay(delay * 1000).Wait();

                string sourceLink = Config.GetSourceLink();
                string videoPath = Path.Combine(Temp.Path, $"video{day}.mkv");
                string shortVideoPath = Path.Combine(Temp.Path, $"shortVideo{day}.mp4");
                string concatedVideoPath = Path.Combine(Temp.Path, "concatedVideo.mp4");
                string concatedTodayVideoPath = Path.Combine(Temp.Path, $"concatedVideo{day}.mp4");

                int allTime = 16 * 60 * 60;
                int videoTime = (int)((allTime / days) + Math.Round(allTime % days / (double)days));

                "Recording is started".Message();
                FFmpeg.Record(sourceLink, videoPath, videoTime);
                "Recording have finished".Message();
                FFmpeg.Repair(videoPath);

                if (File.Exists(videoPath))
                {
                    int realVideoTime = FFprobe.GetInfo.Duration(videoPath);
                    if (realVideoTime < videoTime)
                    {
                        "This video is shorter than required".Message();
                        float i = 1;
                        while (realVideoTime < videoTime)
                        {
                            string temporaryVideoPath = Path.Combine(Temp.Path, $"temporaryVideo{day}({i}).mkv");

                            "Recording continues".Message();
                            FFmpeg.Record(sourceLink, temporaryVideoPath, videoTime - realVideoTime);
                            "Recording have finished again".Message();
                            FFmpeg.Repair(temporaryVideoPath);

                            realVideoTime += FFprobe.GetInfo.Duration(temporaryVideoPath);
                            i++;
                        }

                        List<string> temporaryVideos = Directory.GetFiles(Temp.Path, "*temporaryVideo*", SearchOption.TopDirectoryOnly).ToList();
                        temporaryVideos.Sort();
                        List<string> videos = temporaryVideos;

                        if (File.Exists(videoPath))
                            videos.Insert(0, videoPath);

                        string temporaryConcatedVideoPath = Path.Combine(Temp.Path, $"temporaryConcatedVideo{day}.mp4");

                        "Today's and temporary videos are concating".Message();
                        FFmpeg.Concat(videos, temporaryConcatedVideoPath);
                        "Concating have done".Message();

                        if (File.Exists(temporaryConcatedVideoPath))
                        {
                            foreach (string temporaryVideo in temporaryVideos)
                            {
                                try
                                {
                                    File.Delete(temporaryVideo);
                                }
                                catch (Exception ex)
                                {
                                    ("[Script.File.Delete()]: " + ex.Message).Message();
                                }
                            }

                            try
                            {
                                File.Move(temporaryConcatedVideoPath, videoPath, true);
                            }
                            catch (Exception ex)
                            {
                                ("[Script.File.Move()]: " + ex.Message).Message();
                            }
                        }
                    }

                    $"Yes, video file exists and it's duration is {videoTime}. I am starting to accelerate it".Message();
                    FFmpeg.Accelerate(videoPath, shortVideoPath, 96 + new Random().Next(2));
                    "Accelerating have done".Message();

                    if (File.Exists(shortVideoPath))
                    {
                        try
                        {
                            File.Delete(videoPath);
                        }
                        catch (Exception ex)
                        {
                            ("[Script.File.Delete()]: " + ex.Message).Message();
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
                        else
                        {
                            List<string> shortVideos = Directory.GetFiles(Temp.Path, "*shortVideo*", SearchOption.TopDirectoryOnly).ToList();
                            shortVideos.Sort();
                            List<string> videos = shortVideos;

                            if (File.Exists(concatedVideoPath))
                                videos.Insert(0, concatedVideoPath);

                            "Yes, short video files exist, and I am starting to concat them".Message();
                            FFmpeg.Concat(videos, concatedTodayVideoPath);
                            "Concating have done".Message();

                            if (File.Exists(concatedTodayVideoPath))
                            {
                                foreach (string shortVideo in shortVideos)
                                {
                                    try
                                    {
                                        File.Delete(shortVideo);
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
                        }

                        if (File.Exists(concatedVideoPath))
                        {
                            DateTime cronTime = new(1, 1, 1, 7, 0, 0);
                            cronTime = cronTime.AddSeconds(videoTime * day);
                            string cron = $"{cronTime.Minute} {cronTime.Hour} * * * {Environment.ProcessPath} {day + 1} {days} {cronTime.Second}";
                            Crontab.Change(Environment.ProcessPath, cron);
                            "Cron record have changed".Message();
                        }
                    }
                }
            }
            else
            {
                Delete();
            }
        }
    }
}