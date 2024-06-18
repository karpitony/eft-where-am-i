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
            this.SelectTheMap_Lable = new System.Windows.Forms.Label();
            this.Map_ComboBox = new System.Windows.Forms.ComboBox();
            this.MapApply_Button = new System.Windows.Forms.Button();
            this.HowtouseLinkLabel = new System.Windows.Forms.LinkLabel();
            this.AutoScreenshot_CheckBox = new System.Windows.Forms.CheckBox();
            this.BugReport_LinkLabel = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LaguageApply_Button = new System.Windows.Forms.Button();
            this.LanguageSelect_Combobox = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.HideShowPannel_Button = new System.Windows.Forms.Button();
            this.FullScreen_Button = new System.Windows.Forms.Button();
            this.ForceRun_Button = new System.Windows.Forms.Button();
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.SuspendLayout();
            // 
            // SelectTheMap_Lable
            // 
            this.SelectTheMap_Lable.AutoSize = true;
            this.SelectTheMap_Lable.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.SelectTheMap_Lable.Location = new System.Drawing.Point(91, 6);
            this.SelectTheMap_Lable.Name = "SelectTheMap_Lable";
            this.SelectTheMap_Lable.Size = new System.Drawing.Size(166, 22);
            this.SelectTheMap_Lable.TabIndex = 0;
            this.SelectTheMap_Lable.Text = "Select The Map";
            this.SelectTheMap_Lable.Click += new System.EventHandler(this.SelectTheMap_Click);
            // 
            // Map_ComboBox
            // 
            this.Map_ComboBox.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.Map_ComboBox.FormattingEnabled = true;
            this.Map_ComboBox.Location = new System.Drawing.Point(3, 45);
            this.Map_ComboBox.Name = "Map_ComboBox";
            this.Map_ComboBox.Size = new System.Drawing.Size(198, 29);
            this.Map_ComboBox.TabIndex = 1;
            this.Map_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Map_ComboBox_SelectedIndexChanged);
            // 
            // MapApply_Button
            // 
            this.MapApply_Button.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.MapApply_Button.Location = new System.Drawing.Point(207, 41);
            this.MapApply_Button.Name = "MapApply_Button";
            this.MapApply_Button.Size = new System.Drawing.Size(129, 35);
            this.MapApply_Button.TabIndex = 2;
            this.MapApply_Button.Text = "Apply";
            this.MapApply_Button.UseVisualStyleBackColor = true;
            this.MapApply_Button.Click += new System.EventHandler(this.MapApply_Button_Click);
            // 
            // HowtouseLinkLabel
            // 
            this.HowtouseLinkLabel.AutoSize = true;
            this.HowtouseLinkLabel.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.HowtouseLinkLabel.Location = new System.Drawing.Point(60, 64);
            this.HowtouseLinkLabel.Name = "HowtouseLinkLabel";
            this.HowtouseLinkLabel.Size = new System.Drawing.Size(129, 22);
            this.HowtouseLinkLabel.TabIndex = 3;
            this.HowtouseLinkLabel.TabStop = true;
            this.HowtouseLinkLabel.Text = "How to Use";
            this.HowtouseLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HowtouseLinkLabel_LinkClicked);
            // 
            // AutoScreenshot_CheckBox
            // 
            this.AutoScreenshot_CheckBox.AutoSize = true;
            this.AutoScreenshot_CheckBox.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.AutoScreenshot_CheckBox.Location = new System.Drawing.Point(26, 86);
            this.AutoScreenshot_CheckBox.Name = "AutoScreenshot_CheckBox";
            this.AutoScreenshot_CheckBox.Size = new System.Drawing.Size(301, 26);
            this.AutoScreenshot_CheckBox.TabIndex = 4;
            this.AutoScreenshot_CheckBox.Text = "Auto Screenshot Detection";
            this.AutoScreenshot_CheckBox.UseVisualStyleBackColor = true;
            this.AutoScreenshot_CheckBox.CheckedChanged += new System.EventHandler(this.AutoScreenshot_CheckBox_CheckedChanged);
            // 
            // BugReport_LinkLabel
            // 
            this.BugReport_LinkLabel.AutoSize = true;
            this.BugReport_LinkLabel.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.BugReport_LinkLabel.Location = new System.Drawing.Point(64, 90);
            this.BugReport_LinkLabel.Name = "BugReport_LinkLabel";
            this.BugReport_LinkLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.BugReport_LinkLabel.Size = new System.Drawing.Size(122, 22);
            this.BugReport_LinkLabel.TabIndex = 5;
            this.BugReport_LinkLabel.TabStop = true;
            this.BugReport_LinkLabel.Text = "Bug Report";
            this.BugReport_LinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.BugReport_LinkLabel_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LaguageApply_Button);
            this.panel1.Controls.Add(this.LanguageSelect_Combobox);
            this.panel1.Controls.Add(this.HowtouseLinkLabel);
            this.panel1.Controls.Add(this.BugReport_LinkLabel);
            this.panel1.Location = new System.Drawing.Point(923, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 128);
            this.panel1.TabIndex = 6;
            // 
            // LaguageApply_Button
            // 
            this.LaguageApply_Button.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.LaguageApply_Button.Location = new System.Drawing.Point(158, 26);
            this.LaguageApply_Button.Name = "LaguageApply_Button";
            this.LaguageApply_Button.Size = new System.Drawing.Size(101, 29);
            this.LaguageApply_Button.TabIndex = 7;
            this.LaguageApply_Button.Text = "Apply";
            this.LaguageApply_Button.UseVisualStyleBackColor = true;
            this.LaguageApply_Button.Click += new System.EventHandler(this.LaguageApply_Button_Click);
            // 
            // LanguageSelect_Combobox
            // 
            this.LanguageSelect_Combobox.AllowDrop = true;
            this.LanguageSelect_Combobox.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.LanguageSelect_Combobox.FormattingEnabled = true;
            this.LanguageSelect_Combobox.Location = new System.Drawing.Point(3, 27);
            this.LanguageSelect_Combobox.Name = "LanguageSelect_Combobox";
            this.LanguageSelect_Combobox.Size = new System.Drawing.Size(149, 29);
            this.LanguageSelect_Combobox.TabIndex = 6;
            this.LanguageSelect_Combobox.SelectedIndexChanged += new System.EventHandler(this.LanguageSelect_Combobox_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SelectTheMap_Lable);
            this.panel2.Controls.Add(this.MapApply_Button);
            this.panel2.Controls.Add(this.AutoScreenshot_CheckBox);
            this.panel2.Controls.Add(this.Map_ComboBox);
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(355, 128);
            this.panel2.TabIndex = 7;
            // 
            // HideShowPannel_Button
            // 
            this.HideShowPannel_Button.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.HideShowPannel_Button.Location = new System.Drawing.Point(373, 12);
            this.HideShowPannel_Button.Name = "HideShowPannel_Button";
            this.HideShowPannel_Button.Size = new System.Drawing.Size(223, 45);
            this.HideShowPannel_Button.TabIndex = 8;
            this.HideShowPannel_Button.Text = "Hide/Show Pannels";
            this.HideShowPannel_Button.UseVisualStyleBackColor = true;
            this.HideShowPannel_Button.Click += new System.EventHandler(this.HideShowPannel_Button_Click);
            // 
            // FullScreen_Button
            // 
            this.FullScreen_Button.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.FullScreen_Button.Location = new System.Drawing.Point(373, 56);
            this.FullScreen_Button.Name = "FullScreen_Button";
            this.FullScreen_Button.Size = new System.Drawing.Size(223, 42);
            this.FullScreen_Button.TabIndex = 9;
            this.FullScreen_Button.Text = "Full Screen";
            this.FullScreen_Button.UseVisualStyleBackColor = true;
            this.FullScreen_Button.Click += new System.EventHandler(this.FullScreen_Button_Click);
            // 
            // ForceRun_Button
            // 
            this.ForceRun_Button.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.ForceRun_Button.Location = new System.Drawing.Point(373, 98);
            this.ForceRun_Button.Name = "ForceRun_Button";
            this.ForceRun_Button.Size = new System.Drawing.Size(223, 42);
            this.ForceRun_Button.TabIndex = 10;
            this.ForceRun_Button.Text = "Force Run";
            this.ForceRun_Button.UseVisualStyleBackColor = true;
            this.ForceRun_Button.Click += new System.EventHandler(this.ForceRun_Button_Click);
            // 
            // webView2
            // 
            this.webView2.AllowExternalDrop = true;
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Location = new System.Drawing.Point(12, 146);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(1160, 803);
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
            this.Controls.Add(this.ForceRun_Button);
            this.Controls.Add(this.FullScreen_Button);
            this.Controls.Add(this.HideShowPannel_Button);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SelectTheMap_Lable;
        private System.Windows.Forms.ComboBox Map_ComboBox;
        private System.Windows.Forms.Button MapApply_Button;
        private System.Windows.Forms.LinkLabel HowtouseLinkLabel;
        private System.Windows.Forms.CheckBox AutoScreenshot_CheckBox;
        private System.Windows.Forms.LinkLabel BugReport_LinkLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button HideShowPannel_Button;
        private System.Windows.Forms.Button FullScreen_Button;
        private System.Windows.Forms.Button ForceRun_Button;
        private System.Windows.Forms.Button LaguageApply_Button;
        private System.Windows.Forms.ComboBox LanguageSelect_Combobox;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
    }
}

