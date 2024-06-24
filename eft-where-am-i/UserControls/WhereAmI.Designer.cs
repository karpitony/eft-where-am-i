namespace eft_where_am_i
{
    partial class WhereAmI
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkAutoScreenshot = new System.Windows.Forms.CheckBox();
            this.btnForceRun = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnHideShowPannel = new System.Windows.Forms.Button();
            this.btnFullScreen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSelectTheMap = new System.Windows.Forms.Label();
            this.btnMapApply = new System.Windows.Forms.Button();
            this.cmbMapSelect = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblBugReport = new System.Windows.Forms.LinkLabel();
            this.lblHowToUse = new System.Windows.Forms.LinkLabel();
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkAutoScreenshot);
            this.groupBox3.Controls.Add(this.btnForceRun);
            this.groupBox3.Location = new System.Drawing.Point(702, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(324, 104);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            // 
            // chkAutoScreenshot
            // 
            this.chkAutoScreenshot.AutoSize = true;
            this.chkAutoScreenshot.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.chkAutoScreenshot.Location = new System.Drawing.Point(17, 17);
            this.chkAutoScreenshot.Name = "chkAutoScreenshot";
            this.chkAutoScreenshot.Size = new System.Drawing.Size(301, 26);
            this.chkAutoScreenshot.TabIndex = 4;
            this.chkAutoScreenshot.Text = "Auto Screenshot Detection";
            this.chkAutoScreenshot.UseVisualStyleBackColor = true;
            this.chkAutoScreenshot.CheckedChanged += new System.EventHandler(this.chkAutoScreenshot_CheckedChanged);
            // 
            // btnForceRun
            // 
            this.btnForceRun.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnForceRun.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnForceRun.Location = new System.Drawing.Point(48, 57);
            this.btnForceRun.Name = "btnForceRun";
            this.btnForceRun.Size = new System.Drawing.Size(223, 42);
            this.btnForceRun.TabIndex = 10;
            this.btnForceRun.Text = "Force Run";
            this.btnForceRun.UseVisualStyleBackColor = true;
            this.btnForceRun.Click += new System.EventHandler(this.btnForceRun_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnHideShowPannel);
            this.groupBox2.Controls.Add(this.btnFullScreen);
            this.groupBox2.Location = new System.Drawing.Point(391, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 104);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            // 
            // btnHideShowPannel
            // 
            this.btnHideShowPannel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHideShowPannel.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnHideShowPannel.Location = new System.Drawing.Point(41, 9);
            this.btnHideShowPannel.Name = "btnHideShowPannel";
            this.btnHideShowPannel.Size = new System.Drawing.Size(223, 42);
            this.btnHideShowPannel.TabIndex = 8;
            this.btnHideShowPannel.Text = "Hide/Show Pannels";
            this.btnHideShowPannel.UseVisualStyleBackColor = true;
            this.btnHideShowPannel.Click += new System.EventHandler(this.btnHideShowPannel_Click);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFullScreen.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnFullScreen.Location = new System.Drawing.Point(41, 57);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(223, 42);
            this.btnFullScreen.TabIndex = 9;
            this.btnFullScreen.Text = "Full Screen";
            this.btnFullScreen.UseVisualStyleBackColor = true;
            this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblSelectTheMap);
            this.groupBox1.Controls.Add(this.btnMapApply);
            this.groupBox1.Controls.Add(this.cmbMapSelect);
            this.groupBox1.Location = new System.Drawing.Point(3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 104);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            // 
            // lblSelectTheMap
            // 
            this.lblSelectTheMap.AutoSize = true;
            this.lblSelectTheMap.Font = new System.Drawing.Font("굴림", 18F);
            this.lblSelectTheMap.Location = new System.Drawing.Point(91, 18);
            this.lblSelectTheMap.Name = "lblSelectTheMap";
            this.lblSelectTheMap.Size = new System.Drawing.Size(174, 24);
            this.lblSelectTheMap.TabIndex = 0;
            this.lblSelectTheMap.Text = "Select The Map";
            // 
            // btnMapApply
            // 
            this.btnMapApply.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnMapApply.Location = new System.Drawing.Point(243, 61);
            this.btnMapApply.Name = "btnMapApply";
            this.btnMapApply.Size = new System.Drawing.Size(129, 35);
            this.btnMapApply.TabIndex = 2;
            this.btnMapApply.Text = "Apply";
            this.btnMapApply.UseVisualStyleBackColor = true;
            this.btnMapApply.Click += new System.EventHandler(this.btnMapApply_Click);
            // 
            // cmbMapSelect
            // 
            this.cmbMapSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapSelect.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.cmbMapSelect.FormattingEnabled = true;
            this.cmbMapSelect.Location = new System.Drawing.Point(6, 64);
            this.cmbMapSelect.Name = "cmbMapSelect";
            this.cmbMapSelect.Size = new System.Drawing.Size(230, 29);
            this.cmbMapSelect.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblBugReport);
            this.groupBox4.Controls.Add(this.lblHowToUse);
            this.groupBox4.Location = new System.Drawing.Point(1036, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(266, 104);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            // 
            // lblBugReport
            // 
            this.lblBugReport.AutoSize = true;
            this.lblBugReport.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.lblBugReport.Location = new System.Drawing.Point(77, 67);
            this.lblBugReport.Name = "lblBugReport";
            this.lblBugReport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBugReport.Size = new System.Drawing.Size(122, 22);
            this.lblBugReport.TabIndex = 5;
            this.lblBugReport.TabStop = true;
            this.lblBugReport.Text = "Bug Report";
            this.lblBugReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBugReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblBugReport_LinkClicked);
            // 
            // lblHowToUse
            // 
            this.lblHowToUse.AutoSize = true;
            this.lblHowToUse.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.lblHowToUse.Location = new System.Drawing.Point(73, 19);
            this.lblHowToUse.Name = "lblHowToUse";
            this.lblHowToUse.Size = new System.Drawing.Size(129, 22);
            this.lblHowToUse.TabIndex = 3;
            this.lblHowToUse.TabStop = true;
            this.lblHowToUse.Text = "How to Use";
            this.lblHowToUse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHowToUse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHowtouse_LinkClicked);
            // 
            // webView2
            // 
            this.webView2.AllowExternalDrop = true;
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.webView2.Location = new System.Drawing.Point(0, 107);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(1309, 954);
            this.webView2.Source = new System.Uri("https://tarkov-market.com/maps/ground-zero", System.UriKind.Absolute);
            this.webView2.TabIndex = 17;
            this.webView2.ZoomFactor = 1D;
            // 
            // WhereAmI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.webView2);
            this.Name = "WhereAmI";
            this.Size = new System.Drawing.Size(1309, 1061);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkAutoScreenshot;
        private System.Windows.Forms.Button btnForceRun;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnHideShowPannel;
        private System.Windows.Forms.Button btnFullScreen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblSelectTheMap;
        private System.Windows.Forms.Button btnMapApply;
        private System.Windows.Forms.ComboBox cmbMapSelect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.LinkLabel lblBugReport;
        private System.Windows.Forms.LinkLabel lblHowToUse;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
    }
}
