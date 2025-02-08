using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using eft_where_am_i.Classes;
namespace eft_where_am_i
{
    public class WebViewMessage
    {
        public string Action { get; set; } = string.Empty;
        public string Map { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = false;
        public string Url { get; set; } = string.Empty;
    }

    public partial class WhereAmI : UserControl
    {
        private readonly SettingsHandler settingsHandler; // SettingsHandler 인스턴스
        private AppSettings appSettings; // AppSettings 참조
        private readonly JavaScriptExecutor jsExecutor;
        private string[] mapList = { "ground-zero", "factory", "customs", "interchange", "woods", "shoreline", "lighthouse", "reserve", "streets", "lab" };
        private string siteUrl;
        private bool whereAmIClick = false;
        private string screenshotPath;
        private FileSystemWatcher watcher;

        private bool chkAutoScreenshot;

        public WhereAmI()
        {
            InitializeComponent();
            settingsHandler = new SettingsHandler(); // SettingsHandler 초기화
            LoadSettings();
            InitializeWebViewUI();
            jsExecutor = new JavaScriptExecutor(webView2); // WebView2 전달
            siteUrl = $"https://tarkov-market.com/maps/{appSettings.latest_map}";
            webView2.Source = new Uri(siteUrl);
            WmiFullScreen();
        }

        private async void InitializeWebViewUI()
        {
            await webView2_panel_ui.EnsureCoreWebView2Async(null);

            // HTML 파일 로드
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pages/panel.html");
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
                        // 콤보박스에 맵 목록 전송
                        string mapListJson = Newtonsoft.Json.JsonConvert.SerializeObject(mapList);
                        await webView2_panel_ui.ExecuteScriptAsync($"populateMapList('{mapListJson}', '{appSettings.latest_map}')");

                        // 체크박스 상태 전송
                        await webView2_panel_ui.ExecuteScriptAsync($"setCheckboxState({appSettings.auto_screenshot_detection.ToString().ToLower()})");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"JavaScript 명령 전송 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        chkAutoScreenshot = isChecked;
                        appSettings.auto_screenshot_detection = chkAutoScreenshot;
                        SaveSettings();  // 설정 변경 저장
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
                WmiFullScreen();  // 전체 화면 버튼 자동 클릭
            }
        }

        private void LoadSettings()
        {
            try
            {
                appSettings = settingsHandler.GetSettings(); // SettingsHandler에서 설정 로드
                screenshotPath = appSettings.screenshot_path; // 스크린샷 경로 설정
                if (string.IsNullOrEmpty(screenshotPath) || !Directory.Exists(screenshotPath))
                {
                    try
                    {
                        CheckAndSetScreenshotPath();
                    }
                    catch
                    {
                        MessageBox.Show("올바르지 않은 경로입니다. 설정 페이지에서 경로를 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                chkAutoScreenshot = appSettings.auto_screenshot_detection;
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

        private async void WmiFullScreen()
        {
            await Task.Delay(3000);
            await jsExecutor.ClickButtonAsync("#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > button");
        }

        private async Task CheckLocationAsync()
        {
            string screenshot = GetLatestFile();
            if (screenshot == null) return;

            if (!whereAmIClick)
            {
                whereAmIClick = true;
                await jsExecutor.ClickButtonAsync("#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > div.d-flex.ml-15.fs-0 > button");
                await Task.Delay(500);
            }

            await jsExecutor.SetInputValueAsync("input[type=\"text\"]", screenshot.Replace(".png", ""));
            await ChangeMarkerAsync();
        }

        private async Task ChangeMarkerAsync()
        {
            string[] styleList = {
                "background: #ff0000",
                "height: 20px",
                "width: 20px"
            };

            foreach (var style in styleList)
            {
                string[] styleParts = style.Split(':');
                string jsCode = $@"
                    var marker = document.getElementsByClassName('marker')[0];
                    if (marker) {{
                        marker.style.setProperty('{styleParts[0].Trim()}', '{styleParts[1].Trim()}', 'important');
                    }} else {{
                        console.log('Marker not found');
                    }}";
                await jsExecutor.ExecuteScriptAsync(jsCode);
            }
        }

        private void chkAutoScreenshot_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoScreenshot)
            {
                if (string.IsNullOrEmpty(screenshotPath) || !Directory.Exists(screenshotPath))
                {
                    MessageBox.Show("올바르지 않은 경로입니다. 설정 페이지에서 경로를 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkAutoScreenshot = false;
                    appSettings.auto_screenshot_detection = false;
                    SaveSettings();
                    return;
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

            appSettings.auto_screenshot_detection = chkAutoScreenshot;
            SaveSettings();
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
            await jsExecutor.ClickButtonAsync("#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > div.mr-15 > button");
        }

        private async void btnFullScreen_Click(object sender, EventArgs e)
        {
            await jsExecutor.ClickButtonAsync("#__nuxt > div > div > div.page-content > div > div > div.panel_top > div > button");
        }

        private async void btnForceRun_Click(object sender, EventArgs e)
        {
            await CheckLocationAsync();
        }

        private void lblHowToUse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/blob/main/README.md");
        }

        private void lblBugReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/issues");
        }

        // Form1.cs에 넣고 싶었는데 예상 못한 오류가 발생하여 여기에 넣음
        // settings.json에 screenshot_path가 비어있을 때 경로 자동 탐색하는 로직(추후 교체 예정)
        private void CheckAndSetScreenshotPath()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrEmpty(homeDirectory))
            {
                MessageBox.Show("사용자 홈 디렉터리를 찾을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (appSettings.screenshot_paths_list == null || !appSettings.screenshot_paths_list.Any())
            {
                MessageBox.Show("스크린샷 경로 리스트가 비어 있습니다. 설정을 확인해주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool pathFound = false;

            foreach (string relativePath in appSettings.screenshot_paths_list)
            {
                string fullPath = Path.Combine(homeDirectory, relativePath);
                if (Directory.Exists(fullPath))
                {
                    appSettings.screenshot_path = fullPath;
                    pathFound = true;
                    screenshotPath = fullPath;
                    break;
                }
            }

            if (!pathFound)
            {
                MessageBox.Show("자동으로 경로를 찾는데 실패하였습니다. 설정 페이지에서 수동으로 경로를 지정해주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            try
            {
                SaveSettings(); // 설정 저장
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정을 저장하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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