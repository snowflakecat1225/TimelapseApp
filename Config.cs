using System;
using System.IO;

namespace TimelapseApp
{
    public class Config
    {
        public static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        public static void Create(string sourceLink, string resultPath, bool localTimeChecked)
        {
            try
            {
                using StreamWriter sw = new(System.IO.Path.Combine(Path, "TimelapseApp.conf"));
                sw.WriteLine(sourceLink + "\n" + resultPath + "\n" + localTimeChecked + "\n" + FFmpeg.Path + "\n" + FFplay.Path);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string GetConfigString(int index)
        {
            if (File.Exists(Path + "/TimelapseApp.conf"))
            {
                try
                {
                    using StreamReader sr = new(System.IO.Path.Combine(Path, "TimelapseApp.conf"));
                        return sr.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries)[index];
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return string.Empty;
        }

        public static string GetSourceLink() => GetConfigString(0);
        public static string GetResultPath() => GetConfigString(1);
        public static bool GetLocalTimeChecked()
        {
            string localTimeChecked = string.Empty;
            
            try
            {
                localTimeChecked = GetConfigString(2);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return localTimeChecked.ToLower() switch
            {
                "false" or "0" => false,
                "true" or "1" => true,
                _ => false,
            };
        }
        public static string GetFFmpegPath() => GetConfigString(3);
        public static string GetFFplayPath() => GetConfigString(4);


        public static void Delete()
        {
            try
            {
                File.Delete(System.IO.Path.Combine(Path, "TimelapseApp.conf"));
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
    }
}
