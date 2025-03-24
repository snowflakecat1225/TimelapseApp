using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Gtk;
using Pango;

namespace TimelapseApp
{
    public static class Interface
    {
        public static bool IsInitialised { get; set; } = false;
        private static bool _timestampChecked = Config.GetTimestampChecked();

        private static string _tempPath = Temp.Path;

        

        public class Windows
        {
            public static readonly Window Main = new("Timelapse App");
            public static Window Settings { get; set; }
        }

        public static void Init()
        {
            new MainWindow().ShowAll();
        }

        private class MainWindow
        {
            private static Button _startButton;
            private static Button _ffplayButton;
            private static Button _clearButton;

            private static Entry _rtspEntry;
            private static Entry _saveEntry;
            private static Entry _numberOfDaysEntry;

            public MainWindow()
            {
                Windows.Main.Destroyed += delegate { Application.Quit(); };


                var titlebar = new HeaderBar
                {
                    Title = Windows.Main.Title,
                    ShowCloseButton = true
                };
                Windows.Main.Titlebar = titlebar;

                var helpButton = new Button()
                {
                    Image = new Image(Stock.DialogQuestion, IconSize.Menu)
                };
                helpButton.Clicked += (sender, e) =>
                {
                    MessageDialog messageDialog = new(
                        Windows.Main,
                        DialogFlags.Modal,
                        MessageType.Info,
                        ButtonsType.Close,
                        Language.GetPhrase(0))
                    {
                        Title = Language.GetPhrase(1)
                    };
                    messageDialog.Run();
                    messageDialog.Destroy();
                };
                titlebar.PackStart(helpButton);


                Box box = new(Orientation.Vertical, 15)
                {
                    Margin = 45,
                    MarginTop = 30,
                    MarginBottom = 15
                };
                Windows.Main.Add(box);


                Box rtspEntryBox = new(Orientation.Vertical, 10);
                box.Add(rtspEntryBox);

                rtspEntryBox.Add(new Label()
                {
                    Text = Language.GetPhrase(2),
                    Halign = Align.Center
                });

                _rtspEntry = new()
                {
                    Halign = Align.Fill
                };
                _rtspEntry.Changed += RtspEntry_Changed;
                rtspEntryBox.Add(_rtspEntry);

                _ffplayButton = new()
                {
                    Label = Language.GetPhrase(3),
                    Halign = Align.End,
                    Sensitive = false,
                };
                _ffplayButton.Clicked += static async (s, e) => await FfplayButton_Clicked(s, e);
                rtspEntryBox.Add(_ffplayButton);


                Box saveEntryBox = new(Orientation.Vertical, 10);
                box.Add(saveEntryBox);

                saveEntryBox.Add(new Label()
                {
                    Text = Language.GetPhrase(4),
                    Halign = Align.Center
                });

                _saveEntry = new()
                {
                    Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    Halign = Align.Fill
                };
                _saveEntry.Changed += SaveEntry_Changed;
                saveEntryBox.Add(_saveEntry);

                Button openFolderViewButton = new()
                {
                    Label = Language.GetPhrase(5),
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

                childNodb1.Add(new Label()
                {
                    Text = Language.GetPhrase(6),
                });

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

                _clearButton = new()
                {
                    Label = Language.GetPhrase(7),
                    Halign = Align.Start,
                    Sensitive = Config.Exists | !string.IsNullOrEmpty(Crontab.Get(Environment.ProcessPath)) | Directory.Exists(Temp.Path)
                };
                _clearButton.Clicked += ClearButton_Clicked;
                childCsbb1.Add(_clearButton);

                _startButton = new()
                {
                    Label = Language.GetPhrase(8),
                    Halign = Align.Center,
                    Sensitive = false,
                };
                _startButton.Clicked += StartButton_Clicked;
                childCsbb2.Add(_startButton);

                Button openSettingsButton = new()
                {
                    Halign = Align.End,
                    Label = Language.GetPhrase(9)
                };
                openSettingsButton.Clicked += OpenSettingsButton_Clicked;
                childCsbb3.Add(openSettingsButton);

                startRecordingBox.Add(new Separator(Orientation.Horizontal));
            }

            public void ShowAll()
            {
                Windows.Main.ShowAll();
            }

            private static void SaveEntry_Changed(object sender, EventArgs e)
            {
                if (FFmpeg.Exists && FFplay.Exists && Crontab.Exists &&
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

            private static async Task FfplayButton_Clicked(object sender, EventArgs e)
            {
                if (_rtspEntry.Text.IsRtspLinkValid())
                    await FFplay.Play(_rtspEntry.Text);
                else Language.GetPhrase(10).Message(false, false);
            }

            private static void OpenFolderViewButton_Clicked(object sender, EventArgs e)
            {
                FileChooserDialog fileChooser = new(
                    Language.GetPhrase(11),
                    Windows.Main,
                    FileChooserAction.SelectFolder,
                    Language.GetPhrase(12), ResponseType.Cancel,
                    Language.GetPhrase(5), ResponseType.Accept)
                {
                    SelectMultiple = false
                };
                _ = fileChooser.SetCurrentFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

                fileChooser.SetPosition(WindowPosition.CenterOnParent);

                if (fileChooser.Run() == (int)ResponseType.Accept)
                    _saveEntry.Text = fileChooser.CurrentFolder;

                fileChooser.Destroy();
            }

            private static void ClearButton_Clicked(object sender, EventArgs e)
            {
                MessageDialog clearConfirmationDialog = new(
                    Windows.Main,
                    DialogFlags.Modal,
                    MessageType.Question,
                    ButtonsType.YesNo,
                    Language.GetPhrase(13));
                int response = clearConfirmationDialog.Run();
                clearConfirmationDialog.Destroy();
                
                if (response == (int)ResponseType.Yes)
                {
                    MessageDialog clearInformationDialog = new(
                        Windows.Main,
                        DialogFlags.Modal,
                        MessageType.Info,
                        ButtonsType.Ok,
                        Language.GetPhrase(14));
                    clearInformationDialog.Run();
                    clearInformationDialog.Destroy();

                    Script.Delete();
                    _clearButton.Sensitive = false;
                }
            }

            private static void StartButton_Clicked(object sender, EventArgs e)
            {
                if (!_rtspEntry.Text.IsRtspLinkValid())
                    return;

                if (!Directory.Exists(_saveEntry.Text))
                {
                    Language.GetPhrase(15).Message(false, false);
                    return;
                }
                
                if (!_numberOfDaysEntry.Text.IsNumber())
                {
                    Language.GetPhrase(16).Message(false, false);
                    return;
                }

                int days = !string.IsNullOrEmpty(_numberOfDaysEntry.Text) ? int.Parse(_numberOfDaysEntry.Text) : 180;
                if (days < 1)
                {
                    Language.GetPhrase(17).Message(false, false);
                    return;
                }
                if (days > 16 * 60 * 60)
                {
                    Language.GetPhrase(18).Message(false, false);
                    return;
                }
                
                Script.Create(_rtspEntry.Text, _saveEntry.Text, _timestampChecked, days, _tempPath);
                _clearButton.Sensitive = true;
                MessageDialog messageDialog = new(
                    Windows.Main,
                    DialogFlags.Modal,
                    MessageType.Info,
                    ButtonsType.Close,
                    $"{Language.GetPhrase(19)} {days} {Language.GetPhrase(20)}")
                {
                    Title = Language.GetPhrase(21)
                };
                messageDialog.Run();
    
                messageDialog.Destroy();
            }

            private static void OpenSettingsButton_Clicked(object sender, EventArgs e)
            {
                new SettingsWindow().ShowAll();
            }
        }

        private class SettingsWindow
        {
            private static Entry _tempEntry;
            private static bool isThisTheFirstShowing = true;

            public SettingsWindow()
            {
                Windows.Settings = new(Language.GetPhrase(9))
                {
                    Resizable = false,
                    Modal = true,
                    TransientFor = Windows.Main
                };
                Windows.Settings.DeleteEvent += Settings_DeleteEvent;


                Box box = new(Orientation.Vertical, 25)
                {
                    Margin = 45,
                    MarginTop = 30
                };
                Windows.Settings.Add(box);


                CheckButton addTimestampButton = new()
                {
                    Halign = Align.Center,
                    Label = Language.GetPhrase(22),
                    Active = _timestampChecked
                };
                addTimestampButton.Toggled += delegate { _timestampChecked = addTimestampButton.Active; };
                box.Add(addTimestampButton);


                Box configBox = new(Orientation.Vertical, 10);
                box.Add(configBox);

                configBox.Add(new Label()
                {
                    Text = Language.GetPhrase(24)
                });

                Entry configEntry = new()
                {
                    Halign = Align.Fill,
                    Text = Config.Path,
                    IsEditable = false
                };
                configBox.Add(configEntry);


                Box tempBox = new(Orientation.Vertical, 10);
                box.Add(tempBox);

                tempBox.Add(new Label()
                {
                    Text = Language.GetPhrase(25)
                });

                _tempEntry = new()
                {
                    Halign = Align.Fill,
                    Text = _tempPath
                };
                tempBox.Add(_tempEntry);

                Button openFolderViewButton = new()
                {
                    Label = Language.GetPhrase(5),
                    Halign = Align.End
                };
                openFolderViewButton.Clicked += OpenFolderViewButton_Clicked;
                tempBox.Add(openFolderViewButton);
            }

            public void ShowAll()
            {
                Windows.Settings.ShowAll();
                if (isThisTheFirstShowing)
                {
                    MessageDialog messageDialog = new(
                        Windows.Settings,
                        DialogFlags.Modal,
                        MessageType.Info,
                        ButtonsType.Close,
                        Language.GetPhrase(26))
                    {
                        Title = Language.GetPhrase(27)
                    };
                    messageDialog.Run();
                    messageDialog.Destroy();
                    isThisTheFirstShowing = false;
                }
                
            }

            private static void OpenFolderViewButton_Clicked(object sender, EventArgs e)
            {
                FileChooserDialog fileChooser = new(
                    Language.GetPhrase(28),
                    Windows.Settings,
                    FileChooserAction.SelectFolder,
                    Language.GetPhrase(12), ResponseType.Cancel,
                    Language.GetPhrase(5), ResponseType.Accept)
                {
                    SelectMultiple = false
                };
                _ = fileChooser.SetCurrentFolder(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

                fileChooser.SetPosition(WindowPosition.CenterOnParent);

                if (fileChooser.Run() == (int)ResponseType.Accept)
                    _tempEntry.Text = fileChooser.CurrentFolder;

                fileChooser.Destroy();
            }

            private void Settings_DeleteEvent(object sender, DeleteEventArgs e)
            {
                if (_tempEntry.Text != Path.Combine(Path.GetTempPath(), "TimelapseApp") && !Directory.Exists(_tempEntry.Text))
                {
                    Language.GetPhrase(29).Message(true, false);
                    e.RetVal = true;
                }
                else _tempPath = _tempEntry.Text;
            }
        }
    }
}