using System;
using System.IO;

namespace FileManagerApp
{
    public static class Logger
    {
        private static string logFile = "errors.log";

        public static void Log(string message)
        {
            try
            {
                string text = $"{DateTime.Now}: {message}\n";
                File.AppendAllText(logFile, text);
            }
            catch
            {
                Console.WriteLine("Ошибка записи в лог.");
            }
        }
    }
}
