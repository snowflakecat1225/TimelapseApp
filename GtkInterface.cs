using System;
using System.Net.Sockets;
using Gtk;

namespace TimelapseApp
{
    class GtkInterface
    {
        public static Window MainWindow { get; } = new("Timelapse App");
        private static Window _settingsWindow;
        private static Entry _rtspEntry;
        private static Entry _saveEntry;

        public static void Init()
        {
            MainWindowInit();
            SettingsWindowInit();
        }

        private static void MainWindowInit()
        {
            MainWindow.Destroyed += (sender, e) => Application.Quit();

            
            Box box = new(Orientation.Vertical, 15)
            {
                Margin = 45,
                MarginTop = 30,
                MarginBottom = 15
            };
            MainWindow.Add(box);
            
            
            Box rtspEntryBox = new(Orientation.Vertical, 10);
            box.Add(rtspEntryBox);

            Label rtspLabel = new()
            {
                Text = "Link to stream",
                Halign = Align.Center
            };
            rtspEntryBox.Add(rtspLabel);

            _rtspEntry = new()
            {
                Halign = Align.Fill
            };
            rtspEntryBox.Add(_rtspEntry);
            
            Button ffplayButton = new()
            {
                Label = "Check stream",
                Halign = Align.End
            };
            ffplayButton.Clicked += FfplayButton_Clicked;
            rtspEntryBox.Add(ffplayButton);


            Box saveEntryBox = new(Orientation.Vertical, 10);
            box.Add(saveEntryBox);

            Label saveLabel = new()
            {
                Text = "Path to save",
                Halign = Align.Center
            };
            saveEntryBox.Add(saveLabel);

            _saveEntry = new()
            {
                Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Halign = Align.Fill
            };
            saveEntryBox.Add(_saveEntry);

            Button openFolderViewButton = new()
            {
                Label = "Open",
                Halign = Align.End
            };
            openFolderViewButton.Clicked += OpenFolderViewButton_Clicked;
            saveEntryBox.Add(openFolderViewButton);


            Box commonNumberOfDaysBox = new(Orientation.Horizontal, 0)
            {
                Halign = Align.Center
            };
            box.Add(commonNumberOfDaysBox);

            Box numberOfDaysBox = new(Orientation.Horizontal, 10)
            {
                Homogeneous = true
            };
            commonNumberOfDaysBox.Add(numberOfDaysBox);

            Box childNodb1 = new(Orientation.Vertical, 0)
            {
                Halign = Align.End,
                Valign = Align.Center
            };
            Box childNodb2 = new(Orientation.Vertical, 0);
            Box childNodb3 = new(Orientation.Vertical, 0);
            numberOfDaysBox.Add(childNodb1);
            numberOfDaysBox.Add(childNodb2);
            numberOfDaysBox.Add(childNodb3);
            
            Label numberOfDaysLabel = new()
            {
                Text = "Number of days"
            };
            childNodb1.Add(numberOfDaysLabel);

            Entry numberOfDaysEntry = new()
            {
                InputPurpose = InputPurpose.Digits,
                PlaceholderText = "180",
                Halign = Align.Center
            };
            childNodb2.Add(numberOfDaysEntry);
            

            Box startRecordingBox = new(Orientation.Vertical, 10)
            {
                MarginTop = 20
            };
            box.PackEnd(startRecordingBox, false, false, 10);

            Separator separator1 = new(Orientation.Horizontal);
            startRecordingBox.Add(separator1);

            Box commonStartButtonBox = new(Orientation.Horizontal, 0)
            {
                Homogeneous = true
            };
            startRecordingBox.Add(commonStartButtonBox);

            Box childCsbb1 = new(Orientation.Vertical, 0);
            Box childCsbb2 = new(Orientation.Vertical, 0);
            Box childCsbb3 = new(Orientation.Vertical, 0);
            commonStartButtonBox.Add(childCsbb1);
            commonStartButtonBox.Add(childCsbb2);
            commonStartButtonBox.Add(childCsbb3);

            Button startButton = new()
            {
                Label = "Start recording",
                Halign = Align.Center
            };
            startButton.Clicked += StartButton_Clicked;
            childCsbb2.Add(startButton);
            
            Button openSettingsButton = new()
            {
                Halign = Align.End,
                Label = "Settings"
            };
            openSettingsButton.Clicked += OpenSettingsButton_Clicked;
            childCsbb3.Add(openSettingsButton);

            Separator separator2 = new(Orientation.Horizontal);
            startRecordingBox.Add(separator2);


            MainWindow.ShowAll();
        }

        private static void SettingsWindowInit()
        {
            _settingsWindow = new("Settings")
            {
                Resizable = false
            };


            Box box = new(Orientation.Vertical, 25)
            {
                Margin = 45,
                MarginTop = 30
            };
            _settingsWindow.Add(box);

            Box ffmpegBox = new(Orientation.Vertical, 10);
            box.Add(ffmpegBox);

            Label ffmpegLabel = new()
            {
                Text = "Path to FFmpeg"
            };
            ffmpegBox.Add(ffmpegLabel);

            Entry ffmpegEntry = new()
            {
                Halign = Align.Center,
                Text = FFmpeg.Path
            };
            ffmpegBox.Add(ffmpegEntry);


            Box ffplayBox = new(Orientation.Vertical, 10);
            box.Add(ffplayBox);

            Label ffplayLabel = new()
            {
                Text = "Path to FFplay"
            };
            ffplayBox.Add(ffplayLabel);

            Entry ffplayEntry = new()
            {
                Halign = Align.Center,
                Text = FFplay.Path
            };
            ffplayBox.Add(ffplayEntry);


            Box configBox = new(Orientation.Vertical, 10);
            box.Add(configBox);

            Label configLabel = new()
            {
                Text = "Path to config file"
            };
            configBox.Add(configLabel);

            Entry configEntry = new()
            {
                Halign = Align.Center,
                Text = Config.Path,
                IsEditable = false
            };
            configBox.Add(configEntry);


            Box tempBox = new(Orientation.Vertical, 10);
            box.Add(tempBox);

            Label tempLabel = new()
            {
                Text = "Path to temporary files"
            };
            tempBox.Add(tempLabel);

            Entry tempEntry = new()
            {
                Halign = Align.Center,
                Text = Temp.Path
            };
            tempBox.Add(tempEntry);
        }

        private static void FfplayButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_rtspEntry.Text))
            {
                if (_rtspEntry.Text.StartsWith("rtsp://"))
                {
                    if (IsRtspLinkValid(_rtspEntry.Text))
                    {
                        FFplay.Play(_rtspEntry.Text);
                    }
                }
                else
                {
                    MessageDialog md = new(
                        MainWindow,
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
                    MainWindow,
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
                MainWindow,
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
                var port = uri.Port == -1 ? 554 : uri.Port;

                using var client = new TcpClient();
                client.Connect(host, port);

                return true;
            }
            catch (Exception ex)
            {
                MessageDialog md = new(
                    MainWindow,
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

        private static void OpenSettingsButton_Clicked(object sender, EventArgs e)
        {
            _settingsWindow.ShowAll();
        }
    }
}