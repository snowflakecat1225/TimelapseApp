using System;
using System.Diagnostics;
using System.IO;
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

        public static void Message(this string message, ushort iDontKnowHowToNameThisShit = 0)
        {
            if (Interface.IsInitialised)
            {
                switch (iDontKnowHowToNameThisShit)
                {
                    case 1:
                        MessageDialog mianMessageDialog = new(
                            Interface.Windows.Main,
                            DialogFlags.Modal,
                            MessageType.Error,
                            ButtonsType.Close,
                            message);
                        mianMessageDialog.Run();
                        mianMessageDialog.Destroy();
                        break;
                    case 2:
                        MessageDialog settingsMessageDialog = new(
                            Interface.Windows.Settings,
                            DialogFlags.Modal,
                            MessageType.Error,
                            ButtonsType.Close,
                            message);
                        settingsMessageDialog.Run();
                        settingsMessageDialog.Destroy();
                        break;
                }
            }
            else Console.WriteLine(message);

            using StreamWriter sw = new(Path.Combine(Config.Path, Process.GetCurrentProcess().ProcessName + "Errors.txt"), true);
                sw.WriteLine($"[{DateTime.Now}]   -   {message}");
        }
    }
}
