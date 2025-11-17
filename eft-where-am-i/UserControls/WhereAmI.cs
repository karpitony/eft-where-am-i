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

        private bool chkAutoScreenshot;

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

                // 4. 모든 준비가 끝난 후 WmiInitialize 호출
                WmiInitialize();
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
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
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
                        break;

                    case "full-screen":
                        btnFullScreen_Click(null, null);
                        break;

                    case "force-run":
                        btnForceRun_Click(null, null);
                        break;

                    case "link-clicked":
                        if (!string.IsNullOrEmpty(url))
                        {
                            System.Diagnostics.Process.Start(url);
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
                appSettings.latest_map = selectedMap;
                SaveSettings();  // 설정 저장
                siteUrl = $"https://tarkov-market.com/maps/{selectedMap}";
                webView2.Source = new Uri(siteUrl);
                whereAmIClick = false;
                WmiInitialize();
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

            await jsExecutor.SetInputValueAsync("input[type=\"text\"]", screenshot.Replace(".png", ""));
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

        private async void btnFullScreen_Click(object sender, EventArgs e)
        {
            await jsExecutor.ClickButtonAsync(Constants.FULL_SCREEN_BUTTON_SELECTOR);
        }

        private async void btnForceRun_Click(object sender, EventArgs e)
        {
            await CheckLocationAsync();
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