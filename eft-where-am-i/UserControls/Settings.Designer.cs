namespace eft_where_am_i
{
    partial class Settings
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
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnPathChange = new System.Windows.Forms.Button();
            this.chkAutoScreenshotCleaner = new System.Windows.Forms.CheckBox();
            this.cmbLanguageSelect = new System.Windows.Forms.ComboBox();
            this.btnLanguageSelect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnPathAutoFind = new System.Windows.Forms.Button();
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
            // txtPath
            // 
            this.txtPath.Font = new System.Drawing.Font("굴림", 15F);
            this.txtPath.Location = new System.Drawing.Point(45, 194);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(707, 30);
            this.txtPath.TabIndex = 8;
            // 
            // btnPathChange
            // 
            this.btnPathChange.Font = new System.Drawing.Font("굴림", 15F);
            this.btnPathChange.Location = new System.Drawing.Point(45, 230);
            this.btnPathChange.Name = "btnPathChange";
            this.btnPathChange.Size = new System.Drawing.Size(108, 30);
            this.btnPathChange.TabIndex = 9;
            this.btnPathChange.Text = "Change";
            this.btnPathChange.UseVisualStyleBackColor = true;
            this.btnPathChange.Click += new System.EventHandler(this.btnPathChange_Click);
            // 
            // chkAutoScreenshotCleaner
            // 
            this.chkAutoScreenshotCleaner.AutoSize = true;
            this.chkAutoScreenshotCleaner.Font = new System.Drawing.Font("굴림", 15F);
            this.chkAutoScreenshotCleaner.Location = new System.Drawing.Point(45, 309);
            this.chkAutoScreenshotCleaner.Name = "chkAutoScreenshotCleaner";
            this.chkAutoScreenshotCleaner.Size = new System.Drawing.Size(297, 24);
            this.chkAutoScreenshotCleaner.TabIndex = 10;
            this.chkAutoScreenshotCleaner.Text = "Clean Screenshot Automatically";
            this.chkAutoScreenshotCleaner.UseVisualStyleBackColor = true;
            this.chkAutoScreenshotCleaner.CheckedChanged += new System.EventHandler(this.chkAutoScreenshotCleaner_CheckedChanged);
            // 
            // cmbLanguageSelect
            // 
            this.cmbLanguageSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguageSelect.Font = new System.Drawing.Font("굴림", 15F);
            this.cmbLanguageSelect.FormattingEnabled = true;
            this.cmbLanguageSelect.Location = new System.Drawing.Point(45, 103);
            this.cmbLanguageSelect.Name = "cmbLanguageSelect";
            this.cmbLanguageSelect.Size = new System.Drawing.Size(163, 28);
            this.cmbLanguageSelect.TabIndex = 11;
            // 
            // btnLanguageSelect
            // 
            this.btnLanguageSelect.Font = new System.Drawing.Font("굴림", 15F);
            this.btnLanguageSelect.Location = new System.Drawing.Point(223, 103);
            this.btnLanguageSelect.Name = "btnLanguageSelect";
            this.btnLanguageSelect.Size = new System.Drawing.Size(85, 28);
            this.btnLanguageSelect.TabIndex = 12;
            this.btnLanguageSelect.Text = "Apply";
            this.btnLanguageSelect.UseVisualStyleBackColor = true;
            this.btnLanguageSelect.Click += new System.EventHandler(this.btnLanguageSelect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 15F);
            this.label1.Location = new System.Drawing.Point(42, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(372, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "Choose language and click \'Apply\' button.";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(4, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(183, 27);
            this.lblTitle.TabIndex = 14;
            this.lblTitle.Text = "Setting Page";
            // 
            // btnPathAutoFind
            // 
            this.btnPathAutoFind.Font = new System.Drawing.Font("굴림", 15F);
            this.btnPathAutoFind.Location = new System.Drawing.Point(159, 230);
            this.btnPathAutoFind.Name = "btnPathAutoFind";
            this.btnPathAutoFind.Size = new System.Drawing.Size(137, 30);
            this.btnPathAutoFind.TabIndex = 15;
            this.btnPathAutoFind.Text = "Auto Find";
            this.btnPathAutoFind.UseVisualStyleBackColor = true;
            this.btnPathAutoFind.Click += new System.EventHandler(this.btnPathAutoFind_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnPathAutoFind);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLanguageSelect);
            this.Controls.Add(this.cmbLanguageSelect);
            this.Controls.Add(this.chkAutoScreenshotCleaner);
            this.Controls.Add(this.btnPathChange);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblBugReport);
            this.Controls.Add(this.lblHowToUse);
            this.Name = "Settings";
            this.Size = new System.Drawing.Size(1309, 1061);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.LinkLabel lblBugReport;
        private System.Windows.Forms.LinkLabel lblHowToUse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnPathChange;
        private System.Windows.Forms.CheckBox chkAutoScreenshotCleaner;
        private System.Windows.Forms.ComboBox cmbLanguageSelect;
        private System.Windows.Forms.Button btnLanguageSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnPathAutoFind;
    }
}
