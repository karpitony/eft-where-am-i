namespace eft_where_am_i
{
    partial class SettingPage
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
            this.lblBugReport = new System.Windows.Forms.LinkLabel();
            this.lblHowToUse = new System.Windows.Forms.LinkLabel();
            this.webView2_Settings = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView2_Settings)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBugReport
            // 
            this.lblBugReport.AutoSize = true;
            this.lblBugReport.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.lblBugReport.Location = new System.Drawing.Point(679, 1024);
            this.lblBugReport.Name = "lblBugReport";
            this.lblBugReport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBugReport.Size = new System.Drawing.Size(122, 22);
            this.lblBugReport.TabIndex = 7;
            this.lblBugReport.TabStop = true;
            this.lblBugReport.Text = "Bug Report";
            this.lblBugReport.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBugReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblBugReport_LinkClicked);
            // 
            // lblHowToUse
            // 
            this.lblHowToUse.AutoSize = true;
            this.lblHowToUse.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.lblHowToUse.Location = new System.Drawing.Point(485, 1024);
            this.lblHowToUse.Name = "lblHowToUse";
            this.lblHowToUse.Size = new System.Drawing.Size(129, 22);
            this.lblHowToUse.TabIndex = 6;
            this.lblHowToUse.TabStop = true;
            this.lblHowToUse.Text = "How to Use";
            this.lblHowToUse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHowToUse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHowToUse_LinkClicked);
            // 
            // webView2_Settings
            // 
            this.webView2_Settings.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.webView2_Settings.AllowExternalDrop = true;
            this.webView2_Settings.CreationProperties = null;
            this.webView2_Settings.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView2_Settings.Location = new System.Drawing.Point(0, 0);
            this.webView2_Settings.Name = "webView2_Settings";
            this.webView2_Settings.Size = new System.Drawing.Size(1309, 1061);
            this.webView2_Settings.TabIndex = 8;
            this.webView2_Settings.ZoomFactor = 1D;
            this.webView2_Settings.Click += new System.EventHandler(this.webView21_Click);
            // 
            // SettingPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webView2_Settings);
            this.Controls.Add(this.lblBugReport);
            this.Controls.Add(this.lblHowToUse);
            this.Name = "SettingPage";
            this.Size = new System.Drawing.Size(1309, 1061);
            ((System.ComponentModel.ISupportInitialize)(this.webView2_Settings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.LinkLabel lblBugReport;
        private System.Windows.Forms.LinkLabel lblHowToUse;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2_Settings;
    }
}
