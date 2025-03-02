using Gtk;

namespace TimelapseApp
{
    public static class Extension
    {
        private static bool IsNumber(this string s)
        {
            foreach (var c in s)
                if (!char.IsDigit(c))
                    return false;
            return true;
        }

        public static bool ContainsOnlyNumbers(this string[] array)
        {
            foreach (var item in array)
                if (!item.IsNumber())
                    return false;
            return true;
        }

        public static void Message(this string message, Window window)
        {
            MessageDialog messageDialog = new(
                window,
                DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Close,
                message);
            messageDialog.Run();
            messageDialog.Destroy();
        }
    }
}
