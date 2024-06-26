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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panelSideMenu = new System.Windows.Forms.Panel();
            this.btnWhereAmI = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.checkBoxHide = new System.Windows.Forms.CheckBox();
            this.timerSliding = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelSideMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelSideMenu.Controls.Add(this.btnWhereAmI);
            this.panelSideMenu.Controls.Add(this.btnSetting);
            this.panelSideMenu.Controls.Add(this.checkBoxHide);
            this.panelSideMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.panelSideMenu.Name = "panelSideMenu";
            this.panelSideMenu.Size = new System.Drawing.Size(75, 1061);
            this.panelSideMenu.TabIndex = 12;
            // 
            // btnWhereAmI
            // 
            this.btnWhereAmI.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnWhereAmI.FlatAppearance.BorderSize = 0;
            this.btnWhereAmI.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnWhereAmI.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnWhereAmI.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWhereAmI.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold);
            this.btnWhereAmI.ForeColor = System.Drawing.Color.White;
            this.btnWhereAmI.Image = ((System.Drawing.Image)(resources.GetObject("btnWhereAmI.Image")));
            this.btnWhereAmI.Location = new System.Drawing.Point(0, 75);
            this.btnWhereAmI.Name = "btnWhereAmI";
            this.btnWhereAmI.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnWhereAmI.Size = new System.Drawing.Size(75, 75);
            this.btnWhereAmI.TabIndex = 2;
            this.btnWhereAmI.UseVisualStyleBackColor = true;
            this.btnWhereAmI.Click += new System.EventHandler(this.btnWhereAmI_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSetting.FlatAppearance.BorderSize = 0;
            this.btnSetting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnSetting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetting.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold);
            this.btnSetting.ForeColor = System.Drawing.Color.White;
            this.btnSetting.Image = ((System.Drawing.Image)(resources.GetObject("btnSetting.Image")));
            this.btnSetting.Location = new System.Drawing.Point(0, 0);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(75, 75);
            this.btnSetting.TabIndex = 1;
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // checkBoxHide
            // 
            this.checkBoxHide.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxHide.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkBoxHide.FlatAppearance.BorderSize = 0;
            this.checkBoxHide.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.checkBoxHide.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.checkBoxHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxHide.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold);
            this.checkBoxHide.ForeColor = System.Drawing.Color.White;
            this.checkBoxHide.Location = new System.Drawing.Point(0, 986);
            this.checkBoxHide.Name = "checkBoxHide";
            this.checkBoxHide.Size = new System.Drawing.Size(75, 75);
            this.checkBoxHide.TabIndex = 0;
            this.checkBoxHide.Text = ">";
            this.checkBoxHide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxHide.UseVisualStyleBackColor = true;
            this.checkBoxHide.CheckedChanged += new System.EventHandler(this.checkBoxHide_CheckedChanged);
            // 
            // timerSliding
            // 
            this.timerSliding.Interval = 5;
            this.timerSliding.Tick += new System.EventHandler(this.timerSliding_Tick);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(75, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1309, 1061);
            this.panel1.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1384, 1061);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelSideMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "EFT Where am I? (v2.0)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelSideMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelSideMenu;
        private System.Windows.Forms.CheckBox checkBoxHide;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnWhereAmI;
        private System.Windows.Forms.Timer timerSliding;
        private System.Windows.Forms.Panel panel1;
    }
}

