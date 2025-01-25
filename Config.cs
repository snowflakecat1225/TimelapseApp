using System;
using System.IO;

namespace TimelapseApp
{
    public class Config
    {
        private static readonly string _configPath = $"/home/{Environment.UserName}/.config/TimelapseApp.conf";
        
        public static void Create(string sourceLink, string resultPath, bool localTimeChecked)
        {
            try
            {
                if (File.Exists(_configPath))
                    File.Delete(_configPath);
                using StreamWriter sw = new(_configPath);
                    sw.WriteLine(sourceLink + "\n" + resultPath + "\n" + localTimeChecked);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string GetConfigString(int index)
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    using StreamReader sr = new(_configPath);
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

        public static void Delete()
        {
            try
            {
                File.Delete(_configPath);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
    }
}
