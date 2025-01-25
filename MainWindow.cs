using System;
using System.Diagnostics;
using Gtk;

namespace TimelapseApp
{
    class MainWindow
    {
        private static Window _mainWindow;
        private static Entry _rtspEntry;
        private static Entry _saveEntry;
        public static void Init()
        {
            Application.Init();

            _mainWindow = new("Timelapse App");
            _mainWindow.Destroyed += delegate { Application.Quit(); };
            
            Box mainBox = new(Orientation.Vertical, 15)
            {
                Margin = 45,
                MarginTop = 20,
                MarginBottom = 15,
                WidthRequest = 320
            };
            _mainWindow.Add(mainBox);
            
            Box rtspEntryBox = new(Orientation.Vertical, 10);
            Label rtspLabel = new()
            {
                Text = "Link to stream",
                Halign = Align.Center
            };
            _rtspEntry = new()
            {
                Halign = Align.Fill
            };
            Button ffplayButton = new()
            {
                Label = "Check stream",
                Halign = Align.End
            };
            ffplayButton.Clicked += FfplayButton_Clicked;
            rtspEntryBox.Add(rtspLabel);
            rtspEntryBox.Add(_rtspEntry);
            rtspEntryBox.Add(ffplayButton);
            mainBox.Add(rtspEntryBox);

            Box saveEntryBox = new(Orientation.Vertical, 10);
            Label saveLabel = new()
            {
                Text = "Path to save",
                Halign = Align.Center
            };
            _saveEntry = new()
            {
                Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Halign = Align.Fill
            };
            Button openFolderViewButton = new()
            {
                Label = "Open",
                Halign = Align.End
            };
            openFolderViewButton.Clicked += OpenFolderViewButton_Clicked;
            saveEntryBox.Add(saveLabel);
            saveEntryBox.Add(_saveEntry);
            saveEntryBox.Add(openFolderViewButton);
            mainBox.Add(saveEntryBox);
            
            Box startRecordingBox = new(Orientation.Vertical, 10);
            Separator separator1 = new(Orientation.Horizontal);
            Button startButton = new() 
            {
                Label = "Start recording",
                Halign = Align.Center
            };
            startButton.Clicked += StartButton_Clicked;
            Separator separator2 = new(Orientation.Horizontal);
            startRecordingBox.Add(separator1);
            startRecordingBox.Add(startButton);
            startRecordingBox.Add(separator2);
            mainBox.PackEnd(startRecordingBox, false, false, 10);
            
            _mainWindow.ShowAll();

            Application.Run();
        }

        private async static void FfplayButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_rtspEntry.Text))
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = "/usr/bin/ffplay",
                    Arguments = "-rtsp_transport tcp -b:v 100 -tune zerolatency -preset ultrafast -an " + _rtspEntry.Text
                };

                var ffplay = Process.Start(startInfo)!;
                
                await ffplay.WaitForExitAsync();
            }
            else
            {
                MessageDialog md = new(
                    _mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    "Empty input field");
                md.Run();
                md.Destroy();
            }
        }

        private static void OpenFolderViewButton_Clicked(object sender, EventArgs e)
        {
            FileChooserDialog fileChooser = new(
                "Select the path to save the file",
                null,
                FileChooserAction.SelectFolder,
                "Cancel", ResponseType.Cancel,
                "Open", ResponseType.Accept)
            {
                SelectMultiple = false
            };
            fileChooser.SetCurrentFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

            fileChooser.SetPosition(WindowPosition.CenterOnParent);

            if (fileChooser.Run() == (int)ResponseType.Accept)
                _saveEntry.Text = fileChooser.CurrentFolder;

            fileChooser.Destroy();
        }

        private static void StartButton_Clicked(object sender, EventArgs e)
        {
            
        }
    }
}