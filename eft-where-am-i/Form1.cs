using System;
using System.Drawing;
using System.Windows.Forms;
using eft_where_am_i;

namespace eft_where_am_i_chasrp
{
    public partial class Form1 : Form
    {
        private string currentScreen = "WhereAmI";

        public Form1()
        {
            InitializeComponent();
            LoadUserControl();
        }

        private void LoadUserControl()
        {
            // 처음 시작할 때 WhereAmI 컨트롤을 로드
            SwitchUserControl(new WhereAmI());
        }

        // 슬라이딩 메뉴의 최대, 최소 폭 크기
        const int MAX_SLIDING_WIDTH = 200;
        const int MIN_SLIDING_WIDTH = 75;
        // 슬라이딩 메뉴가 보이는/접히는 속도 조절
        const int STEP_SLIDING = 10;
        // 최초 슬라이딩 메뉴 크기
        int _posSliding = 75;

        private void checkBoxHide_CheckedChanged(object sender, EventArgs e)
        {
            // 기본은 슬라이딩 접혀있는 상태
            if (checkBoxHide.Checked == true)
            {
                //슬라이딩 메뉴가 보였을 때, 메뉴 버튼의 표시
                btnSetting.Text = "Settings";
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

            // 타이머 시작
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
            panelSideMenu.Width = _posSliding; // 초기 패널 크기 설정
            checkBoxHide.Checked = false; // 기본값으로 접혀있는 상태
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (currentScreen != "Settings")
            {
                SwitchUserControl(new Settings());
                currentScreen = "Settings";
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
    }
}
