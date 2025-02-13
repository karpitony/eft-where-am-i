using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using eft_where_am_i.Classes;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace eft_where_am_i
{
    public partial class SettingPage : UserControl
    {
        private readonly SettingsHandler settingsHandler; // SettingsHandler 인스턴스
        private AppSettings appSettings; // AppSettings 참조

        public SettingPage()
        {
            InitializeComponent();
            settingsHandler = new SettingsHandler(); // SettingsHandler 초기화
            LoadSettings();
            InitializeWebViewUI();
        }

        private async void InitializeWebViewUI()
        {
            await webView2_Settings.EnsureCoreWebView2Async(null);

            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pages/settings.html");
            if (File.Exists(htmlPath))
            {
                webView2_Settings.Source = new Uri(htmlPath);
            }

            webView2_Settings.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            // HTML이 완전히 로드된 후 명령 전달
            webView2_Settings.NavigationCompleted += async (sender, args) =>
            {
                if (args.IsSuccess)
                {
                    try
                    {
                        await webView2_Settings.ExecuteScriptAsync($"setCheckboxState({appSettings.auto_screenshot_detection.ToString().ToLower()})");

                        // 언어 설정 전송
                        string language = appSettings.language;
                        await webView2_Settings.ExecuteScriptAsync($"setLanguage('{language}')");

                        // 스크린샷 경로 설정 전송
                        string screenshotPath = appSettings.screenshot_path.Replace("\\", "\\\\"); // JS에서 백슬래시 처리
                        await webView2_Settings.ExecuteScriptAsync($"setScreenshotPath('{screenshotPath}')");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"JavaScript 명령 전송 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
        }

        // WebView2 메시지 수신 핸들러
        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string rawMessage = e.WebMessageAsJson.Trim('"').Replace("\\\"", "\"");
                JObject message = JObject.Parse(rawMessage);

                // 안전하게 속성 접근
                string action = message["action"]?.ToString() ?? "";
                string language = message["language"]?.ToString() ?? "";
                string path = message["path"]?.ToString() ?? "";
                string url = message["url"]?.ToString() ?? "";

                // 메시지 처리
                switch (action.ToLower())
                {
                    case "language-updated":
                        if (!string.IsNullOrEmpty(language))
                        {
                            appSettings.language = language;
                            SaveSettings();  // 설정 저장
                            MessageBox.Show($"Language updated to: {language}", "Language Change", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case "change-path":
                        SelectScreenshotFolder();
                        break;

                    case "auto-detect-path":
                        string detectedPath = btnPathAutoFind_Click();
                        if (!string.IsNullOrEmpty(detectedPath))
                        {
                            appSettings.screenshot_path = detectedPath;
                            SaveSettings();
                            webView2_Settings.ExecuteScriptAsync($"setScreenshotPath('{detectedPath.Replace("\\", "\\\\")}')");
                            MessageBox.Show("Screenshot path auto-detected!", "Auto Detect", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to detect screenshot path.", "Auto Detect Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "open-folder":
                        if (!string.IsNullOrEmpty(appSettings.screenshot_path) && Directory.Exists(appSettings.screenshot_path))
                        {
                            Process.Start("explorer.exe", appSettings.screenshot_path);
                        }
                        else
                        {
                            MessageBox.Show("Invalid screenshot folder path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "link-clicked":
                        if (!string.IsNullOrEmpty(url))
                        {
                            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
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


        private void LoadSettings()
        {
            try
            {
                appSettings = settingsHandler.GetSettings(); // SettingsHandler에서 설정 로드
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 파일을 로드하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"설정 파일을 저장하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SelectScreenshotFolder()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "스크린샷 폴더를 선택하세요. Select the screenshot folder.";

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;

                    // 설정 업데이트 및 저장
                    appSettings.screenshot_path = selectedPath;
                    SaveSettings();

                    // JavaScript에 업데이트된 경로 전송
                    webView2_Settings.ExecuteScriptAsync($"setScreenshotPath('{selectedPath.Replace("\\", "\\\\")}')");
                }
            }
        }


        private string btnPathAutoFind_Click()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            foreach (string relativePath in appSettings.screenshot_paths_list)
            {
                string fullPath = Path.Combine(homeDirectory, relativePath);
                if (Directory.Exists(fullPath))
                {
                    appSettings.screenshot_path = fullPath; // AppSettings 업데이트
                    SaveSettings(); // 변경 사항 저장
                    return fullPath; // 첫번째 일치하는 경로만 사용
                }
            }

            return "자동 탐지중 오류가 발생했습니다. An error occurred during automatic detection.";
        }

        private void lblHowToUse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/blob/main/README.md");
        }

        private void lblBugReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/issues");
        }

        private void webView21_Click(object sender, EventArgs e)
        {

        }
    }
}