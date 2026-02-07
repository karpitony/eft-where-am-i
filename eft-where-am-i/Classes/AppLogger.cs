using System;
using System.IO;

namespace eft_where_am_i.Classes
{
    public static class AppLogger
    {
        private static readonly string _logPath;
        private static readonly object _lock = new object();

        static AppLogger()
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");
        }

        public static void Log(string level, string tag, string message)
        {
            try
            {
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{tag}] {message}";
                lock (_lock)
                {
                    File.AppendAllText(_logPath, line + Environment.NewLine);
                }
            }
            catch
            {
                // 로깅 실패는 무시
            }
        }

        public static void Info(string tag, string message) => Log("INFO", tag, message);
        public static void Warn(string tag, string message) => Log("WARN", tag, message);
        public static void Error(string tag, string message) => Log("ERROR", tag, message);
        public static void Debug(string tag, string message) => Log("DEBUG", tag, message);
    }
}
