using System;
using System.Diagnostics;
using System.IO;

namespace TimelapseApp
{
    public class Temp
    {
        public static string Path
        {
            get
            {
                string tempPath = Config.GetTempPath();
                return !string.IsNullOrEmpty(tempPath) ? tempPath : System.IO.Path.Combine(System.IO.Path.GetTempPath(), Process.GetCurrentProcess().ProcessName);
            }
        }

        public static void Create()
        {
            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
            }
            catch (Exception ex)
            {
                ("[Temp.Create()]: " + ex.Message).Message(1);
            }
        }

        public static void Delete()
        {
            try
            {
                Directory.Delete(Path, true);
            }
            catch (Exception ex)
            {
                ("[Temp.Delete()]: " + ex.Message).Message(1);
            }
        }
    }
}
