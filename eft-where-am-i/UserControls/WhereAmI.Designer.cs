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
            this.components = new System.ComponentModel.Container();
            this.webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxHide = new System.Windows.Forms.CheckBox();
            this.timerSliding = new System.Windows.Forms.Timer(this.components);
            this.webView2_panel_ui = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView2_panel_ui)).BeginInit();
            this.SuspendLayout();
            // 
            // webView2
            // 
            this.webView2.AllowExternalDrop = true;
            this.webView2.CreationProperties = null;
            this.webView2.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView2.Location = new System.Drawing.Point(0, 110);
            this.webView2.Name = "webView2";
            this.webView2.Size = new System.Drawing.Size(1309, 951);
            this.webView2.Source = new System.Uri("https://tarkov-market.com/maps/ground-zero", System.UriKind.Absolute);
            this.webView2.TabIndex = 17;
            this.webView2.ZoomFactor = 1D;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.webView2_panel_ui);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1309, 110);
            this.panel1.TabIndex = 22;
            // 
            // checkBoxHide
            // 
            this.checkBoxHide.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxHide.FlatAppearance.BorderSize = 0;
            this.checkBoxHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxHide.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold);
            this.checkBoxHide.Location = new System.Drawing.Point(0, 110);
            this.checkBoxHide.Name = "checkBoxHide";
            this.checkBoxHide.Size = new System.Drawing.Size(170, 30);
            this.checkBoxHide.TabIndex = 23;
            this.checkBoxHide.Text = "∧ Click to Fold";
            this.checkBoxHide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxHide.UseVisualStyleBackColor = true;
            this.checkBoxHide.CheckedChanged += new System.EventHandler(this.checkBoxHide_CheckedChanged);
            // 
            // timerSliding
            // 
            this.timerSliding.Interval = 10;
            this.timerSliding.Tick += new System.EventHandler(this.timerSliding_Tick);
            // 
            // webView2_panel_ui
            // 
            this.webView2_panel_ui.AllowExternalDrop = true;
            this.webView2_panel_ui.CreationProperties = null;
            this.webView2_panel_ui.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView2_panel_ui.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webView2_panel_ui.Location = new System.Drawing.Point(0, 0);
            this.webView2_panel_ui.Name = "webView2_panel_ui";
            this.webView2_panel_ui.Size = new System.Drawing.Size(1309, 110);
            this.webView2_panel_ui.TabIndex = 0;
            this.webView2_panel_ui.ZoomFactor = 1D;
            // 
            // WhereAmI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxHide);
            this.Controls.Add(this.webView2);
            this.Controls.Add(this.panel1);
            this.Name = "WhereAmI";
            this.Size = new System.Drawing.Size(1309, 1061);
            ((System.ComponentModel.ISupportInitialize)(this.webView2)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webView2_panel_ui)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxHide;
        private System.Windows.Forms.Timer timerSliding;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2_panel_ui;
    }
}
