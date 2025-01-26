using System;
using System.Diagnostics;
using System.Net.Sockets;
using Gtk;

namespace TimelapseApp
{
    class MainWindow
    {
        private static Window _mainWindow;
        private static Window _settingsWindow;
        private static Entry _rtspEntry;
        private static Entry _saveEntry;

        [Obsolete]
        public static void Init()
        {
            Application.Init();

            MainWindowInit();
            SettingsWindowInit();

            Application.Run();
        }

        [Obsolete]
        private static void MainWindowInit()
        {
            _mainWindow = new("Timelapse App");
            _mainWindow.Destroyed += delegate { Application.Quit(); };
            
            Box box = new(Orientation.Vertical, 15)
            {
                Margin = 45,
                MarginTop = 20,
                MarginBottom = 15,
                WidthRequest = 320
            };
            _mainWindow.Add(box);
            
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
            box.Add(rtspEntryBox);

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
            box.Add(saveEntryBox);
            
            HBox commonBox = new(true, 0);
            HBox box1 = new();
            HBox box2 = new();
            HBox box3 = new();
            Button startButton = new() 
            {
                Label = "Start recording",
                Halign = Align.Center
            };
            startButton.Clicked += StartButton_Clicked;
            box2.Add(startButton);
            Button openSettingsButton = new()
            {
                Halign = Align.End
            };
            box3.Add(openSettingsButton);
            commonBox.Add(box1);
            commonBox.Add(box2);
            commonBox.Add(box3);

            Box startRecordingBox = new(Orientation.Vertical, 10);
            Separator separator1 = new(Orientation.Horizontal);
            Separator separator2 = new(Orientation.Horizontal);
            startRecordingBox.Add(separator1);
            startRecordingBox.Add(commonBox);
            startRecordingBox.Add(separator2);
            box.PackEnd(startRecordingBox, false, false, 10);

            _mainWindow.ShowAll();
        }

        private static void SettingsWindowInit()
        {
            _settingsWindow = new("Settings");

            Box box = new(Orientation.Vertical, 15)
            {
                Margin = 45,
                MarginTop = 20,
                MarginBottom = 15,
                WidthRequest = 320
            };
            _settingsWindow.Add(box);
        }

        private async static void FfplayButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_rtspEntry.Text))
            {
                if (_rtspEntry.Text.StartsWith("rtsp://"))
                {
                    if (IsRtspLinkValid(_rtspEntry.Text))
                    {
                        ProcessStartInfo startInfo = new()
                        {
                            FileName = "/usr/bin/ffplay",
                            Arguments = "-rtsp_transport tcp -b:v 100 -tune zerolatency -preset ultrafast -an " + _rtspEntry.Text
                        };

                        var ffplay = Process.Start(startInfo)!;
                        
                        await ffplay.WaitForExitAsync();
                    }
                }
                else
                {
                    MessageDialog md = new(
                        _mainWindow,
                        DialogFlags.Modal,
                        MessageType.Error,
                        ButtonsType.Close,
                        "This is not RTSP-link");
                    md.Run();
                    md.Destroy();
                }
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
                _mainWindow,
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

        private static bool IsRtspLinkValid(string link)
        {
            try
            {
                var uri = new Uri(link);
                var host = uri.Host;
                MessageDialog md = new(
                    _mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    uri.UserInfo);
                md.Run();
                md.Destroy();
                var port = uri.Port == -1 ? 554 : uri.Port;

                using var client = new TcpClient();
                client.Connect(host, port);

                return true;
            }
            catch (Exception ex)
            {
                MessageDialog md = new(
                    _mainWindow,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Close,
                    ex.Message);
                md.Run();
                md.Destroy();

                return false;
            }
        }

        private static void StartButton_Clicked(object sender, EventArgs e)
        {
            
        }
    }
}