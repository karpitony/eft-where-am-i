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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblSelectTheMap = new System.Windows.Forms.Label();
            this.cmbMapSelect = new System.Windows.Forms.ComboBox();
            this.btnMapApply = new System.Windows.Forms.Button();
            this.lblHowToUse = new System.Windows.Forms.LinkLabel();
            this.cktAutoScreenshot = new System.Windows.Forms.CheckBox();
            this.lblBugReport = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLaguageApply = new System.Windows.Forms.Button();
            this.cmbLanguageSelect = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHideShowPannel = new System.Windows.Forms.Button();
            this.btnFullScreen = new System.Windows.Forms.Button();
            this.btnForceRun = new System.Windows.Forms.Button();
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSelectTheMap
            // 
            this.lblSelectTheMap.AutoSize = true;
            this.lblSelectTheMap.Font = new System.Drawing.Font("굴림", 18F);
            this.lblSelectTheMap.Location = new System.Drawing.Point(89, 11);
            this.lblSelectTheMap.Name = "lblSelectTheMap";
            this.lblSelectTheMap.Size = new System.Drawing.Size(174, 24);
            this.lblSelectTheMap.TabIndex = 0;
            this.lblSelectTheMap.Text = "Select The Map";
            // 
            // cmbMapSelect
            // 
            this.cmbMapSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMapSelect.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.cmbMapSelect.FormattingEnabled = true;
            this.cmbMapSelect.Location = new System.Drawing.Point(3, 50);
            this.cmbMapSelect.Name = "cmbMapSelect";
            this.cmbMapSelect.Size = new System.Drawing.Size(214, 29);
            this.cmbMapSelect.TabIndex = 1;
            // 
            // btnMapApply
            // 
            this.btnMapApply.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnMapApply.Location = new System.Drawing.Point(223, 47);
            this.btnMapApply.Name = "btnMapApply";
            this.btnMapApply.Size = new System.Drawing.Size(129, 35);
            this.btnMapApply.TabIndex = 2;
            this.btnMapApply.Text = "Apply";
            this.btnMapApply.UseVisualStyleBackColor = true;
            this.btnMapApply.Click += new System.EventHandler(this.btnMapApply_Click);
            // 
            // lblHowToUse
            // 
            this.lblHowToUse.AutoSize = true;
            this.lblHowToUse.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.lblHowToUse.Location = new System.Drawing.Point(61, 59);
            this.lblHowToUse.Name = "lblHowToUse";
            this.lblHowToUse.Size = new System.Drawing.Size(129, 22);
            this.lblHowToUse.TabIndex = 3;
            this.lblHowToUse.TabStop = true;
            this.lblHowToUse.Text = "How to Use";
            this.lblHowToUse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHowToUse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHowtouse_LinkClicked);
            // 
            // cktAutoScreenshot
            // 
            this.cktAutoScreenshot.AutoSize = true;
            this.cktAutoScreenshot.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.cktAutoScreenshot.Location = new System.Drawing.Point(24, 94);
            this.cktAutoScreenshot.Name = "cktAutoScreenshot";
            this.cktAutoScreenshot.Size = new System.Drawing.Size(301, 26);
            this.cktAutoScreenshot.TabIndex = 4;
            this.cktAutoScreenshot.Text = "Auto Screenshot Detection";
            this.cktAutoScreenshot.UseVisualStyleBackColor = true;
            this.cktAutoScreenshot.CheckedChanged += new System.EventHandler(this.chkAutoScreenshot_CheckedChanged);
            // 
            // lblBugReport
            // 
            this.lblBugReport.AutoSize = true;
            this.lblBugReport.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.lblBugReport.Location = new System.Drawing.Point(64, 95);
            this.lblBugReport.Name = "lblBugReport";
            this.lblBugReport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBugReport.Size = new System.Drawing.Size(122, 22);
            this.lblBugReport.TabIndex = 5;
            this.lblBugReport.TabStop = true;
            this.lblBugReport.Text = "Bug Report";
            this.lblBugReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBugReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblBugReport_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnLaguageApply);
            this.panel1.Controls.Add(this.cmbLanguageSelect);
            this.panel1.Controls.Add(this.lblHowToUse);
            this.panel1.Controls.Add(this.lblBugReport);
            this.panel1.Location = new System.Drawing.Point(913, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 129);
            this.panel1.TabIndex = 6;
            // 
            // btnLaguageApply
            // 
            this.btnLaguageApply.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnLaguageApply.Location = new System.Drawing.Point(158, 16);
            this.btnLaguageApply.Name = "btnLaguageApply";
            this.btnLaguageApply.Size = new System.Drawing.Size(101, 29);
            this.btnLaguageApply.TabIndex = 7;
            this.btnLaguageApply.Text = "Apply";
            this.btnLaguageApply.UseVisualStyleBackColor = true;
            this.btnLaguageApply.Click += new System.EventHandler(this.btnLaguageApply_Click);
            // 
            // cmbLanguageSelect
            // 
            this.cmbLanguageSelect.AllowDrop = true;
            this.cmbLanguageSelect.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.cmbLanguageSelect.FormattingEnabled = true;
            this.cmbLanguageSelect.Location = new System.Drawing.Point(3, 16);
            this.cmbLanguageSelect.Name = "cmbLanguageSelect";
            this.cmbLanguageSelect.Size = new System.Drawing.Size(149, 29);
            this.cmbLanguageSelect.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblSelectTheMap);
            this.panel2.Controls.Add(this.btnMapApply);
            this.panel2.Controls.Add(this.cktAutoScreenshot);
            this.panel2.Controls.Add(this.cmbMapSelect);
            this.panel2.Location = new System.Drawing.Point(12, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(355, 129);
            this.panel2.TabIndex = 7;
            // 
            // btnHideShowPannel
            // 
            this.btnHideShowPannel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHideShowPannel.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnHideShowPannel.Location = new System.Drawing.Point(373, 3);
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
            this.btnFullScreen.Location = new System.Drawing.Point(373, 45);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(223, 42);
            this.btnFullScreen.TabIndex = 9;
            this.btnFullScreen.Text = "Full Screen";
            this.btnFullScreen.UseVisualStyleBackColor = true;
            this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
            // 
            // btnForceRun
            // 
            this.btnForceRun.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnForceRun.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.btnForceRun.Location = new System.Drawing.Point(373, 87);
            this.btnForceRun.Name = "btnForceRun";
            this.btnForceRun.Size = new System.Drawing.Size(223, 42);
            this.btnForceRun.TabIndex = 10;
            this.btnForceRun.Text = "Force Run";
            this.btnForceRun.UseVisualStyleBackColor = true;
            this.btnForceRun.Click += new System.EventHandler(this.btnForceRun_Click);
            // 
            // webView2
            // 
            this.webView2.AllowExternalDrop = true;
            this.webView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Location = new System.Drawing.Point(12, 135);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(1160, 814);
            this.webView2.Source = new System.Uri("https://tarkov-market.com/maps/ground-zero", System.UriKind.Absolute);
            this.webView2.TabIndex = 11;
            this.webView2.ZoomFactor = 1D;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 961);
            this.Controls.Add(this.webView2);
            this.Controls.Add(this.btnForceRun);
            this.Controls.Add(this.btnFullScreen);
            this.Controls.Add(this.btnHideShowPannel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "EFT Where am I? (v2.0)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblSelectTheMap;
        private System.Windows.Forms.ComboBox cmbMapSelect;
        private System.Windows.Forms.Button btnMapApply;
        private System.Windows.Forms.LinkLabel lblHowToUse;
        private System.Windows.Forms.CheckBox cktAutoScreenshot;
        private System.Windows.Forms.LinkLabel lblBugReport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnHideShowPannel;
        private System.Windows.Forms.Button btnFullScreen;
        private System.Windows.Forms.Button btnForceRun;
        private System.Windows.Forms.Button btnLaguageApply;
        private System.Windows.Forms.ComboBox cmbLanguageSelect;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
    }
}

