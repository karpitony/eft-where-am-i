using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace eft_where_am_i.Classes
{
    internal class SettingsHandler
    {
        private static readonly string DefaultFilePath = Path.Combine("assets", "settings.json");
        private readonly string _filePath;
        private AppSettings _settings;

        public SettingsHandler()
        {
            _filePath = DefaultFilePath;
            EnsureDirectoryExists();
            Load();
        }

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
        }
    }

    public class AppSettings
    {
        public bool isFirstRun { get; set; } = true;
        public bool auto_screenshot_detection { get; set; } = false;
        public string language { get; set; } = "en";
        public string screenshot_path { get; set; } = string.Empty;
        public List<string> screenshot_paths_list { get; set; } = new List<string>();
        public string latest_map { get; set; } = "ground-zero";
    }
}
