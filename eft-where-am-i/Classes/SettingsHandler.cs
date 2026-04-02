using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace eft_where_am_i.Classes
{
    internal class SettingsHandler
    {
        private static SettingsHandler _instance;
        public static SettingsHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsHandler();
                return _instance;
            }
        }

        private static readonly string DefaultFilePath = Path.Combine("assets", "settings.json");
        private readonly string _filePath;
        private AppSettings _settings;
        public event Action<AppSettings> SettingsChanged;

        private SettingsHandler()
        {
            _filePath = DefaultFilePath;
            EnsureDirectoryExists();
            Load();
        }

        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory ?? throw new Exception("Invalid file path."));
            }
        }

        private void Load()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    _settings = new AppSettings();
                    Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading JSON file: {ex.Message}");
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving JSON file: {ex.Message}");
            }
        }

        public T GetValue<T>(Func<AppSettings, T> selector) => selector(_settings);

        public void SetValue<T>(Action<AppSettings> updater)
        {
            updater(_settings);
            Save();
            SettingsChanged?.Invoke(_settings);
        }

        public AppSettings GetSettings() => _settings;

        public void UpdateSettings(AppSettings newSettings)
        {
            _settings = newSettings;
            Save();
            SettingsChanged?.Invoke(_settings);
        }

        #region Screenshot Path

        /// <summary>
        /// 캐시된 스크린샷 경로를 반환하거나, 없으면 탐색 후 캐시합니다.
        /// </summary>
        public string GetOrFindScreenshotPath()
        {
            if (!string.IsNullOrEmpty(_settings.screenshot_path) && Directory.Exists(_settings.screenshot_path))
                return _settings.screenshot_path;

            return ScreenshotPathSearch();
        }

        /// <summary>
        /// 스크린샷 폴더 탐색:
        /// 1. 내 문서/Escape From Tarkov/Screenshots
        /// 2. UserProfile + settings.json의 screenshot_paths_list
        /// </summary>
        public string ScreenshotPathSearch()
        {
            Exception lastKnownError = null;

            // [전략 1] 내 문서 경로
            try
            {
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string[] folderCandidates = { "Escape From Tarkov", "Escape from Tarkov" };

                foreach (string folderName in folderCandidates)
                {
                    string candidatePath = Path.Combine(documents, folderName, "Screenshots");
                    if (Directory.Exists(candidatePath))
                    {
                        SetValue<string>(s => s.screenshot_path = candidatePath);
                        return candidatePath;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Standard path detection failed: {ex.Message}");
                lastKnownError = ex;
            }

            // [전략 2] UserProfile + JSON 목록 기반 탐지 (폴백)
            try
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                if (!string.IsNullOrEmpty(homeDirectory) && _settings.screenshot_paths_list != null)
                {
                    foreach (string relativePath in _settings.screenshot_paths_list)
                    {
                        string fullPath = Path.Combine(homeDirectory, relativePath);
                        if (Directory.Exists(fullPath))
                        {
                            SetValue<string>(s => s.screenshot_path = fullPath);
                            return fullPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] UserProfile list detection failed: {ex.Message}");
                lastKnownError = ex;
            }

            if (lastKnownError != null)
                throw new Exception($"스크린샷 폴더 탐색 중 오류 발생: {lastKnownError.Message}", lastKnownError);

            throw new DirectoryNotFoundException(
                "모든 경로에서 'Escape From Tarkov/Screenshots' 폴더를 자동으로 탐지하지 못했습니다. 설정에서 수동으로 지정해주세요.");
        }

        #endregion

        #region Log Path

        /// <summary>
        /// 캐시된 로그 경로를 반환하거나, 없으면 탐색 후 캐시합니다.
        /// </summary>
        public string GetOrFindLogPath()
        {
            if (!string.IsNullOrEmpty(_settings.log_path) && Directory.Exists(_settings.log_path))
                return _settings.log_path;

            return LogPathSearch();
        }

        /// <summary>
        /// EFT 로그 폴더 탐색:
        /// 1. 레지스트리 Uninstall 키의 InstallLocation → Logs
        /// 2. %LOCALAPPDATA%\Battlestate Games\EFT\Logs
        /// 3. 실행 중인 EscapeFromTarkov.exe 프로세스 경로 → Logs
        /// </summary>
        public string LogPathSearch()
        {
            // [전략 1] 레지스트리에서 게임 설치 경로 탐지
            try
            {
                string registryPath = GetLogPathFromRegistry();
                if (!string.IsNullOrEmpty(registryPath))
                {
                    SetValue<string>(s => s.log_path = registryPath);
                    return registryPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Registry path detection failed: {ex.Message}");
            }

            // [전략 2] 런처 버전 %LOCALAPPDATA% 경로
            try
            {
                string launcherPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Battlestate Games", "EFT", "Logs");

                if (Directory.Exists(launcherPath))
                {
                    SetValue<string>(s => s.log_path = launcherPath);
                    return launcherPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Launcher path detection failed: {ex.Message}");
            }

            // [전략 3] 실행 중인 프로세스에서 탐지
            try
            {
                string processPath = GetLogPathFromProcess();
                if (!string.IsNullOrEmpty(processPath))
                {
                    SetValue<string>(s => s.log_path = processPath);
                    return processPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Process path detection failed: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// 레지스트리 Uninstall 키에서 EFT 설치 경로를 찾아 Logs 폴더를 반환합니다.
        /// HKLM\SOFTWARE\WOW6432Node\...\Uninstall\EscapeFromTarkov → InstallLocation
        /// </summary>
        private string GetLogPathFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov"))
                {
                    string installPath = key?.GetValue("InstallLocation")?.ToString();
                    if (string.IsNullOrEmpty(installPath)) return null;

                    string logsPath = Path.Combine(installPath, "Logs");
                    return Directory.Exists(logsPath) ? logsPath : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetLogPathFromRegistry failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 실행 중인 EscapeFromTarkov.exe 프로세스 경로에서 Logs 폴더를 반환합니다.
        /// </summary>
        private string GetLogPathFromProcess()
        {
            try
            {
                var processes = Process.GetProcessesByName("EscapeFromTarkov");
                if (processes.Length > 0)
                {
                    string exePath = processes[0].MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        string logsPath = Path.Combine(Path.GetDirectoryName(exePath), "Logs");
                        if (Directory.Exists(logsPath)) return logsPath;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] GetLogPathFromProcess failed: {ex.Message}");
            }
            return null;
        }

        #endregion
    }

    public class AppSettings
    {
        public bool auto_screenshot_detection { get; set; } = false;
        public bool auto_map_detection { get; set; } = false;
        public bool auto_panning { get; set; } = true;
        public bool auto_screenshot_cleanup { get; set; } = false;
        public string language { get; set; } = "en";
        public string screenshot_path { get; set; } = string.Empty;
        public string log_path { get; set; } = string.Empty;
        public List<string> screenshot_paths_list { get; set; } = new List<string>();
        public string latest_map { get; set; } = "ground-zero";
        public int dead_zone_percent { get; set; } = 93;
        public Dictionary<string, bool> panel_hidden_per_map { get; set; } = new Dictionary<string, bool>();
    }
}
