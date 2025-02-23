using System;
using System.IO;

namespace TimelapseApp
{
    public class Config
    {
        public static string Path => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static void Create(string sourceLink, string resultPath, bool localTimeChecked)
        {
            try
            {
                using StreamWriter sw = new(System.IO.Path.Combine(Path, "TimelapseApp.conf"));
                sw.WriteLine(sourceLink + "\n" + resultPath + "\n" + localTimeChecked + "\n" + FFmpeg.Path + "\n" + FFplay.Path);
            }
            catch (IOException ex)
            {
                ex.Message.MwErrorMessage();
            }
        }

        private static string GetConfigString(int index)
        {
            if (File.Exists(System.IO.Path.Combine(Path, "TimelapseApp.conf")))
            {
                try
                {
                    using StreamReader sr = new(System.IO.Path.Combine(Path, "TimelapseApp.conf"));
                        return sr.ReadToEnd().Split("\n", StringSplitOptions.RemoveEmptyEntries)[index];
                }
                catch (IOException ex)
                {
                    ex.Message.MwErrorMessage();
                }
            }
            return string.Empty;
        }

        public static string GetSourceLink() => GetConfigString(0);
        public static string GetResultPath() => GetConfigString(1);
        public static bool GetTimestampChecked()
        {
            string timestampChecked = string.Empty;
            
            try
            {
                timestampChecked = GetConfigString(2);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return timestampChecked.ToLower() switch
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
