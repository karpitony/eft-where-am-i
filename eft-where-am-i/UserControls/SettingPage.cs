using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using eft_where_am_i.Classes;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Runtime.InteropServices;

namespace eft_where_am_i
{
    public partial class SettingPage : UserControl
    {
        private readonly SettingsHandler settingsHandler; // SettingsHandler 인스턴스
        private AppSettings appSettings; // AppSettings 참조

        public SettingPage()
        {
            InitializeComponent();
            settingsHandler = SettingsHandler.Instance;     // 싱글톤 인스턴스 사용
            settingsHandler.SettingsChanged += OnSettingsChanged;

            LoadSettings();
            // Load 이벤트 핸들러 등록
            this.Load += SettingPage_Load;
        }

        private async void SettingPage_Load(object sender, EventArgs e)
        {
            try
            {
                // 컨트롤이 로드된 후 비동기 초기화 시작
                await InitializeWebViewUI();
            }
            catch (Exception ex)
            {
                // InitializeWebViewUI에서 throw한 예외를 여기서 최종 처리
                MessageBox.Show($"설정 페이지 WebView 초기화 중 심각한 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSettingsChanged(AppSettings updatedSettings)
        {
            appSettings = updatedSettings; // 로컬 참조 업데이트
                                           // WebView나 UI 갱신 필요 시 호출
            _ = webView2_Settings.ExecuteScriptAsync($"setCheckboxState({appSettings.auto_screenshot_detection.ToString().ToLower()})");
            _ = webView2_Settings.ExecuteScriptAsync($"setLanguage('{appSettings.language}')");
            string escapedPath = appSettings.screenshot_path.Replace("\\", "\\\\");
            _ = webView2_Settings.ExecuteScriptAsync($"setScreenshotPath('{escapedPath}')");
            string escapedLogPath = appSettings.log_path.Replace("\\", "\\\\");
            _ = webView2_Settings.ExecuteScriptAsync($"setLogPath('{escapedLogPath}')");
        }

        private async Task InitializeWebViewUI()
        {
            // 고유한 사용자 데이터 폴더 생성 (임시 폴더 + GUID 사용)
            string userDataFolder = Path.Combine(Path.GetTempPath(), "MyAppWebView2_Settings", Guid.NewGuid().ToString());
            CoreWebView2Environment env = null;
            try
            {
                // 사용자 데이터 폴더를 지정하여 새로운 WebView2 환경 생성
                env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView2_Settings.EnsureCoreWebView2Async(env);

                // 가상 호스트 매핑 코드: 예를 들어, 번역 파일들이 저장된 폴더를 매핑
                string translationFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations");
                webView2_Settings.CoreWebView2.SetVirtualHostNameToFolderMapping(
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

            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "html/settings.html");
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

                        // 로그 경로 설정 전송
                        string logPath = appSettings.log_path.Replace("\\", "\\\\");
                        await webView2_Settings.ExecuteScriptAsync($"setLogPath('{logPath}')");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"JavaScript 명령 전송 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
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
                        try
                        {
                            string detectedPath = settingsHandler.ScreenshotPathSearch();
                            MessageBox.Show($"경로를 찾았습니다:\n{detectedPath}", "자동 탐지 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            // 3. 실패 시 예외 메시지 표시
                            // GetOrFindScreenshotPath가 throw한 예외 메시지를 그대로 보여줍니다.
                            MessageBox.Show(ex.Message, "자동 탐지 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    case "change-log-path":
                        SelectLogFolder();
                        break;

                    case "auto-detect-log-path":
                        try
                        {
                            string detectedLogPath = settingsHandler.LogPathSearch();
                            if (!string.IsNullOrEmpty(detectedLogPath))
                            {
                                MessageBox.Show($"경로를 찾았습니다:\n{detectedLogPath}", "자동 탐지 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("EFT 로그 폴더를 자동으로 탐지하지 못했습니다. 수동으로 지정해주세요.", "자동 탐지 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "자동 탐지 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public async void SelectScreenshotFolder()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "스크린샷 폴더를 선택하세요. Select the screenshot folder.";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;

                    // 설정 업데이트 및 저장
                    appSettings.screenshot_path = selectedPath;
                    SaveSettings();

                    // JavaScript에 업데이트된 경로 전송 (백슬래시 이스케이프)
                    string escapedPath = selectedPath.Replace("\\", "\\\\");
                    await webView2_Settings.ExecuteScriptAsync($"setScreenshotPath('{escapedPath}')");
                }
            }
        }

        public async void SelectLogFolder()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "EFT 로그 폴더를 선택하세요. Select the EFT logs folder.";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;

                    // 설정 업데이트 및 저장
                    appSettings.log_path = selectedPath;
                    SaveSettings();

                    // JavaScript에 업데이트된 경로 전송 (백슬래시 이스케이프)
                    string escapedPath = selectedPath.Replace("\\", "\\\\");
                    await webView2_Settings.ExecuteScriptAsync($"setLogPath('{escapedPath}')");
                }
            }
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