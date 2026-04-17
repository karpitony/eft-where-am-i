namespace eft_where_am_i_chasrp
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panelSideMenu = new Panel();
            btnWhereAmI = new Button();
            btnSetting = new Button();
            btnServerLocation = new Button();
            checkBoxHide = new CheckBox();
            timerSliding = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            panelSideMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panelSideMenu
            // 
            panelSideMenu.BackColor = Color.FromArgb(64, 64, 64);
            panelSideMenu.Controls.Add(btnServerLocation);
            panelSideMenu.Controls.Add(btnWhereAmI);
            panelSideMenu.Controls.Add(btnSetting);
            panelSideMenu.Controls.Add(checkBoxHide);
            panelSideMenu.Dock = DockStyle.Left;
            panelSideMenu.Location = new Point(0, 0);
            panelSideMenu.Margin = new Padding(3, 4, 3, 4);
            panelSideMenu.Name = "panelSideMenu";
            panelSideMenu.Size = new Size(75, 1022);
            panelSideMenu.TabIndex = 12;
            // 
            // btnWhereAmI
            // 
            btnWhereAmI.Dock = DockStyle.Top;
            btnWhereAmI.FlatAppearance.BorderSize = 0;
            btnWhereAmI.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 60, 60);
            btnWhereAmI.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            btnWhereAmI.FlatStyle = FlatStyle.Flat;
            btnWhereAmI.Font = new Font("굴림", 15F, FontStyle.Bold);
            btnWhereAmI.ForeColor = Color.White;
            btnWhereAmI.Image = (Image)resources.GetObject("btnWhereAmI.Image");
            btnWhereAmI.Location = new Point(0, 94);
            btnWhereAmI.Margin = new Padding(3, 4, 3, 4);
            btnWhereAmI.Name = "btnWhereAmI";
            btnWhereAmI.RightToLeft = RightToLeft.No;
            btnWhereAmI.Size = new Size(75, 94);
            btnWhereAmI.TabIndex = 2;
            btnWhereAmI.UseVisualStyleBackColor = true;
            btnWhereAmI.Click += btnWhereAmI_Click;
            // 
            // btnSetting
            // 
            btnSetting.Dock = DockStyle.Top;
            btnSetting.FlatAppearance.BorderSize = 0;
            btnSetting.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 60, 60);
            btnSetting.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            btnSetting.FlatStyle = FlatStyle.Flat;
            btnSetting.Font = new Font("굴림", 15F, FontStyle.Bold);
            btnSetting.ForeColor = Color.White;
            btnSetting.Image = (Image)resources.GetObject("btnSetting.Image");
            btnSetting.Location = new Point(0, 0);
            btnSetting.Margin = new Padding(3, 4, 3, 4);
            btnSetting.Name = "btnSetting";
            btnSetting.Size = new Size(75, 94);
            btnSetting.TabIndex = 1;
            btnSetting.UseVisualStyleBackColor = true;
            btnSetting.Click += btnSetting_Click;
            // 
            // btnServerLocation
            // 
            btnServerLocation.Dock = DockStyle.Top;
            btnServerLocation.FlatAppearance.BorderSize = 0;
            btnServerLocation.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 60, 60);
            btnServerLocation.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            btnServerLocation.FlatStyle = FlatStyle.Flat;
            btnServerLocation.Font = new Font("굴림", 15F, FontStyle.Bold);
            btnServerLocation.ForeColor = Color.White;
            btnServerLocation.Image = Image.FromFile(@"assets\images\server.png");
            btnServerLocation.Location = new Point(0, 188);
            btnServerLocation.Margin = new Padding(3, 4, 3, 4);
            btnServerLocation.Name = "btnServerLocation";
            btnServerLocation.Size = new Size(75, 94);
            btnServerLocation.TabIndex = 3;
            btnServerLocation.UseVisualStyleBackColor = true;
            btnServerLocation.Click += btnServerLocation_Click;
            // 
            // checkBoxHide
            // 
            checkBoxHide.Appearance = Appearance.Button;
            checkBoxHide.Dock = DockStyle.Bottom;
            checkBoxHide.FlatAppearance.BorderSize = 0;
            checkBoxHide.FlatAppearance.MouseDownBackColor = Color.FromArgb(60, 60, 60);
            checkBoxHide.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 70, 70);
            checkBoxHide.FlatStyle = FlatStyle.Flat;
            checkBoxHide.Font = new Font("굴림", 15F, FontStyle.Bold);
            checkBoxHide.ForeColor = Color.White;
            checkBoxHide.Location = new Point(0, 928);
            checkBoxHide.Margin = new Padding(3, 4, 3, 4);
            checkBoxHide.Name = "checkBoxHide";
            checkBoxHide.Size = new Size(75, 94);
            checkBoxHide.TabIndex = 0;
            checkBoxHide.Text = ">";
            checkBoxHide.TextAlign = ContentAlignment.MiddleCenter;
            checkBoxHide.UseVisualStyleBackColor = true;
            checkBoxHide.CheckedChanged += checkBoxHide_CheckedChanged;
            // 
            // timerSliding
            // 
            timerSliding.Interval = 5;
            timerSliding.Tick += timerSliding_Tick;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(75, 0);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1293, 1022);
            panel1.TabIndex = 13;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(1368, 1022);
            Controls.Add(panel1);
            Controls.Add(panelSideMenu);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "EFT Where am I? (v2.2.3)";
            Load += Form1_Load;
            panelSideMenu.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelSideMenu;
        private System.Windows.Forms.CheckBox checkBoxHide;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnWhereAmI;
        private System.Windows.Forms.Button btnServerLocation;
        private System.Windows.Forms.Timer timerSliding;
        private System.Windows.Forms.Panel panel1;
    }
}
