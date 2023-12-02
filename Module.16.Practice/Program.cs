using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module._16.Practice
{
    public class Program
    {
        private static string logFilePath;
        private static string pathToWatch;

        static void Main(string[] args)
        {
            Console.WriteLine("Программа отслеживания изменений в директории");

            try
            {
                ConfigureTracking();
                StartTracking();
                Console.WriteLine("");
                Console.WriteLine("Все изменение в директории: ");
                DisplayLog();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void ConfigureTracking()
        {
            Console.Write("Введите путь к отслеживаемой директории: ");
            pathToWatch = Console.ReadLine();

            if (!Directory.Exists(pathToWatch))
            {
                throw new ArgumentException("Указанная директория не существует.");
            }

            Console.Write("Введите путь к лог-файлу: ");
            logFilePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                throw new ArgumentException("Путь к лог-файлу не может быть пустым.");
            }
        }

        static void StartTracking()
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = pathToWatch;
                watcher.IncludeSubdirectories = true;

                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;

                watcher.EnableRaisingEvents = true;

                Console.WriteLine($"Отслеживание запущено для директории: {pathToWatch}");
                Console.WriteLine("Нажмите 'Q' для завершения.");

                while (Console.ReadKey().Key != ConsoleKey.Q) { }
            }

        }

        static void OnChanged(object sender, FileSystemEventArgs e)
        {
            LogAction($"[{DateTime.Now}] Изменение: {e.ChangeType} - {e.FullPath}");
        }

        static void OnRenamed(object sender, RenamedEventArgs e)
        {
            LogAction($"[{DateTime.Now}] Переименование: {e.OldFullPath} -> {e.FullPath}");
        }

        static void LogAction(string action)
        {
            try
            {
                File.AppendAllText(logFilePath, $"{action}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в лог: {ex.Message}");
            }
        }

        static void DisplayLog()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    string logContent = File.ReadAllText(logFilePath);
                    Console.WriteLine($"Путь к файлу: ({logFilePath}):");
                    Console.WriteLine("Содержимое лога ");
                    Console.WriteLine(logContent);
                }
                else
                {
                    Console.WriteLine($"Лог-файл не найден: {logFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении лога: {ex.Message}");
            }
        }
    }
}
