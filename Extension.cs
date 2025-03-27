using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Gtk;

namespace TimelapseApp
{
    public static class Extension
    {
        public static bool IsNumber(this string s)
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

        private static void MessageProcessing(string message, bool useSettingsWindow)
        {
            if (Interface.IsInitialised)
            {
                Window window = useSettingsWindow ? Interface.Windows.Settings : Interface.Windows.Main;
                
                MessageDialog messageDialog = new(
                    window,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    message)
                {
                    Title = Language.GetPhrase(35)
                };
                messageDialog.Run();
                messageDialog.Destroy();
            }
            else Console.WriteLine(message);

            using StreamWriter sw = new(Path.Combine(Config.Path, Process.GetCurrentProcess().ProcessName + "_log.txt"), true);
                sw.WriteLine(message);
        }
        public static void Message(this string message, bool useSettingsWindow = false, bool addTimestamp = true)
        {
            if (addTimestamp)
                MessageProcessing($"[{DateTime.Now}]   -   {message}", useSettingsWindow);
            else
                MessageProcessing(message, useSettingsWindow);
        }
        public static void Message(this string message, string beforeText, bool useSettingsWindow = false, bool addTimestamp = true)
        {
            if (addTimestamp)
                MessageProcessing($"{beforeText}[{DateTime.Now}]   -   {message}", useSettingsWindow);
            else
                MessageProcessing($"{beforeText}{message}", useSettingsWindow);
        }

        public static bool IsRtspLinkValid(this string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                Language.GetPhrase(36).Message(false, false);
                return false;
            }

            if (!link.StartsWith("rtsp://"))
            {
                Language.GetPhrase(37).Message(false, false);
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

        public static int ExtractNumber(this string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                $"[videos.Sort(ExtractNumber())]: {Language.GetPhrase(38)}".Message();

            var match = Regex.Match(fileName, @"\((\d+)\)");

            if (match.Success)
                return int.Parse(match.Groups[1].Value);
            else $"[videos.Sort(ExtractNumber())]: {Language.GetPhrase(39)}".Message();

            return 0;
        }
    }
}
