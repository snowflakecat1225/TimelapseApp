using System.IO;

namespace TimelapseApp
{
    public class Temp
    {
        public static string Path { get; set; }

        public static string GetDefaultPath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TimelapseApp");
        }

        public static void Create()
        {
            try
            {
                if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            }
            catch (IOException ex)
            {
                ex.Message.Message(Interface.Main);
            }
        }

        public static void Delete()
        {
            try
            {
                Directory.Delete(Path, true);
            }
            catch (IOException ex)
            {
                ex.Message.Message(Interface.Main);
            }
        } 
    }
}
