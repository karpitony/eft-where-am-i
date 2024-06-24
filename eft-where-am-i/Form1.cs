using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using eft_where_am_i;
using Newtonsoft.Json;

namespace eft_where_am_i_chasrp
{
    public partial class Form1 : Form
    {
        private string currentScreen = "WhereAmI";
        private Settings appSettings = new Settings(); // Settings 객체 초기화
        private string settingsFile = @"assets\settings.json";

        public Form1()
        {
            InitializeComponent();
            LoadSettings();
            LoadUserControl();
            CheckAndSetScreenshotPath();
        }

        private void LoadUserControl()
        {
            SwitchUserControl(new WhereAmI());
        }

        const int MAX_SLIDING_WIDTH = 200;
        const int MIN_SLIDING_WIDTH = 75;
        const int STEP_SLIDING = 10;
        // 최초 슬라이딩 메뉴 크기
        int _posSliding = 75;

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                //슬라이딩 메뉴가 보였을 때, 메뉴 버튼의 표시
                btnSetting.Text = "Setting Page";
                btnSetting.Image = null;
                btnWhereAmI.Text = "Where Am I";
                btnWhereAmI.Image = null;
                checkBoxHide.Text = "<";
            }
            else
            {
                //슬라이딩 메뉴가 접혔을 때, 메뉴 버튼의 표시
                btnSetting.Text = "";
                btnSetting.Image = Image.FromFile(@"assets\settings_icon2_resize.png");
                btnWhereAmI.Text = "";
                btnWhereAmI.Image = Image.FromFile(@"assets\eft-where-am-i_icon_resize.png");
                checkBoxHide.Text = ">";
            }

            timerSliding.Start();
        }

        private void timerSliding_Tick(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                //슬라이딩 메뉴를 보이는 동작
                _posSliding += STEP_SLIDING;
                if (_posSliding >= MAX_SLIDING_WIDTH)
                    timerSliding.Stop();
            }
            else
            {
                //슬라이딩 메뉴를 숨기는 동작
                _posSliding -= STEP_SLIDING;
                if (_posSliding <= MIN_SLIDING_WIDTH)
                    timerSliding.Stop();
            }

            panelSideMenu.Width = _posSliding;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelSideMenu.Width = _posSliding;
            checkBoxHide.Checked = false; // 기본값으로 접혀있는 상태
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (currentScreen != "SettingPage")
            {
                SwitchUserControl(new SettingPage());
                currentScreen = "SettingPage";
            }
        }

        private void btnWhereAmI_Click(object sender, EventArgs e)
        {
            if (currentScreen != "WhereAmI")
            {
                SwitchUserControl(new WhereAmI());
                currentScreen = "WhereAmI";
            }
        }

        private void SwitchUserControl(UserControl newControl)
        {
            // 기존에 로드된 컨트롤 제거
            panel1.Controls.Clear();

            // 새로운 컨트롤 추가
            newControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(newControl);
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
                    }
                    else
                    {
                        MessageBox.Show("settings.json 파일이 비어 있습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        public class Settings
        {
            public bool isFirstRun { get; set; }
            public bool auto_screenshot_detection { get; set; }
            public string language { get; set; }
            public string screenshot_path { get; set; }
            public List<string> screenshot_paths_list { get; set; }
            public string latest_map { get; set; }

            public Settings()
            {
                isFirstRun = true;
                screenshot_paths_list = new List<string>();
            }
        }
    }
}