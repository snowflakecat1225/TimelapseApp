using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Gtk;

namespace TimelapseApp
{
    public static class Extension
    {
        private static bool IsNumber(this string s)
        {
            foreach (char c in s)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        public static bool ContainsOnlyNumbers(this string[] array)
        {
            foreach (string item in array)
                if (!item.IsNumber())
                    return false;
            return true;
        }

        public static void Message(this string message, bool useSettingsWindow = false)
        {
            if (Interface.IsInitialised)
            {
                Window window = useSettingsWindow ? Interface.Windows.Settings : Interface.Windows.Main;
                
                MessageDialog mianMessageDialog = new(
                    window,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    message);
                mianMessageDialog.Run();
                mianMessageDialog.Destroy();
            }
            else Console.WriteLine(message);

            using StreamWriter sw = new(Path.Combine(Config.Path, Process.GetCurrentProcess().ProcessName + "Errors.txt"), true);
                sw.WriteLine($"[{DateTime.Now}]   -   {message}");
        }

        public static bool IsRtspLinkValid(this string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                "There is no text to check".Message();
                return false;
            }

            if (!link.StartsWith("rtsp://"))
            {
                "This is not RTSP-link".Message();
                return false;
            }

            try
            {
                Uri uri = new(link);
                string host = uri.Host;
                int port = uri.Port == -1 ? 554 : uri.Port;

                using TcpClient client = new();
                client.Connect(host, port);

                return true;
            }
            catch (Exception ex)
            {
                ("[string.IsRtspLinkValid()]: " + ex.Message).Message();
                return false;
            }
        }
    }
}
