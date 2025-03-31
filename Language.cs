using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace TimelapseApp
{
    public static class Language
    {
        private static readonly Dictionary<int, string> _engPhrases = new()
        {
            {0, "\tThis program adds itself to crontab and creates a timelapse for a specified period of time." + Environment.NewLine +
                "\tThe timelapse will be recorded for 16 hours (from 7:00 to 23:00)" + Environment.NewLine +
                "\tThe program includes a console script which works automaticaly and graphical interface where you can configure some parameters of the script." + Environment.NewLine +
                "\tAs you can see right now, this is the main app window, but you can open another settings window." + Environment.NewLine +
                "\tIf you just want to start you should insert RTSP-link to your camera, choose a path to save final video and write a number of days you need the program will work." + Environment.NewLine +
                "\tBefore this I recommend you to check your RTSP-stream by clicking the \"Check stream\" button!" + Environment.NewLine +
                "\tIf you want to add a timestamp to video (your camera may not have it by default), (dis)allow to delete temporary files or change a path of temporary files you can do it by clicking the \"Settings\" button." + Environment.NewLine +
                "\tYou can delete an existing program record in crontab by clicking the \"Clear\" button." + Environment.NewLine +
                "\tYou can read program log in " + Environment.NewLine + Path.Combine(Config.Path, Process.GetCurrentProcess().ProcessName + "_log.txt")},
            { 1, "Help" },
            { 2, "Link to stream" },
            { 3, "Check stream" },
            { 4, "Path to save" },
            { 5, "Open" },
            { 6, "Number of days" },
            { 7, "Clear" },
            { 8, "Start recording" },
            { 9, "Settings" },
            { 10, "This RTSP-link is invalid" },
            { 11, "Select the path to save the file" },
            { 12, "Cancel" },
            { 13, "Are you sure you want to remove an existing program record in crontab and all related data?" },
            { 14, "If nesessary, make a backup copy of a program data" },
            { 15, "The path to save does not exist" },
            { 16, "The days count entry does not contain number" },
            { 17, "This count of days is too small" },
            { 18, "Do you really need the program to run for that long?" },
            { 19, "This timelapse will be created over" },
            { 20, "days" + Environment.NewLine + "You can close the application" },
            { 21, "Done!" },
            { 22, "Add timestamp" },
            { 23, "Path to Cron file" },
            { 24, "Path to config file" },
            { 25, "Path to temporary files" },
            { 26, "Close this window when you want to apply the settings" },
            { 27, "Information" },
            { 28, "Select the path to save the files" },
            { 29, "Such directory doesn't exist" },
            { 30, "There is no Internet connection" },
            { 31, "There is not installed FFmpeg" },
            { 32, "There is not installed FFplay" },
            { 33, "There is not installed FFprobe" },
            { 34, "There is not installed Crontab" },
            { 35, "Message" },
            { 36, "There is no text to check" },
            { 37, "This is not RTSP-link" },
            { 38, "This filepath is empty"},
            { 39, "There is no numbers in this filepath" },
            { 40, "The video time index" },
            { 41, "is lower than it could be" },
            { 42, "Video was not recorded" },
            { 43, "File" },
            { 44, "was not found" },
            { 45, "The acceleration index" },
            { 46, "Video was not accelerated" },
            { 47, "There is no videos to concat" },
            { 48, "The videos were not concated" },
            { 49, "The video was not repaired" },
            { 50, "I have finished all my work. I hope we will meet again :3" },
            { 51, "Recording is started" },
            { 52, "Recording have finished" },
            { 53, "This video is shorter than required" },
            { 54, "I am waiting for" },
            { 55, "Recording continues. Attempt" },
            { 56, "Recording have finished again" },
            { 57, "Today's videos are concating" },
            { 58, "Concating have done" },
            { 59, "Yes, video file exists and it's duration is" },
            { 60, "Accelerating is started" },
            { 61, "Accelerating have done" },
            { 62, "Yes, short video files exist, and I am starting to concat them" },
            { 63, "Concating is started" },
            { 64, "Cron record have changed" },
            { 65, "seconds" },
            { 66, "It is time to start recording for"},
            { 67, "Allow to delete\ntemporary files" },
            { 68, "Deleting files is not allowed" }

        };
        private static readonly Dictionary<int, string> _ruPhrases = new()
        {
            {0, "\tЭта программа добавляет себя в crontab и создаёт таймлапс в течение указанного периода времени." + Environment.NewLine +
                "\tТаймлапс будет записываться в течение 16 часов (с 7:00 до 23:00)" + Environment.NewLine +
                "\tПрограмма включает в себя консольный скрипт, работающий в автоматическом режиме, и графический интерфейс, в котором вы можете настроить некоторые параметры скрипта." + Environment.NewLine +
                "\tВ данный момент перед вами основное окно приложения, однако вы можете открыть другое окно с настройками." + Environment.NewLine +
                "\tЕсли вы просто хотите запустить работу программы, вам следует вставить RTSP-ссылку на вашу камеру, выбрать путь сохранения готового видео и написать число дней, в течение которых скрипт должен работать." + Environment.NewLine +
                "\tПеред началом рекомендую проверить ваш RTSP-поток с помощью нажатия кнопки \"Проверить поток\"!" + Environment.NewLine +
                "\tЕсли вы хотите добавить время на видео (на вашей камере может не быть их по умолчанию), разрешить/запретить удалять временные файлы или изменить путь к временным файлам, вы можете сделать это с помощью нажатия кнопки \"Настройки\"" + Environment.NewLine +
                "\tУдалить существующую запись программы в crontab вы можете, нажав кнопку \"Очистить\"" + Environment.NewLine +
                "\tВы можете изучить журнал событий программы в " + Environment.NewLine + Path.Combine(Config.Path, Process.GetCurrentProcess().ProcessName + "_log.txt")},
            { 1, "Помощь" },
            { 2, "Ссылка на поток" },
            { 3, "Проверить поток" },
            { 4, "Путь сохранения" },
            { 5, "Открыть" },
            { 6, "Количество дней" },
            { 7, "Очистить" },
            { 8, "Начать запись" },
            { 9, "Настройки" },
            { 10, "Эта RTSP-ссылка недействительна" },
            { 11, "Выберите путь сохранения файла" },
            { 12, "Отмена" },
            { 13, "Вы уверены, что хотите удалить уже имеющуюся запись кронтаб и связанные с ней файлы?"},
            { 14, "Если это необходимо - сделайте резервную копию файлов" },
            { 15, "Путь сохранения не существует" },
            { 16, "Поле ввода количества дней не содержит цифр" },
            { 17, "Число дней слишком саленькое" },
            { 18, "Вам действительно нужно, чтобы программа работала настолько долго?" },
            { 19, "Этот таймлапс будет создан через"},
            { 20, "дней" + Environment.NewLine + "Вы можете закрыть приложение" },
            { 21, "Готово!" },
            { 22, "Добавить дату и время" },
            { 23, "Путь к файлу Сron" },
            { 24, "Путь к конфигурационному файлу" },
            { 25, "Путь к временным файлам" },
            { 26, "Закройте это окно, когда захотите сохранить настройки" },
            { 27, "Информация" },
            { 28, "Выберите путь сохранения файлов" },
            { 29, "Такая директория не существует" },
            { 30, "Отсутствует подключение к Интернету" },
            { 31, "FFmpeg не установлен" },
            { 32, "FFplay не установлен" },
            { 33, "FFprobe не установлен" },
            { 34, "Crontab не установлен" },
            { 35, "Сообщение" },
            { 36, "Здесь нет текста для проверки" },
            { 37, "Это не RTSP-ссылка" },
            { 38, "Указанный файл не существует" },
            { 39, "В указанном пути к файлу нет цифр" },
            { 40, "Индекс времени видео" },
            { 41, "меньше, чем мог бы быть" },
            { 42, "Видео не было записано" },
            { 43, "Файл" },
            { 44, "не найден" },
            { 45, "Индекс ускорения видео" },
            { 46, "Видео не было ускорено" },
            { 47, "Здесь нет видео для обьединения" },
            { 48, "Видео не были обьединены" },
            { 49, "Видео не было восстановлено" },
            { 50, "Я закончил всю свою работу. Я надеюсь, мы встретимся снова :3" },
            { 51, "Запись началась" },
            { 52, "Запись окончена" },
            { 53, "Видео короче, чем необходимо" },
            { 54, "Я ожидаю" },
            { 55, "Запись продолжается. Попытка"},
            { 56, "Запись окончена снова" },
            { 57, "Сегодняшние видео обьединяются" },
            { 58, "Обьединение завершено" },
            { 59, "Да, видео существует, и его длительность"},
            { 60, "Ускорение началось" },
            { 61, "Ускорение завершено" },
            { 62, "Да, короткие видео существуют, и я начинаю их обьединять" },
            { 63, "Обьединение началось" },
            { 64, "Запись Cron была изменена" },
            { 65, "секунд" },
            { 66, "Пришло время начать запись на"},
            { 67, "Разрешить удалять\nвременные файлы" },
            { 68, "Удалять файлы запрещено" }
        };

        public static string GetPhrase(int phraseNumber)
        {
            return CultureInfo.CurrentUICulture.LCID switch
            {
                1049 => _ruPhrases.GetValueOrDefault(phraseNumber),
                _ => _engPhrases.GetValueOrDefault(phraseNumber)
            };
        }
    }
}