using System;
using System.IO;

namespace TimelapseApp
{
    public class Temp
    {
        private static string _tempPath;
        public static string Path
        {
            get { return _tempPath; }
            set { _tempPath = value; }
        }

        public static void Create()
        {
            try
            {
                if (!Directory.Exists(_tempPath))
                Directory.CreateDirectory(_tempPath);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Delete()
        {
            try
            {
                Directory.Delete(_tempPath, true);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
    }
}
