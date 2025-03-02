using System;
using System.IO;
using System.Net.Sockets;
using Gtk;

namespace TimelapseApp
{
    public class Interface
    {
        public static Window Main { get; } = new("Timelapse App");
        public static Window Settings { get; private set; }

        private static Entry _rtspEntry;
        private static Entry _saveEntry;
        private static Entry _crontabEntry;
        private static Entry _tempEntry;
        private static Entry _numberOfDaysEntry;

        private static readonly CheckButton _addTimestampButton = new() { Label = "Add timestamp" };

        private static Button _startButton;
        private static Button _ffplayButton;

        public static void Init()
        {
            MainWindow.Init();
        }

        private static class MainWindow
        {
            public static void Init()
            {
                Main.Destroyed += static (sender, e) => Application.Quit();


                Box box = new(Orientation.Vertical, 15)
                {
                    Margin = 45,
                    MarginTop = 30,
                    MarginBottom = 15
                };
                Main.Add(box);


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
                _rtspEntry.Changed += RtspEntry_Changed;
                rtspEntryBox.Add(_rtspEntry);

                _ffplayButton = new()
                {
                    Label = "Check stream",
                    Halign = Align.End,
                    Sensitive = false,
                };
                _ffplayButton.Clicked += FfplayButton_Clicked;
                rtspEntryBox.Add(_ffplayButton);


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
                _saveEntry.Changed += SaveEntry_Changed;
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

                _numberOfDaysEntry = new()
                {
                    InputPurpose = InputPurpose.Digits,
                    PlaceholderText = "180",
                    Halign = Align.Center
                };
                childNodb2.Add(_numberOfDaysEntry);


                Box startRecordingBox = new(Orientation.Vertical, 10)
                {
                    MarginTop = 20
                };
                box.PackEnd(startRecordingBox, false, false, 10);

                startRecordingBox.Add(new Separator(Orientation.Horizontal));

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

                _startButton = new()
                {
                    Label = "Start recording",
                    Halign = Align.Center,
                    Sensitive = false,
                };
                _startButton.Clicked += StartButton_Clicked;
                childCsbb2.Add(_startButton);

                Button openSettingsButton = new()
                {
                    Halign = Align.End,
                    Label = "Settings"
                };
                openSettingsButton.Clicked += OpenSettingsButton_Clicked;
                childCsbb3.Add(openSettingsButton);

                startRecordingBox.Add(new Separator(Orientation.Horizontal));


                Main.ShowAll();
            }

            private static void SaveEntry_Changed(object sender, EventArgs e)
            {
                if (FFmpeg.Exists && FFplay.Exists && Crontab.FileExists &&
                    !string.IsNullOrEmpty(_rtspEntry.Text) && !string.IsNullOrEmpty(_saveEntry.Text))
                        _startButton.Sensitive = true;
                else _startButton.Sensitive = false;
            }

            private static void RtspEntry_Changed(object sender, EventArgs e)
            {
                if (FFplay.Exists && !string.IsNullOrEmpty(_rtspEntry.Text))
                    _ffplayButton.Sensitive = true;
                else _ffplayButton.Sensitive = false;

                SaveEntry_Changed(sender, e);
            }

            private static void FfplayButton_Clicked(object sender, EventArgs e)
            {
                if (!string.IsNullOrEmpty(_rtspEntry.Text))
                {
                    if (_rtspEntry.Text.StartsWith("rtsp://"))
                    {
                        if (IsRtspLinkValid(_rtspEntry.Text))
                            FFplay.Play(_rtspEntry.Text);
                    }
                    else "This is not RTSP-link".Message(Main);
                }
                else "Empty input field".Message(Main);
            }

            private static void OpenFolderViewButton_Clicked(object sender, EventArgs e)
            {
                FileChooserDialog fileChooser = new(
                    "Select the path to save the file",
                    Main,
                    FileChooserAction.SelectFolder,
                    "Cancel", ResponseType.Cancel,
                    "Open", ResponseType.Accept)
                {
                    SelectMultiple = false
                };
                _ = fileChooser.SetCurrentFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

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
                    ex.Message.Message(Main);
                    return false;
                }
            }

            private static void StartButton_Clicked(object sender, EventArgs e)
            {
                int days = !string.IsNullOrEmpty(_numberOfDaysEntry.Text) ? int.Parse(_numberOfDaysEntry.Text) : 180;
                TimelapseScript.Create(_rtspEntry.Text, _saveEntry.Text, _addTimestampButton.Active);
                $"Done! This timelapse will be creating during {days} days".Message(Main);
            }

            private static void OpenSettingsButton_Clicked(object sender, EventArgs e)
            {
                SettingsWindow.Init();
                Settings.ShowAll();
            }
        }

        private static class SettingsWindow
        {
            public static void Init()
            {
                Settings = new("Settings")
                { 
                    Resizable = false, 
                    Modal = true, 
                    TransientFor = Main 
                };
                Settings.DeleteEvent += SettingsWindow_DeleteEvent;


                Box box = new(Orientation.Vertical, 25)
                {
                    Margin = 45,
                    MarginTop = 30
                };
                Settings.Add(box);


                Box addTimestampBox = new(Orientation.Horizontal, 0)
                {
                    Halign = Align.Center,
                };
                box.Add(addTimestampBox);
                addTimestampBox.Add(_addTimestampButton);


                Box crontabBox = new(Orientation.Vertical, 10);
                box.Add(crontabBox);

                Label crontabLabel = new()
                {
                    Text = "Path to Cron file"
                };
                crontabBox.Add(crontabLabel);

                _crontabEntry = new()
                {
                    Halign = Align.Center,
                    Text = Crontab.Path,
                };
                crontabBox.Add(_crontabEntry);


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

                _tempEntry = new()
                {
                    Halign = Align.Center,
                    Text = Temp.Path
                };
                tempBox.Add(_tempEntry);
            }

            private static void SettingsWindow_DeleteEvent(object sender, DeleteEventArgs e)
            {
                if (_tempEntry.Text != Path.Combine(Path.GetTempPath(), "TimelapseApp") && !Directory.Exists(_tempEntry.Text))
                {
                    "Temporary directory doesn't exist".Message(Settings);
                    e.RetVal = true;
                }
            }
        }
    }
}