using System;
using System.Diagnostics;
using System.IO;

static class ArrowHelper
{
    public static int GetArrowSelection(int maxIndex)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();

            for (int i = 0; i <= maxIndex; i++)
            {
                Console.Write(i == selectedIndex ? ">> " : "   ");
                Console.WriteLine($"Option {i + 1}");
            }

            key = Console.ReadKey().Key;

            if (key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key == ConsoleKey.DownArrow && selectedIndex < maxIndex)
            {
                selectedIndex++;
            }
        } while (key != ConsoleKey.Enter && key != ConsoleKey.Escape);

        return selectedIndex;
    }
}

static class Explorer
{
    public static void RunExplorer()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Просмотреть диски");
            Console.WriteLine("2. Выйти из проводника");

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            switch (keyInfo.KeyChar)
            {
                case '1':
                    DisplayDrives();
                    break;
                case '2':
                    return;
            }
        }
    }

    private static void DisplayDrives()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите диск:");

            DriveInfo[] drives = DriveInfo.GetDrives();
            int selectedDriveIndex = ArrowHelper.GetArrowSelection(drives.Length - 1);

            if (selectedDriveIndex != -1)
            {
                ExploreDrive(drives[selectedDriveIndex]);
            }
            else
            {
                break;
            }
        }
    }

    private static int DisplayMenu(DriveInfo[] drives)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("Выберите диск:");

            for (int i = 0; i < drives.Length; i++)
            {
                Console.Write(i == selectedIndex ? ">> " : "   ");
                Console.WriteLine($"{drives[i].Name}");
            }

            key = Console.ReadKey().Key;

            if (key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key == ConsoleKey.DownArrow && selectedIndex < drives.Length - 1)
            {
                selectedIndex++;
            }
        } while (key != ConsoleKey.Enter && key != ConsoleKey.Escape);

        return selectedIndex;
    }

    private static void ExploreDrive(DriveInfo drive)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Информация о диске {drive.Name}:");
            Console.WriteLine($"Свободное место: {drive.TotalFreeSpace / (1024 * 1024 * 1024)} ГБ");
            Console.WriteLine($"Общий объем: {drive.TotalSize / (1024 * 1024 * 1024)} ГБ");

            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Просмотреть содержимое диска");
            Console.WriteLine("2. Вернуться к выбору диска");

            ConsoleKeyInfo choiceKey = Console.ReadKey();
            if (choiceKey.KeyChar == '1')
            {
                ExploreDirectory(drive.RootDirectory);
            }
            else if (choiceKey.KeyChar == '2')
            {
                return;
            }
        }
    }

    private static void ExploreDirectory(DirectoryInfo directory)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine($"Содержимое папки {directory.FullName}:");

            FileSystemInfo[] items = directory.GetFileSystemInfos();

            for (int i = 0; i < items.Length; i++)
            {
                Console.Write(i == selectedIndex ? ">> " : "   ");
                Console.WriteLine($"{items[i].Name} ({(items[i] is DirectoryInfo ? "Папка" : "Файл")})");
            }

            key = Console.ReadKey().Key;

            if (key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key == ConsoleKey.DownArrow && selectedIndex < items.Length - 1)
            {
                selectedIndex++;
            }
            else if (key == ConsoleKey.Enter)
            {
                if (items[selectedIndex] is DirectoryInfo)
                {
                    ExploreDirectory((DirectoryInfo)items[selectedIndex]);
                }
                else if (items[selectedIndex] is FileInfo)
                {
                    RunFile(items[selectedIndex].FullName);
                }
            }
            else if (key == ConsoleKey.Escape)
            {
                if (directory.Parent != null)
                {
                    ExploreDirectory(directory.Parent);
                }
                else
                {
                    ExploreDrive(new DriveInfo(directory.Root.FullName));
                }
            }
        } while (key != ConsoleKey.Escape);
    }

    private static void RunFile(string filePath)
    {
        Console.Clear();
        Console.WriteLine($"Запуск файла: {filePath}");

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске файла: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }
}

class Program
{
    static void Main()
    {
        Explorer.RunExplorer();
    }
}
