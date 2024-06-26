using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Web.WebView2.Core;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data;

namespace eft_where_am_i
{
    public partial class WhereAmI : UserControl
    {
        private string[] mapList = { "ground-zero", "factory", "customs", "interchange", "woods", "shoreline", "lighthouse", "reserve", "streets", "lab" };
        private string siteUrl;
        private bool whereAmIClick = false;
        private string settingsFile = @"assets\settings.json";
        private Settings appSettings = new Settings();
        private string screenshotPath;
        private FileSystemWatcher watcher;

        public WhereAmI()
        {
            InitializeComponent();
            LoadSettings();
            InitializeMapComboBox();
            siteUrl = $"https://tarkov-market.com/maps/{appSettings.latest_map}";
            webView2.Source = new Uri(siteUrl);
            WmiFullScreen();
        }

        private async void WmiFullScreen()
        {
            await Task.Delay(3000);

            string jsCode =
                "var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > button');\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Fullscreen button clicked');\n" +
                "} else {\n" +
                "    console.log('Fullscreen button not found');\n" +
                "}";
            await ExecuteJavaScriptAsync(jsCode);
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

            var latestFile = files.First();
            return latestFile.Name;
        }

        private async Task ExecuteJavaScriptAsync(string script)
        {
            try
            {
                await webView2.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"JavaScript 실행 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CheckLocationAsync()
        {
            string screenshot = GetLatestFile();
            if (screenshot == null)
                return;

            if (!whereAmIClick)
            {
                whereAmIClick = true;
                string jsCodeClick =
                    "var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > div.d-flex.ml-15.fs-0 > button');\n" +
                    "if (button) {\n" +
                    "    button.click();\n" +
                    "    console.log('Button clicked');\n" +
                    "} else {\n" +
                    "    console.log('Button not found');\n" +
                    "}";
                await ExecuteJavaScriptAsync(jsCodeClick);
                await Task.Delay(500);
            }

            string jsInputCode =
                "var input = document.querySelector('input[type=\"text\"]');\n" +
                "if (input) {\n" +
                "    input.value = '" + screenshot.Replace(".png", "") + "';\n" +
                "    input.dispatchEvent(new Event('input'));\n" +
                "    console.log('Input value set');\n" +
                "} else {\n" +
                "    console.log('Input not found');\n" +
                "}";
            await ExecuteJavaScriptAsync(jsInputCode);
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
                string jsCode =
                    "var marker = document.getElementsByClassName('marker')[0];\n" +
                    "if (marker) {\n" +
                    "    marker.style.setProperty('" + style.Split(':')[0].Trim() + "', '" + style.Split(':')[1].Trim() + "', 'important');\n" +
                    "} else {\n" +
                    "    console.log('Marker not found');\n" +
                    "}";
                await ExecuteJavaScriptAsync(jsCode);
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
            }
        }

        private void chkAutoScreenshot_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoScreenshot.Checked)
            {
                // 경로가 올바르지 않은 경우 경고 메시지 표시 및 체크박스를 해제
                screenshotPath = appSettings.screenshot_path;
                if (string.IsNullOrEmpty(screenshotPath) || !Directory.Exists(screenshotPath))
                {
                    MessageBox.Show("올바르지 않은 경로입니다. 설정 페이지에서 경로를 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    chkAutoScreenshot.Checked = false;
                    appSettings.auto_screenshot_detection = false;
                    SaveSettings();
                    return; // 경로가 잘못된 경우 이후 코드를 실행하지 않음
                }
                else
                {
                    watcher = new FileSystemWatcher();
                    watcher.Path = screenshotPath;
                    watcher.Filter = "*.png";
                    watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                    watcher.Created += OnScreenshotCreated;
                    watcher.EnableRaisingEvents = true;
                }
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

            // 체크 상태를 저장
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
            string jsCode =
                "var button = document.evaluate('//*[@id=\"__nuxt\"]/div/div/div[2]/div/div/div[1]/div[1]/button', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Panel control button clicked');\n" +
                "} else {\n" +
                "    console.log('Panel control button not found');\n" +
                "}";
            await webView2.CoreWebView2.ExecuteScriptAsync(jsCode);
        }

        private async void btnFullScreen_Click(object sender, EventArgs e)
        {
            string jsCode =
                "var button = document.querySelector('#__nuxt > div > div > div.page-content > div > div > div.panel_top.d-flex > button');\n" +
                "if (button) {\n" +
                "    button.click();\n" +
                "    console.log('Fullscreen button clicked');\n" +
                "} else {\n" +
                "    console.log('Fullscreen button not found');\n" +
                "}";
            await webView2.CoreWebView2.ExecuteScriptAsync(jsCode);
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

        // Settings class to match the JSON structure
        public class Settings
        {
            public bool isFirstRun { get; set; }
            public bool auto_screenshot_detection { get; set; }
            public string language { get; set; }
            public string screenshot_path { get; set; }
            public List<string> screenshot_paths_list { get; set; }
            public string latest_map { get; set; }
        }
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    string json = File.ReadAllText(settingsFile);

                    if (!string.IsNullOrEmpty(json))
                    {
                        appSettings = JsonConvert.DeserializeObject<Settings>(json);

                        if (appSettings == null)
                        {
                            appSettings = new Settings();
                        }

                        if (string.IsNullOrEmpty(appSettings.latest_map) || !mapList.Contains(appSettings.latest_map))
                        {
                            appSettings.latest_map = "ground-zero";
                            SaveSettings();
                        }

                        chkAutoScreenshot.Checked = appSettings.auto_screenshot_detection;
                    }
                    else
                    {
                        MessageBox.Show("settings.json 파일이 비어 있습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    MessageBox.Show("settings.json 파일을 찾을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                
                if (appSettings.isFirstRun == true)
                {
                    CheckAndSetScreenshotPath();
                    screenshotPath = appSettings.screenshot_path;
                }
                else
                {
                    if (string.IsNullOrEmpty(appSettings.screenshot_path) || !Directory.Exists(appSettings.screenshot_path))
                    {
                        MessageBox.Show("올바르지 않은 경로입니다. 설정 페이지에서 경로를 확인해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        screenshotPath = appSettings.screenshot_path;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 파일을 로드하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
        private void SaveSettings()
        {
            string json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
            File.WriteAllText(settingsFile, json);
        }

        // Form1.cs에 넣고 싶었는데 예상 못한 오류가 발생하여 여기에 넣음
        private void CheckAndSetScreenshotPath()
        {
            if (appSettings.isFirstRun)
            {
                string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                bool pathFound = false;

                foreach (string relativePath in appSettings.screenshot_paths_list)
                {
                    string fullPath = Path.Combine(homeDirectory, relativePath);
                    if (Directory.Exists(fullPath))
                    {
                        appSettings.screenshot_path = fullPath;
                        pathFound = true;
                        break;
                    }
                }

                if (!pathFound)
                {
                    MessageBox.Show("자동으로 경로를 찾는데 실패하였습니다. 설정 페이지에서 수동으로 경로를 지정해주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                appSettings.isFirstRun = false;
                SaveSettings();
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
