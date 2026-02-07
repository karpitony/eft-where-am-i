using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace eft_where_am_i.Classes
{
    internal class SettingsHandler
    {
        // 싱글톤 인스턴스
        private static SettingsHandler _instance;
        public static SettingsHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsHandler();  // 처음 접근 시 생성
                return _instance;
            }
        }

        private SettingsHandler()
        {
            _filePath = DefaultFilePath;
            EnsureDirectoryExists();
            Load();
        }


        private static readonly string DefaultFilePath = Path.Combine("assets", "settings.json");
        private readonly string _filePath;
        private AppSettings _settings;
        public event Action<AppSettings> SettingsChanged;


        /// 디렉터리 존재 여부 확인 후 없으면 생성
        private void EnsureDirectoryExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory ?? throw new Exception("Invalid file path."));
            }
        }

        /// JSON 파일을 불러오고 AppSettings 객체로 변환
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
                    Save(); // 파일이 없으면 기본 설정 저장
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading JSON file: {ex.Message}");
            }
        }

        /// 현재 AppSettings 객체를 JSON 파일로 저장
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

        /// 특정 설정 값을 가져옴
        public T GetValue<T>(Func<AppSettings, T> selector)
        {
            return selector(_settings);
        }

        /// 특정 설정 값을 수정하고 저장
        public void SetValue<T>(Action<AppSettings> updater)
        {
            updater(_settings);
            Save();
            SettingsChanged?.Invoke(_settings);
        }

        /// 전체 AppSettings 객체를 반환
        public AppSettings GetSettings()
        {
            return _settings;
        }

        /// 전체 AppSettings 객체를 수정하고 저장
        public void UpdateSettings(AppSettings newSettings)
        {
            _settings = newSettings;
            Save();
            SettingsChanged?.Invoke(_settings);
        }

        /// <summary>
        /// 캐시된 스크린샷 경로를 가져오거나, 없으면 탐색하여 캐시하고 반환합니다.
        /// (탐색, 캐싱, 반환 함수)
        /// </summary>
        /// <returns>성공 시 폴더 경로</returns>
        /// <exception cref="DirectoryNotFoundException">탐색에 성공했으나 폴더가 존재하지 않는 경우</exception>
        /// <exception cref="Exception">탐색 과정 중 권한 등의 오류로 실패한 경우</exception>
        public string GetOrFindScreenshotPath()
        {
            // 1. (캐시 확인) 이미 경로가 설정되어 있고 "실제로 존재하는지" 확인
            if (!string.IsNullOrEmpty(_settings.screenshot_path))
            {
                if (Directory.Exists(_settings.screenshot_path))
                {
                    return _settings.screenshot_path; // 캐시 유효, 즉시 반환
                }
                // 캐시된 경로는 있으나 실제 폴더가 사라진 경우: 탐색 재시도
            }

            // 2. (탐색) 캐시가 없거나 무효하므로 실제 탐색 수행
            // PerformPathSearch는 이제 성공 시 경로 반환, 실패 시 예외를 throw합니다.
            return ScreenshotPathSearch(); // 여기서 예외가 발생할 수 있음
        }

        /// <summary>
        /// 실제 스크린샷 폴더 탐색 로직을 수행합니다.
        /// </summary>
        /// <returns>성공 시 폴더 경로</returns>
        /// <exception cref="DirectoryNotFoundException">모든 전략으로 탐색했으나 폴더를 찾지 못한 경우</exception>
        /// <exception cref="Exception">탐색 중 파일 시스템 접근 오류 등이 발생한 경우</exception>
        public string ScreenshotPathSearch()
        {
            // 탐색 중 발생한 내부 오류를 저장하기 위한 변수
            Exception lastKnownError = null;

            // --- [전략 1] "내 문서"를 이용한 동적 탐지 (최우선 순위) ---
            try
            {
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Tarkov 폴더 이름 후보 2개
                string[] folderCandidates =
                {
                    "Escape From Tarkov",   // 기존
                    "Escape from Tarkov"    // 새로 추가
                };

                foreach (string folderName in folderCandidates)
                {
                    string candidatePath = Path.Combine(documents, folderName, "Screenshots");

                    if (Directory.Exists(candidatePath))
                    {
                        SetValue<string>(settings =>
                        {
                            settings.screenshot_path = candidatePath;
                        });

                        return candidatePath; // 탐지 성공
                    }
                }
            }
            catch (Exception ex)
            {
                // MyDocuments 접근 권한이 없는 등 심각한 오류일 수 있음
                Console.WriteLine($"[Error] Standard path detection failed: {ex.Message}");
                lastKnownError = ex; // 첫 번째 오류 기록
            }

            // --- [전략 2] "UserProfile" + JSON 목록 기반 탐지 (폴백) ---
            // (전략 1이 예외를 발생시켰더라도, 폴백 전략은 시도해봐야 함)
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
                            SetValue<string>(settings =>
                            {
                                settings.screenshot_path = fullPath;
                            });
                            return fullPath; // 탐지 성공!
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // UserProfile 접근 권한이 없는 등 심각한 오류일 수 있음
                Console.WriteLine($"[Error] UserProfile list detection failed: {ex.Message}");
                lastKnownError = ex; // 두 번째 오류 기록 (덮어쓰기)
            }

            // --- [최종] 탐지 실패 ---

            // 만약 탐색 '중'에 오류(예: 권한)가 있었다면, 그것이 주된 실패 원인이므로 해당 예외를 throw
            if (lastKnownError != null)
            {
                throw new Exception($"스크린샷 폴더 탐색 중 오류 발생: {lastKnownError.Message}", lastKnownError);
            }

            // 탐색 중 오류는 없었으나, 단순히 폴더를 '못 찾은' 경우
            throw new DirectoryNotFoundException("모든 경로에서 'Escape From Tarkov/Screenshots' 폴더를 자동으로 탐지하지 못했습니다. 설정에서 수동으로 지정해주세요.");
        }

        /// <summary>
        /// 캐시된 로그 경로를 가져오거나, 없으면 탐색하여 캐시하고 반환합니다.
        /// </summary>
        /// <returns>성공 시 폴더 경로, 실패 시 null</returns>
        public string GetOrFindLogPath()
        {
            // 1. (캐시 확인) 이미 경로가 설정되어 있고 "실제로 존재하는지" 확인
            if (!string.IsNullOrEmpty(_settings.log_path))
            {
                if (Directory.Exists(_settings.log_path))
                {
                    return _settings.log_path; // 캐시 유효, 즉시 반환
                }
                // 캐시된 경로는 있으나 실제 폴더가 사라진 경우: 탐색 재시도
            }

            // 2. (탐색) 캐시가 없거나 무효하므로 실제 탐색 수행
            return LogPathSearch();
        }

        /// <summary>
        /// EFT 로그 폴더 탐색 로직을 수행합니다.
        /// 검사 순서:
        /// 1. %LOCALAPPDATA%\Battlestate Games\EFT\Logs (런처 버전)
        /// 2. C:\Program Files (x86)\Steam\steamapps\common\Escape from Tarkov\build\Logs (스팀 버전)
        /// 3. 실행 중인 EscapeFromTarkov.exe 프로세스 경로에서 Logs 폴더
        /// </summary>
        /// <returns>성공 시 폴더 경로, 실패 시 null</returns>
        public string LogPathSearch()
        {
            // --- [전략 1] 런처 버전 경로 ---
            try
            {
                string launcherPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Battlestate Games", "EFT", "Logs"
                );

                if (Directory.Exists(launcherPath))
                {
                    SetValue<string>(settings =>
                    {
                        settings.log_path = launcherPath;
                    });
                    return launcherPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Launcher path detection failed: {ex.Message}");
            }

            // --- [전략 2] 스팀 버전 경로 ---
            try
            {
                string steamPath = @"C:\Program Files (x86)\Steam\steamapps\common\Escape from Tarkov\build\Logs";

                if (Directory.Exists(steamPath))
                {
                    SetValue<string>(settings =>
                    {
                        settings.log_path = steamPath;
                    });
                    return steamPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Steam path detection failed: {ex.Message}");
            }

            // --- [전략 3] 실행 중인 프로세스에서 탐지 ---
            try
            {
                string processPath = GetLogPathFromProcess();
                if (!string.IsNullOrEmpty(processPath))
                {
                    SetValue<string>(settings =>
                    {
                        settings.log_path = processPath;
                    });
                    return processPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Process path detection failed: {ex.Message}");
            }

            // 모든 탐색 실패
            return null;
        }

        /// <summary>
        /// 실행 중인 EscapeFromTarkov.exe 프로세스에서 로그 경로를 찾습니다.
        /// </summary>
        private string GetLogPathFromProcess()
        {
            try
            {
                var processes = Process.GetProcessesByName("EscapeFromTarkov");
                if (processes.Length > 0)
                {
                    var exePath = processes[0].MainModule?.FileName;
                    if (!string.IsNullOrEmpty(exePath))
                    {
                        var logsPath = Path.Combine(Path.GetDirectoryName(exePath), "Logs");
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
    }

    public class AppSettings
    {
        public bool auto_screenshot_detection { get; set; } = false;
        public bool auto_map_detection { get; set; } = false;
        public bool auto_panning { get; set; } = true;
        public string language { get; set; } = "en";
        public string screenshot_path { get; set; } = string.Empty;
        public string log_path { get; set; } = string.Empty;
        public List<string> screenshot_paths_list { get; set; } = new List<string>();
        public string latest_map { get; set; } = "ground-zero";
        public int dead_zone_percent { get; set; } = 93;
        public Dictionary<string, bool> panel_hidden_per_map { get; set; } = new Dictionary<string, bool>();
    }
}
