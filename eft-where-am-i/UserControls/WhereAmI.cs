using System;
using System.IO;
using System.Linq; 
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using eft_where_am_i.Classes;
using System.Runtime.InteropServices;

namespace eft_where_am_i
{
    public partial class WhereAmI : UserControl
    {
        private readonly SettingsHandler settingsHandler; // SettingsHandler 인스턴스
        private AppSettings appSettings; // AppSettings 참조
        private JavaScriptExecutor jsExecutor;
        private QuestRepository questRepository;
        private FloorManager floorManager;
        private string[] mapList = { 
            "ground-zero", 
            "factory", 
            "customs", 
            "interchange", 
            "woods", 
            "shoreline", 
            "lighthouse",
            "reserve", 
            "streets", 
            "lab", 
            "labyrinth", 
            "terminal" 
        };
        private string siteUrl;
        private bool whereAmIClick = false;
        private string screenshotPath;
        private FileSystemWatcher watcher;
        private LogWatcherService logWatcher;

        private bool chkAutoScreenshot;
        private bool isFloorEditMode = false;
        private GlobalHotkeyManager hotkeyManager;

        public WhereAmI()
        {
            InitializeComponent();
            settingsHandler = SettingsHandler.Instance;             // 싱글톤 인스턴스 사용
            settingsHandler.SettingsChanged += OnSettingsChanged;   // 세팅 변경될 때마다 호출됨
            LoadSettings();                                         // 동기작업
            siteUrl = $"https://tarkov-market.com/maps/{appSettings.latest_map}";
            
            // Load 이벤트 핸들러 등록
            this.Load += WhereAmI_Load;
        }

        private async void WhereAmI_Load(object sender, EventArgs e)
        {
            try
            {
                // 1. UI용 WebView2 초기화
                await InitializeWebViewUI();

                // 2. 콘텐츠용 WebView2 초기화
                await InitializeWebViewContent();

                // 3. 모든 WebView가 초기화된 후에 jsExecutor 생성
                jsExecutor = new JavaScriptExecutor(webView2);
                questRepository = new QuestRepository();
                floorManager = new FloorManager();

                // 4. LogWatcher 초기화
                InitializeLogWatcher();

                // 5. 모든 준비가 끝난 후 WmiInitialize 호출
                WmiInitialize();

                // 퀘스트 복원/리스너 주입은 NavigationCompleted 핸들러에서 처리

                // 6. 글로벌 핫키 매니저 초기화 (EFT 활성 시 Ctrl+Numpad로 층 전환)
                hotkeyManager = new GlobalHotkeyManager();
                hotkeyManager.FloorHotkeyPressed += OnFloorHotkeyPressed;
            }
            catch (Exception ex)
            {
                // Initialize... 메서드에서 throw한 예외를 여기서 최종 처리
                MessageBox.Show($"WebView 초기화 중 심각한 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OnSettingsChanged(AppSettings updatedSettings)
        {
            // 새로운 설정 반영
            appSettings = updatedSettings;

            // 화면 갱신 (언어/경로 등 UI 업데이트)
            LoadSettings();
            string language = appSettings.language;

            // webView2_panel_ui.CoreWebView2가 null이 아닌지 확인하여
            // 컨트롤이 초기화되었을 때만 스크립트를 실행합니다.
            if (webView2_panel_ui.CoreWebView2 != null)
            {
                try
                {
                    await webView2_panel_ui.ExecuteScriptAsync($"setLanguage('{language}')");
                }
                catch (Exception ex)
                {
                    // 초기화 직후 드물게 발생하는 예외를 대비한 방어 코드
                    MessageBox.Show($"설정 변경 중 스크립트 실행 오류: {ex.Message}", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            // 만약 null이라면 (아직 초기화 전),
            // 어차피 InitializeWebViewUI()의 NavigationCompleted 이벤트 핸들러가
            // 나중에 최신 appSettings 값을 읽어 언어를 설정할 것이므로
            // 여기서 별도로 처리할 필요가 없습니다.
        }


        private async Task InitializeWebViewContent()
        {
            // 고유한 사용자 데이터 폴더 생성 (임시 폴더 + GUID 사용)
            string userDataFolder = Path.Combine(Path.GetTempPath(), "MyAppWebView2_Content", Guid.NewGuid().ToString());
            CoreWebView2Environment env = null;
            try
            {
                // 사용자 데이터 폴더를 지정하여 새로운 WebView2 환경 생성
                env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView2.EnsureCoreWebView2Async(env);
            }
            catch (COMException comEx) when (comEx.ErrorCode == unchecked((int)0x8007139F))
            {
                MessageBox.Show($"웹 콘텐츠용 WebView2 초기화 오류: {comEx.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"웹 콘텐츠용 WebView2 초기화 예외: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            // WebView2 콘텐츠 메시지 수신 핸들러 등록
            webView2.CoreWebView2.WebMessageReceived += WebView2Content_WebMessageReceived;

            // 페이지 로드 완료 핸들러 등록 (퀘스트 복원/리스너 주입을 페이지 로드 완료 후 수행)
            webView2.NavigationCompleted += WebView2_NavigationCompleted;

            // 외부 URL 로드 (예외 처리 포함)
            try
            {
                webView2.Source = new Uri(siteUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"웹 콘텐츠 URL 설정 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private async Task InitializeWebViewUI()
        {
            // 고유한 사용자 데이터 폴더 생성 (임시 폴더 + GUID 사용)
            string userDataFolder = Path.Combine(Path.GetTempPath(), "MyAppWebView2", Guid.NewGuid().ToString());
            CoreWebView2Environment env = null;
            try
            {
                // 사용자 데이터 폴더를 지정하여 새로운 WebView2 환경 생성
                env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView2_panel_ui.EnsureCoreWebView2Async(env);


                // 가상 호스트 매핑 코드: 예를 들어, 번역 파일들이 저장된 폴더를 매핑
                string translationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations");
                webView2_panel_ui.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "appassets",
                    translationFolder,
                    CoreWebView2HostResourceAccessKind.Allow
                );
            }
            catch (COMException comEx) when (comEx.ErrorCode == unchecked((int)0x8007139F))
            {
                MessageBox.Show($"WebView2 초기화 중 오류 발생: {comEx.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 초기화 중 예외 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

            // HTML 파일 로드
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "html/panel.html");
            if (File.Exists(htmlPath))
            {
                webView2_panel_ui.Source = new Uri(htmlPath);
            }

            webView2_panel_ui.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            // HTML이 완전히 로드된 후 명령 전달
            webView2_panel_ui.NavigationCompleted += async (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    try
                    {
                        // 언어 설정 전송
                        string language = appSettings.language;
                        await webView2_panel_ui.ExecuteScriptAsync($"setLanguage('{language}')");

                        // 콤보박스에 맵 목록 전송
                        string mapListJson = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
                        await webView2_panel_ui.ExecuteScriptAsync($"populateMapList('{mapListJson}', '{appSettings.latest_map}')");

                        // 체크박스 상태 전송
                        await webView2_panel_ui.ExecuteScriptAsync($"setCheckboxState({appSettings.auto_screenshot_detection.ToString().ToLower()})");

                        // 자동 맵 감지 체크박스 상태 전송
                        await webView2_panel_ui.ExecuteScriptAsync($"setAutoMapCheckboxState({appSettings.auto_map_detection.ToString().ToLower()})");

                        // 자동 패닝 체크박스 상태 전송
                        await webView2_panel_ui.ExecuteScriptAsync($"setAutoPanningCheckboxState({appSettings.auto_panning.ToString().ToLower()})");

                        // 스크린샷 자동 삭제 체크박스 상태 전송
                        await webView2_panel_ui.ExecuteScriptAsync(
                            $"setAutoScreenshotCleanupCheckboxState({appSettings.auto_screenshot_cleanup.ToString().ToLower()})");

                        // 디버그 모드 플래그 전송
#if DEBUG
                        await webView2_panel_ui.ExecuteScriptAsync("setDebugMode(true)");
#else
                        await webView2_panel_ui.ExecuteScriptAsync("setDebugMode(false)");
#endif
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"JavaScript 명령 전송 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            };
        }

        // 메시지 수신 핸들러
        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string rawMessage = e.WebMessageAsJson.Trim('"').Replace("\\\"", "\"");

                JObject message = JObject.Parse(rawMessage);

                // 안전하게 속성 접근
                string action = message["action"]?.ToString() ?? "";
                string map = message["map"]?.ToString() ?? "";
                bool isChecked = message["isChecked"] != null && bool.Parse(message["isChecked"].ToString());
                string url = message["url"]?.ToString() ?? "";

                // 메시지 처리
                switch (action.ToLower())
                {
                    case "map-selected":
                        if (!string.IsNullOrEmpty(map))
                        {
                            HandleMapSelection(map);
                        }
                        break;

                    case "checkbox-updated":
                        appSettings.auto_screenshot_detection = isChecked;
                        SaveSettings();  // 설정 변경 저장
                        UpdateWatcherState(isChecked);
                        break;

                    case "hide-show-panel":
                        btnHideShowPannel_Click(null, null);
                        // 패널 상태 저장 (클릭 처리 후 딜레이를 두고 상태 읽기)
                        _ = SavePanelStateAsync();
                        break;

                    case "full-screen":
                        btnFullScreen_Click(null, null);
                        break;

                    case "force-run":
                        btnForceRun_Click(null, null);
                        break;

                    case "auto-map-toggle":
                        UpdateLogWatcherState(isChecked);
                        break;

                    case "auto-panning-toggle":
                        appSettings.auto_panning = isChecked;
                        SaveSettings();
                        break;

                    case "auto-screenshot-cleanup-toggle":
                        appSettings.auto_screenshot_cleanup = isChecked;
                        SaveSettings();
                        if (logWatcher != null)
                        {
                            if (isChecked && !appSettings.auto_map_detection)
                                logWatcher.Start();
                            else if (!isChecked && !appSettings.auto_map_detection)
                                logWatcher.Stop();
                        }
                        break;

                    case "toggle-floor-edit-mode":
                        await ToggleFloorEditModeAsync();
                        break;

                    case "floor-db-updated":
                        floorManager?.Reload();
                        break;

                    case "link-clicked":
                        if (!string.IsNullOrEmpty(url))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                        }
                        break;

                    default:
                        MessageBox.Show($"알 수 없는 action: {action}", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메시지 처리 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleMapSelection(string selectedMap)
        {
            if (!string.IsNullOrEmpty(selectedMap))
            {
                // 맵 변경 - 퀘스트는 실시간으로 저장되므로 별도 저장 불필요

                appSettings.latest_map = selectedMap;
                SaveSettings();  // 설정 저장
                siteUrl = $"https://tarkov-market.com/maps/{selectedMap}";
                webView2.Source = new Uri(siteUrl);
                whereAmIClick = false;
                WmiInitialize();

                // 퀘스트 복원/리스너 주입은 NavigationCompleted 핸들러에서 처리
            }
        }

        private async void WebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess) return;

            // jsExecutor가 아직 초기화되지 않은 경우 무시
            if (jsExecutor == null) return;

            // 데드존 auto-pan 스크립트 재주입 (새 페이지 로드 시)
            await jsExecutor.ExecuteScriptAsync(Constants.DEAD_ZONE_AUTO_PAN_SCRIPT);

            // 퀘스트 컨테이너 로드 대기 (DOM 준비 완료까지 대기)
            bool containerReady = await jsExecutor.WaitForQuestContainerAsync(15000);

            // 패널 숨김 상태 복원 (WaitForQuestContainer 후 = DOM 준비 완료 상태)
            if (appSettings.panel_hidden_per_map.TryGetValue(appSettings.latest_map, out bool isHidden))
            {
                if (isHidden)
                {
                    await jsExecutor.ClickButtonAsync(Constants.HIDE_SHOW_PANNE_BUTTON_SELECTOR);
                }
            }

            if (containerReady)
            {
                // 클릭 리스너 주입
                await jsExecutor.InjectQuestClickListenerAsync();
                // 퀘스트 복원
                await RestoreQuestsAsync(appSettings.latest_map);
            }
        }

        private void LoadSettings()
        {
            try
            {
                appSettings = settingsHandler.GetSettings();                // SettingsHandler에서 설정 로드
                screenshotPath = settingsHandler.GetOrFindScreenshotPath(); // 스크린샷 경로 설정

                chkAutoScreenshot = appSettings.auto_screenshot_detection;
                // 설정값을 기준으로 FileSystemWatcher를 초기화합니다.
                UpdateWatcherState(chkAutoScreenshot);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정을 로드하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveSettings()
        {
            try
            {
                settingsHandler.UpdateSettings(appSettings); // SettingsHandler를 통해 설정 저장
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정을 저장하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetLatestFile()
        {
            if (!Directory.Exists(screenshotPath))
            {
                return null; // 경로가 존재하지 않으면 null 반환
            }

            var directoryInfo = new DirectoryInfo(screenshotPath);
            var files = directoryInfo.GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();

            if (files.Count == 0)
            {
                return null; // 파일이 없으면 null 반환
            }

            return files.FirstOrDefault()?.Name;
        }

        private async void WmiInitialize()
        {
            await Task.Delay(4000);
            await jsExecutor.ClickButtonAsync(Constants.FULL_SCREEN_BUTTON_SELECTOR);
            if(!whereAmIClick)
            {
                whereAmIClick = true;
                await jsExecutor.ClickButtonAsync(Constants.WHERE_AM_I_BUTTON_SELECTOR);
                await Task.Delay(500);
            }
            await jsExecutor.ExecuteScriptAsync(Constants.ADD_DIRECTION_INDICATORS_SCRIPT);
            await jsExecutor.ExecuteScriptAsync(Constants.DEAD_ZONE_AUTO_PAN_SCRIPT);
        }

        private async Task CheckLocationAsync()
        {
            string screenshot = GetLatestFile();
            if (screenshot == null) return;

            if (!await jsExecutor.CheckInputAble())
            {
                whereAmIClick = true;
                await jsExecutor.ClickButtonAsync(Constants.WHERE_AM_I_BUTTON_SELECTOR);
                await Task.Delay(500);
            }

            string filenameWithoutExt = screenshot.Replace(".png", "");
            await jsExecutor.SetInputValueAsync("input[type=\"text\"]", filenameWithoutExt);

            // Z좌표 파싱 후 자동 층 전환
            await AutoSwitchFloorAsync(filenameWithoutExt);

            // 마커 렌더링 대기 후 데드존 auto-pan (설정이 활성화된 경우에만)
            if (appSettings.auto_panning)
            {
                await Task.Delay(300);
                await jsExecutor.AutoPanToMarkerAsync(appSettings.dead_zone_percent);
            }
        }

        private async Task AutoSwitchFloorAsync(string filename)
        {
            if (floorManager == null) return;

            try
            {
                // 파일명 형식: YYYY-MM-DD[HH-MM]_x, y, z_quatX, quatY, quatZ, quatW_speed
                // 예: 2026-01-10[03-59]_-318.44, 24.84, -107.49_0.00000, 0.82497, 0.00000, 0.56518_3.98 (0)
                //           [0]              [1]                    [2]                         [3]
                string[] parts = filename.Split('_');
                if (parts.Length < 2) return;

                // parts[1]이 좌표 부분 (예: "-318.44, 24.84, -107.49")
                string[] coords = parts[1].Split(',');
                if (coords.Length < 3) return;

                // 각 좌표에서 공백 trim 필요
                if (!double.TryParse(coords[0].Trim(), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double xCoord))
                    return;

                if (!double.TryParse(coords[1].Trim(), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double yCoord))
                    return;

                if (!double.TryParse(coords[2].Trim(), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out double zCoord))
                    return;

                string floorName = floorManager.GetFloorName(appSettings.latest_map, xCoord, yCoord, zCoord);
                if (!string.IsNullOrEmpty(floorName))
                {
                    await Task.Delay(500); // Wait for marker to appear
                    await jsExecutor.ClickFloorAsync(floorName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Floor] Auto floor switch error: {ex.Message}");
            }
        }

        private void UpdateWatcherState(bool isEnabled)
        {
            // bool 필드 값을 새 상태로 업데이트
            chkAutoScreenshot = isEnabled;

            if (isEnabled)
            {
                if (string.IsNullOrEmpty(screenshotPath) || !Directory.Exists(screenshotPath))
                {
                    MessageBox.Show("올바르지 않은 경로입니다. 설정 페이지에서 경로를 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkAutoScreenshot = false; // 상태를 다시 false로
                    appSettings.auto_screenshot_detection = false;
                    SaveSettings();

                    // (중요) UI에도 반영
                    _ = webView2_panel_ui.ExecuteScriptAsync($"setCheckboxState(false)");
                    return;
                }

                // 와처가 이미 실행 중이면 중복 생성 방지
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }

                watcher = new FileSystemWatcher
                {
                    Path = screenshotPath,
                    Filter = "*.png",
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

                watcher.Created += OnScreenshotCreated;
                watcher.EnableRaisingEvents = true;
            }
            else
            {
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Created -= OnScreenshotCreated;
                    watcher.Dispose();
                    watcher = null;
                }
            }

            // (선택사항) 이 메서드에서 직접 설정을 저장하도록 변경
            // appSettings.auto_screenshot_detection = isEnabled;
            // SaveSettings();
        }

        private async void OnScreenshotCreated(object sender, FileSystemEventArgs e)
        {
            // 파일 생성 후 사용 가능해질 때까지 잠시 대기
            await Task.Delay(500);

            // UI 스레드에서 CheckLocationAsync 호출
            this.Invoke(new MethodInvoker(async () => await CheckLocationAsync()));
        }

        private async void btnHideShowPannel_Click(object sender, EventArgs e)
        {
            await jsExecutor.ClickButtonAsync(Constants.HIDE_SHOW_PANNE_BUTTON_SELECTOR);
        }

        private async Task SavePanelStateAsync()
        {
            await Task.Delay(500); // 버튼 클릭 처리 완료 대기
            bool isHidden = await jsExecutor.IsPanelHiddenAsync();
            appSettings.panel_hidden_per_map[appSettings.latest_map] = isHidden;
            SaveSettings();
        }

        private async void btnFullScreen_Click(object sender, EventArgs e)
        {
            await jsExecutor.ClickButtonAsync(Constants.FULL_SCREEN_BUTTON_SELECTOR);
        }

        private async void btnForceRun_Click(object sender, EventArgs e)
        {
            await CheckLocationAsync();
        }

        private async Task RestoreQuestsAsync(string mapName)
        {
            try
            {
                var quests = questRepository.GetQuests(mapName);
                foreach (var questName in quests)
                {
                    await jsExecutor.SelectQuestByNameAsync(questName);
                    await Task.Delay(300); // Wait between selections to avoid race conditions
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"퀘스트 복원 중 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ToggleFloorEditModeAsync()
        {
            if (isFloorEditMode)
            {
                // Exit edit mode
                AppLogger.Info("FloorEdit", "Exiting floor edit mode");
                await jsExecutor.DisableFloorEditModeAsync();
                isFloorEditMode = false;
                // Update button text
                if (webView2_panel_ui.CoreWebView2 != null)
                {
                    await webView2_panel_ui.ExecuteScriptAsync(
                        "document.getElementById('floorDbEditorButton').textContent = i18next.t('floorDbEditorButton') || 'Edit Floor Zones';");
                }
            }
            else
            {
                // Enter edit mode (no calibration needed - pixel coordinate based)
                AppLogger.Info("FloorEdit", "Entering floor edit mode...");

                // Get existing zones and floors for current map
                string zonesJson = floorManager?.GetZonesJson(appSettings.latest_map) ?? "[]";
                string floorsJson = floorManager?.GetFloorsJson(appSettings.latest_map) ?? "[]";

                // Enable edit mode with overlay + editor UI
                await jsExecutor.EnableFloorEditModeAsync(zonesJson, floorsJson);
                isFloorEditMode = true;

                // Update button text
                if (webView2_panel_ui.CoreWebView2 != null)
                {
                    await webView2_panel_ui.ExecuteScriptAsync(
                        "document.getElementById('floorDbEditorButton').textContent = 'Exit Edit Mode';");
                }
            }
        }

        /// <summary>
        /// tarkov-market WebView2 콘텐츠에서 오는 메시지 핸들러 (폴리곤 에디터용)
        /// </summary>
        private async void WebView2Content_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string rawMessage = e.WebMessageAsJson.Trim('"').Replace("\\\"", "\"");
                AppLogger.Debug("WebView2Content", $"Message received: {rawMessage}");

                JObject message = JObject.Parse(rawMessage);
                string action = message["action"]?.ToString() ?? "";

                switch (action.ToLower())
                {
                    case "polygon-vertex-added":
                        // Vertex added on map - currently handled in JS, no C# action needed
                        break;

                    case "quest-toggled":
                        string questName = message["questName"]?.ToString();
                        bool isSelected = message["isSelected"]?.Value<bool>() ?? false;

                        if (!string.IsNullOrEmpty(questName))
                        {
                            if (isSelected)
                                questRepository.AddQuest(appSettings.latest_map, questName);
                            else
                                questRepository.RemoveQuest(appSettings.latest_map, questName);
                        }
                        break;

                    case "save-floor-zones":
                        string zonesData = message["data"]?.ToString() ?? "[]";
                        AppLogger.Info("FloorEdit", $"save-floor-zones received for map: {appSettings.latest_map}");
                        AppLogger.Debug("FloorEdit", $"Zones data length: {zonesData.Length}");

                        floorManager?.UpdateZonesFromJson(appSettings.latest_map, zonesData);

                        // Exit edit mode after save
                        await jsExecutor.DisableFloorEditModeAsync();
                        isFloorEditMode = false;
                        if (webView2_panel_ui.CoreWebView2 != null)
                        {
                            await webView2_panel_ui.ExecuteScriptAsync(
                                "document.getElementById('floorDbEditorButton').textContent = i18next.t('floorDbEditorButton') || 'Edit Floor Zones';");
                        }
                        MessageBox.Show("Zones saved successfully!", "Floor Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case "exit-floor-edit-mode":
                        await jsExecutor.DisableFloorEditModeAsync();
                        isFloorEditMode = false;
                        if (webView2_panel_ui.CoreWebView2 != null)
                        {
                            await webView2_panel_ui.ExecuteScriptAsync(
                                "document.getElementById('floorDbEditorButton').textContent = i18next.t('floorDbEditorButton') || 'Edit Floor Zones';");
                        }
                        break;

                    default:
                        AppLogger.Debug("WebView2Content", $"Unhandled action: {action}");
                        break;
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("WebView2Content", $"Message handling error: {ex.Message}");
            }
        }

        private void InitializeLogWatcher()
        {
            // Get log path from settings, or auto-detect if not set
            string logPath = settingsHandler.GetOrFindLogPath();

            if (string.IsNullOrEmpty(logPath))
            {
                AppLogger.Warn("LogWatcher", "Log path not found. Auto map detection will not work.");
                return;
            }

            logWatcher = new LogWatcherService(logPath);
            logWatcher.MapDetected += OnMapDetectedFromLog;
            logWatcher.RaidEnded += OnRaidEndedFromLog;
            if (appSettings.auto_map_detection || appSettings.auto_screenshot_cleanup)
            {
                logWatcher.Start();
            }
        }

        private async void OnRaidEndedFromLog()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnRaidEndedFromLog()));
                return;
            }

            if (!appSettings.auto_screenshot_cleanup) return;
            if (string.IsNullOrEmpty(screenshotPath) || !Directory.Exists(screenshotPath)) return;

            await Task.Delay(3000); // 게임이 마지막 스크린샷 쓰기 완료 대기

            try
            {
                var pngFiles = Directory.GetFiles(screenshotPath, "*.png");
                if (pngFiles.Length == 0) return;

                AppLogger.Info("ScreenshotCleanup", $"Cleaning up {pngFiles.Length} PNG file(s)");
                int deleted = 0, failed = 0;
                foreach (var file in pngFiles)
                {
                    try { File.Delete(file); deleted++; }
                    catch (Exception ex)
                    {
                        AppLogger.Warn("ScreenshotCleanup", $"Failed: {Path.GetFileName(file)} - {ex.Message}");
                        failed++;
                    }
                }
                AppLogger.Info("ScreenshotCleanup", $"Done. Deleted: {deleted}, Failed: {failed}");
            }
            catch (Exception ex)
            {
                AppLogger.Error("ScreenshotCleanup", $"Cleanup error: {ex.Message}");
            }
        }

        private void OnMapDetectedFromLog(string mapName)
        {
            // Ensure we're on the UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnMapDetectedFromLog(mapName)));
                return;
            }

            // Only switch if it's a different map
            if (string.Equals(appSettings.latest_map, mapName, StringComparison.OrdinalIgnoreCase))
                return;

            // Update the panel UI dropdown
            if (webView2_panel_ui.CoreWebView2 != null)
            {
                _ = webView2_panel_ui.ExecuteScriptAsync($"document.getElementById('mapSelect').value = '{mapName}';");
            }

            HandleMapSelection(mapName);
        }

        private void UpdateLogWatcherState(bool isEnabled)
        {
            appSettings.auto_map_detection = isEnabled;
            SaveSettings();

            if (logWatcher == null) return;

            if (isEnabled)
            {
                logWatcher.Start();
            }
            else
            {
                if (!appSettings.auto_screenshot_cleanup)
                    logWatcher.Stop();
            }
        }

        // Ctrl+Numpad 핫키 → 층 이름 후보 매핑 (순서대로 시도, 첫 매칭 클릭)
        private static readonly Dictionary<int, string[]> FloorHotkeyMap = new Dictionary<int, string[]>
        {
            { 0, new[] { "Basement", "Bunker" } },  // Numpad 0 → 지하
            { 1, new[] { "Main" } },                 // Numpad 1 → Ground/Main
            { 2, new[] { "Level 2" } },              // Numpad 2 → 2층
            { 3, new[] { "Level 3" } },              // Numpad 3 → 3층
            { 4, new[] { "Level 4" } },              // Numpad 4 → 4층
            { 5, new[] { "Level 5" } }               // Numpad 5 → 5층
        };

        private async void OnFloorHotkeyPressed(int keyIndex)
        {
            if (jsExecutor == null) return;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnFloorHotkeyPressed(keyIndex)));
                return;
            }

            if (FloorHotkeyMap.TryGetValue(keyIndex, out string[] candidates))
            {
                await jsExecutor.ClickFloorByFirstMatchAsync(candidates);
            }
        }

        const int MAX_SLIDING_HEIGHT = 110;
        const int MIN_SLIDING_HEIGHT = 0;
        const int STEP_SLIDING = 10;
        int _posSliding = 110;

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                checkBoxHide.Text = "∨ Click to Unfold";
            }
            else
            {
                checkBoxHide.Text = "∧ Click to Fold";
            }

            timerSliding.Start();
        }

        private void timerSliding_Tick(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                _posSliding -= STEP_SLIDING;
                checkBoxHide.Top = _posSliding;
                if (_posSliding <= MIN_SLIDING_HEIGHT)
                    timerSliding.Stop();
            }
            else
            {
                _posSliding += STEP_SLIDING;
                checkBoxHide.Top = _posSliding;
                if (_posSliding >= MAX_SLIDING_HEIGHT)
                    timerSliding.Stop();

            }

            panel1.Height = _posSliding;
        }
    }
}