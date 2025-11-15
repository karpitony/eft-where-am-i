using System;
using System.Drawing;
using System.Windows.Forms;
using eft_where_am_i;

namespace eft_where_am_i_chasrp
{
    public partial class Form1 : Form
    {
        private string currentScreen = "WhereAmI";

        // UserControl을 캐싱해서 상태 유지
        private WhereAmI whereAmIControl;
        private SettingPage settingPageControl;

        public Form1()
        {
            InitializeComponent();

            // 깜빡임 방지 옵션
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            InitializeScreens();
        }

        private void InitializeScreens()
        {
            // 화면 2개 생성 (딱 1번만)
            whereAmIControl = new WhereAmI();
            settingPageControl = new SettingPage();

            // Panel에 미리 추가
            panel1.Controls.Add(whereAmIControl);
            panel1.Controls.Add(settingPageControl);

            whereAmIControl.Dock = DockStyle.Fill;
            settingPageControl.Dock = DockStyle.Fill;

            // 시작 화면 설정
            whereAmIControl.Visible = true;
            settingPageControl.Visible = false;
        }

        const int MAX_SLIDING_WIDTH = 200;
        const int MIN_SLIDING_WIDTH = 75;
        const int STEP_SLIDING = 10;
        int _posSliding = 75;

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                btnSetting.Text = "Setting Page";
                btnSetting.Image = null;
                btnWhereAmI.Text = "Where Am I";
                btnWhereAmI.Image = null;
                checkBoxHide.Text = "<";
            }
            else
            {
                btnSetting.Text = "";
                btnSetting.Image = Image.FromFile(@"assets\images\settings_icon2_resize.png");
                btnWhereAmI.Text = "";
                btnWhereAmI.Image = Image.FromFile(@"assets\images\eft-where-am-i_icon_resize.png");
                checkBoxHide.Text = ">";
            }

            timerSliding.Start();
        }

        private void timerSliding_Tick(object sender, EventArgs e)
        {
            if (checkBoxHide.Checked == true)
            {
                _posSliding += STEP_SLIDING;
                if (_posSliding >= MAX_SLIDING_WIDTH)
                    timerSliding.Stop();
            }
            else
            {
                _posSliding -= STEP_SLIDING;
                if (_posSliding <= MIN_SLIDING_WIDTH)
                    timerSliding.Stop();
            }

            panelSideMenu.Width = _posSliding;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelSideMenu.Width = _posSliding;
            checkBoxHide.Checked = false;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (currentScreen != "SettingPage")
            {
                SwitchUserControl(settingPageControl);
                currentScreen = "SettingPage";
            }
        }

        private void btnWhereAmI_Click(object sender, EventArgs e)
        {
            if (currentScreen != "WhereAmI")
            {
                SwitchUserControl(whereAmIControl);
                currentScreen = "WhereAmI";
            }
        }

        private void SwitchUserControl(UserControl control)
        {
            // 모든 화면 숨기기
            whereAmIControl.Visible = false;
            settingPageControl.Visible = false;

            // 새 화면만 보이기
            control.Visible = true;
        }
    }

    // 초기 경로 설정은 WhereAmI.cs에서 처리
}