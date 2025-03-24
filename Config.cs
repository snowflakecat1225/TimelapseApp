using System;
using System.Diagnostics;
using System.IO;

namespace TimelapseApp
{
    public class Config
    {
        public static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string _configPath = System.IO.Path.Combine(Path, $"{Process.GetCurrentProcess().ProcessName}.conf");
        public static readonly bool Exists = File.Exists(_configPath);

        public static void Create(string sourceLink, string resultPath, bool timestampChecked, string tempPath = null)
        {
            if (tempPath == System.IO.Path.Combine(System.IO.Path.GetTempPath(), Process.GetCurrentProcess().ProcessName)) tempPath = null;

            try
            {
                using StreamWriter sw = new(_configPath);
                sw.Write(
                    sourceLink + "\n" +
                    resultPath + "\n" +
                    timestampChecked + "\n" +
                    tempPath
                );
            }
            catch (Exception ex)
            {
                ("[Config.Create()]: " + ex.Message).Message();
            }
        }

        private static string GetConfigString(int index)
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    using StreamReader sr = new(_configPath);
                    string[] configs = sr.ReadToEnd().Split("\n");
                    return configs.Length == 4 ? configs[index] : string.Empty;
                }
                catch (Exception ex)
                {
                    string method = index switch
                    {
                        0 => "GetSourceLink()",
                        1 => "GetResultPath()",
                        2 => "GetTimestampChecked()",
                        3 => "GetTempPath()",
                        _ => string.Empty,
                    };
                    ($"[Config.{method}]: " + ex.Message).Message();
                }
            }
            return string.Empty;
        }

        public static string GetSourceLink() => GetConfigString(0);
        public static string GetResultPath() => GetConfigString(1);
        public static bool GetTimestampChecked()
        {
            return GetConfigString(2).ToLower() switch
            {
                "true" or "1" => true,
                _ => false,
            };
        }
        public static string GetTempPath() => GetConfigString(3);

        public static void Delete()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    File.Delete(_configPath);
                }
                catch (Exception ex)
                {
                    ("[Config.Delete()]: " + ex.Message).Message();
                }
            }
        }
    }
}
