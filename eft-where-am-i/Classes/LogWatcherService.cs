using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace eft_where_am_i.Classes
{
    public class LogWatcherService : IDisposable
    {
        private Timer _pollTimer;
        private string _currentLogFolder;
        private string _currentLogFile;
        private long _lastReadPosition;
        private readonly string _logsBasePath;
        private DateTime _lastFolderCheck = DateTime.MinValue;
        private static readonly Regex MapRegex = new Regex(@"scene preset path:maps/([^.]+)\.bundle", RegexOptions.Compiled);
        private static readonly Regex TransitEndRegex = new Regex(
            @"\[Transit\] `([a-f0-9]+)` Count:(\d+), EventPlayer:(True|False)",
            RegexOptions.Compiled);

        private static readonly Dictionary<string, string> MapNameMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "woods_preset", "woods" },
            { "customs_preset", "customs" },
            { "bigmap", "customs" },
            { "shoreline_preset", "shoreline" },
            { "shopping_mall", "interchange" },
            { "rezerv_base_preset", "reserve" },
            { "rezervbase", "reserve" },
            { "lighthouse_preset", "lighthouse" },
            { "city_preset", "streets" },
            { "tarkovstreets", "streets" },
            { "factory_day_preset", "factory" },
            { "factory_night_preset", "factory" },
            { "factory4_day", "factory" },
            { "factory4_night", "factory" },
            { "sandbox_preset", "ground-zero" },
            { "sandbox_high_preset", "ground-zero" },
            { "sandbox", "ground-zero" },
            { "sandbox_high", "ground-zero" },
            { "laboratory_preset", "lab" },
            { "laboratory", "lab" },
            { "labyrinth_preset", "labyrinth" },
        };

        private const int POLL_INTERVAL_MS = 2000;
        private const int FOLDER_CHECK_INTERVAL_SEC = 30;

        public event Action<string> MapDetected;
        public event Action RaidEnded;

        public LogWatcherService(string logsBasePath)
        {
            _logsBasePath = logsBasePath;
        }

        public void Start()
        {
            if (!Directory.Exists(_logsBasePath))
            {
                AppLogger.Warn("LogWatcher", $"Logs base path not found: {_logsBasePath}");
                return;
            }

            FindLatestLogFolder();

            // Start polling timer
            _pollTimer = new Timer(OnPollTick, null, POLL_INTERVAL_MS, POLL_INTERVAL_MS);
            AppLogger.Info("LogWatcher", $"Started polling every {POLL_INTERVAL_MS}ms");
        }

        public void Stop()
        {
            if (_pollTimer != null)
            {
                _pollTimer.Dispose();
                _pollTimer = null;
            }
            _currentLogFile = null;
            _lastReadPosition = 0;
            AppLogger.Info("LogWatcher", "Stopped");
        }

        private void OnPollTick(object state)
        {
            try
            {
                // Periodically check for new log folders
                if ((DateTime.Now - _lastFolderCheck).TotalSeconds >= FOLDER_CHECK_INTERVAL_SEC)
                {
                    FindLatestLogFolder();
                    _lastFolderCheck = DateTime.Now;
                }

                // Check for new log file in current folder
                if (!string.IsNullOrEmpty(_currentLogFolder))
                {
                    FindLatestLogFile();
                }

                // Read new content
                ReadNewContent();
            }
            catch (Exception ex)
            {
                AppLogger.Error("LogWatcher", $"Poll tick error: {ex.Message}");
            }
        }

        private void FindLatestLogFolder()
        {
            try
            {
                if (!Directory.Exists(_logsBasePath)) return;

                var dirs = new DirectoryInfo(_logsBasePath).GetDirectories();
                if (dirs.Length == 0) return;

                DirectoryInfo latest = null;
                foreach (var dir in dirs)
                {
                    if (latest == null || dir.CreationTime > latest.CreationTime)
                        latest = dir;
                }

                if (latest != null && !string.Equals(_currentLogFolder, latest.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    AppLogger.Info("LogWatcher", $"Switched to log folder: {latest.Name}");
                    _currentLogFolder = latest.FullName;
                    _currentLogFile = null;
                    _lastReadPosition = 0;
                    FindLatestLogFile();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("LogWatcher", $"Error finding latest log folder: {ex.Message}");
            }
        }

        private void FindLatestLogFile()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentLogFolder) || !Directory.Exists(_currentLogFolder))
                    return;

                var files = Directory.GetFiles(_currentLogFolder, "*application*.log");
                if (files.Length == 0) return;

                string latestFile = null;
                DateTime latestTime = DateTime.MinValue;
                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    if (fi.LastWriteTime > latestTime)
                    {
                        latestTime = fi.LastWriteTime;
                        latestFile = file;
                    }
                }

                if (latestFile != null && !string.Equals(_currentLogFile, latestFile, StringComparison.OrdinalIgnoreCase))
                {
                    AppLogger.Info("LogWatcher", $"Watching log file: {Path.GetFileName(latestFile)}");
                    _currentLogFile = latestFile;
                    // Start from end - only read new lines
                    try
                    {
                        _lastReadPosition = new FileInfo(latestFile).Length;
                    }
                    catch
                    {
                        _lastReadPosition = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("LogWatcher", $"Error finding latest log file: {ex.Message}");
            }
        }

        private void ReadNewContent()
        {
            if (string.IsNullOrEmpty(_currentLogFile) || !File.Exists(_currentLogFile))
                return;

            try
            {
                using (var fs = new FileStream(_currentLogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    if (fs.Length <= _lastReadPosition)
                        return;

                    fs.Seek(_lastReadPosition, SeekOrigin.Begin);

                    using (var reader = new StreamReader(fs))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            ParseLine(line);
                        }

                        _lastReadPosition = fs.Position;
                    }
                }
            }
            catch (IOException)
            {
                // File locked by game - will retry on next poll
            }
            catch (Exception ex)
            {
                AppLogger.Error("LogWatcher", $"Error reading log content: {ex.Message}");
            }
        }

        private void ParseLine(string line)
        {
            var match = MapRegex.Match(line);
            if (match.Success)
            {
                string rawMapName = match.Groups[1].Value;

                if (MapNameMapping.TryGetValue(rawMapName, out string mappedName))
                {
                    AppLogger.Info("LogWatcher", $"Map detected: {rawMapName} -> {mappedName}");
                    MapDetected?.Invoke(mappedName);
                }
                else
                {
                    AppLogger.Warn("LogWatcher", $"Unknown map name in log: {rawMapName}");
                }
                return;
            }

            var transitEndMatch = TransitEndRegex.Match(line);
            if (transitEndMatch.Success)
            {
                AppLogger.Info("LogWatcher", "Raid end detected (Transit pattern)");
                RaidEnded?.Invoke();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
