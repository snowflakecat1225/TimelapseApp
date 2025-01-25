using System;
using System.IO;

namespace TimelapseApp
{
    public class Temp
    {
        private static readonly string tempPath = $"/home/{Environment.UserName}/.cache/TimelapseApp";

        public static void Create()
        {
            try
            {
                if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
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
                Directory.Delete(tempPath, true);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        } 
    }
}
