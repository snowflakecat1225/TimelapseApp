using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gtk;

namespace TimelapseApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Application.Init();

            string directory = "/", ffmpegSearchResult = string.Empty, ffplaySearchResult = string.Empty;

            Parallel.Invoke(
                () =>
                {
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(directory, "ffmpeg", SearchOption.AllDirectories))
                            ffmpegSearchResult = file;
                    }
                    catch { /*To be honest I don't care :P*/ }
                },

                () =>
                {
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(directory, "ffplay", SearchOption.AllDirectories))
                            ffplaySearchResult = file;
                    }
                    catch { /*To be honest I don't care :P*/ }
                }
            );

            if (!string.IsNullOrEmpty(ffmpegSearchResult))
                FFmpeg.Path = ffmpegSearchResult;
            else
            {
                MessageDialog messageDialogFfmpeg = new(
                    GtkInterface.MainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "There is not installed FFmpeg");
                messageDialogFfmpeg.Run();
                messageDialogFfmpeg.Destroy();
            }

            if (!string.IsNullOrEmpty(ffplaySearchResult))
                FFplay.Path = ffplaySearchResult;
            else
            {
                MessageDialog messageDialogFfplay = new(
                    GtkInterface.MainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "There is not installed FFplay");
                messageDialogFfplay.Run();
                messageDialogFfplay.Destroy();
            }

            Temp.Path = Path.Combine(Path.GetTempPath(), "TimelapseApp");

            if (args.Length == 3 && args.ContainsOnlyNumbers())
                TimelapseScript.Run(
                    int.Parse(args[0]),
                    int.Parse(args[1]),
                    int.Parse(args[2]));
            else GtkInterface.Init();

            Application.Run();
        }
    }
}
