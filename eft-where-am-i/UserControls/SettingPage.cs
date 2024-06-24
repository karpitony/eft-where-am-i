using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace eft_where_am_i
{
    public partial class SettingPage : UserControl
    {
        private string settingsFile = @"assets\settings.json";
        private AppSettings appSettings = new AppSettings(); // Settings 객체 초기화

        public SettingPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    string json = File.ReadAllText(settingsFile);
                    appSettings = JsonConvert.DeserializeObject<AppSettings>(json);

                    // 텍스트박스에 현재 경로 표시
                    txtPath.Text = appSettings.screenshot_path;
                }
                else
                {
                    MessageBox.Show("settings.json 파일을 찾을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                string json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
                File.WriteAllText(settingsFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 파일을 저장하는 동안 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLanguageSelect_Click(object sender, EventArgs e)
        {

        }

        private void btnPathChange_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "스크린샷 경로를 선택하세요.";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    txtPath.Text = selectedPath;
                    appSettings.screenshot_path = selectedPath;
                    SaveSettings(); // 변경된 경로만 저장
                }
            }
        }

        private void btnPathAutoFind_Click(object sender, EventArgs e)
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            foreach (string relativePath in appSettings.screenshot_paths_list)
            {
                string fullPath = Path.Combine(homeDirectory, relativePath);
                if (Directory.Exists(fullPath))
                {
                    appSettings.screenshot_path = fullPath;
                    SaveSettings(); // 변경된 경로만 저장
                    txtPath.Text = appSettings.screenshot_path;
                    break; // 첫 번째 일치하는 경로만 사용
                }
            }
        }

        private void chkAutoScreenshotCleaner_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lblHowToUse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/blob/main/README.md");
        }

        private void lblBugReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/karpitony/eft-where-am-i/issues");
        }
    }

    public class AppSettings
    {
        public bool isFirstRun { get; set; }
        public bool auto_screenshot_detection { get; set; }
        public string language { get; set; }
        public string screenshot_path { get; set; }
        public List<string> screenshot_paths_list { get; set; }
        public string latest_map { get; set; }
    }
}
