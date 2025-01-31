using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using eft_where_am_i.Classes;
namespace eft_where_am_i
{
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

        public WhereAmI()
        {
            InitializeComponent();
            settingsHandler = new SettingsHandler(); // SettingsHandler 초기화
            jsExecutor = new JavaScriptExecutor(webView2); // WebView2 전달
            LoadSettings();
            InitializeMapComboBox();
            siteUrl = $"https://tarkov-market.com/maps/{appSettings.latest_map}";
            webView2.Source = new Uri(siteUrl);
            WmiFullScreen();
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

                chkAutoScreenshot.Checked = appSettings.auto_screenshot_detection;
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

        private void InitializeMapComboBox()
        {
            cmbMapSelect.Items.AddRange(mapList);
            cmbMapSelect.SelectedItem = appSettings.latest_map; // 기본 입력값 설정
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

        private void btnMapApply_Click(object sender, EventArgs e)
        {
            string selectedMap = cmbMapSelect.SelectedItem.ToString();
            if (selectedMap != null)
            {
                appSettings.latest_map = selectedMap;
                SaveSettings(); // latest_map 업데이트 후 설정 저장

                siteUrl = $"https://tarkov-market.com/maps/{selectedMap}";
                webView2.Source = new Uri(siteUrl);
                whereAmIClick = false;
                WmiFullScreen();
            }
        }

        private void chkAutoScreenshot_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoScreenshot.Checked)
            {
                if (string.IsNullOrEmpty(screenshotPath) || !Directory.Exists(screenshotPath))
                {
                    MessageBox.Show("올바르지 않은 경로입니다. 설정 페이지에서 경로를 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkAutoScreenshot.Checked = false;
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

            appSettings.auto_screenshot_detection = chkAutoScreenshot.Checked;
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
