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
            if (disposing)
            {
                hotkeyManager?.Dispose();
                components?.Dispose();
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
            components = new System.ComponentModel.Container();
            webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            panel1 = new Panel();
            webView2_panel_ui = new Microsoft.Web.WebView2.WinForms.WebView2();
            checkBoxHide = new CheckBox();
            timerSliding = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)webView2).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView2_panel_ui).BeginInit();
            SuspendLayout();
            // 
            // webView2
            // 
            webView2.AllowExternalDrop = true;
            webView2.CreationProperties = null;
            webView2.DefaultBackgroundColor = Color.White;
            webView2.Dock = DockStyle.Fill;
            webView2.Location = new Point(0, 138);
            webView2.Margin = new Padding(3, 4, 3, 4);
            webView2.Name = "webView2";
            webView2.Size = new Size(1309, 1188);
            webView2.TabIndex = 17;
            webView2.ZoomFactor = 1D;
            webView2.Click += webView2_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(webView2_panel_ui);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1309, 138);
            panel1.TabIndex = 22;
            // 
            // webView2_panel_ui
            // 
            webView2_panel_ui.AllowExternalDrop = true;
            webView2_panel_ui.CreationProperties = null;
            webView2_panel_ui.DefaultBackgroundColor = Color.White;
            webView2_panel_ui.Dock = DockStyle.Fill;
            webView2_panel_ui.Location = new Point(0, 0);
            webView2_panel_ui.Margin = new Padding(3, 4, 3, 4);
            webView2_panel_ui.Name = "webView2_panel_ui";
            webView2_panel_ui.Size = new Size(1309, 138);
            webView2_panel_ui.TabIndex = 0;
            webView2_panel_ui.ZoomFactor = 1D;
            webView2_panel_ui.Click += webView2_panel_ui_Click;
            // 
            // checkBoxHide
            // 
            checkBoxHide.Appearance = Appearance.Button;
            checkBoxHide.FlatAppearance.BorderSize = 0;
            checkBoxHide.FlatStyle = FlatStyle.Flat;
            checkBoxHide.Font = new Font("굴림", 12F, FontStyle.Bold);
            checkBoxHide.Location = new Point(0, 138);
            checkBoxHide.Margin = new Padding(3, 4, 3, 4);
            checkBoxHide.Name = "checkBoxHide";
            checkBoxHide.Size = new Size(170, 38);
            checkBoxHide.TabIndex = 23;
            checkBoxHide.Text = "∧ Click to Fold";
            checkBoxHide.TextAlign = ContentAlignment.MiddleCenter;
            checkBoxHide.UseVisualStyleBackColor = true;
            checkBoxHide.CheckedChanged += checkBoxHide_CheckedChanged;
            // 
            // timerSliding
            // 
            timerSliding.Interval = 10;
            timerSliding.Tick += timerSliding_Tick;
            // 
            // WhereAmI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(checkBoxHide);
            Controls.Add(webView2);
            Controls.Add(panel1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "WhereAmI";
            Size = new Size(1309, 1326);
            ((System.ComponentModel.ISupportInitialize)webView2).EndInit();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webView2_panel_ui).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxHide;
        private System.Windows.Forms.Timer timerSliding;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2_panel_ui;
    }
}
