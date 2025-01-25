using System;
using Gtk;

namespace TimelapseApp
{
    class MainWindow
    {
        //private static Window mainWindow;
        //private static Box mainBox;

        public static void Init()
        {
            Application.Init();

            //mainWindow = new Window("Aboba");
            //mainWindow.DeleteEvent += delegate { Application.Quit(); };
            
            //mainBox = new(Orientation.Vertical, 20) { Margin = 10, WidthRequest = 640 };
            //mainWindow.Add(mainBox);

            MessageDialog md = new(
                null,
                DialogFlags.Modal,
                MessageType.Error,
                ButtonsType.Close,
                "You're adopted");
            md.Destroyed += delegate { Environment.Exit(0); };
            md.Run();
            md.Destroy();

            //mainWindow.ShowAll();

            Application.Run();
        }
    }
}