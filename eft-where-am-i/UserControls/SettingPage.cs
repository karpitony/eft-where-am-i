using System;
using System.IO;
using System.Windows.Forms;
using eft_where_am_i.Classes;

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
        }

        private void LoadSettings()
        {
            try
            {
                appSettings = settingsHandler.GetSettings(); // SettingsHandler에서 설정 로드
                txtPath.Text = appSettings.screenshot_path; // 텍스트박스에 현재 경로 표시
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
                    appSettings.screenshot_path = selectedPath; // AppSettings 업데이트
                    SaveSettings(); // 변경 사항 저장
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
                    appSettings.screenshot_path = fullPath; // AppSettings 업데이트
                    SaveSettings(); // 변경 사항 저장
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
}