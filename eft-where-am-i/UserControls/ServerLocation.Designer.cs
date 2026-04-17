// Auto-generated layout code for ServerLocation UI
namespace eft_where_am_i
{
    partial class ServerLocation
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblCurrentLogFile = new System.Windows.Forms.Label();
            this.btnFindLatest = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelCityName = new System.Windows.Forms.Label();
            this.labelRegionName = new System.Windows.Forms.Label();
            this.labelCountryName = new System.Windows.Forms.Label();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.lblHistory = new System.Windows.Forms.Label();
            this.dataGridViewHistory = new System.Windows.Forms.DataGridView();
            this.colCheck = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).BeginInit();
            this.SuspendLayout();
            
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblCurrentLogFile);
            this.splitContainer1.Panel1.Controls.Add(this.btnFindLatest);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewHistory);
            this.splitContainer1.Panel2.Controls.Add(this.lblHistory);
            this.splitContainer1.Size = new System.Drawing.Size(800, 700);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 0;
            
            // 
            // lblCurrentLogFile
            // 
            this.lblCurrentLogFile.AutoSize = true;
            this.lblCurrentLogFile.Location = new System.Drawing.Point(20, 70);
            this.lblCurrentLogFile.Name = "lblCurrentLogFile";
            this.lblCurrentLogFile.Size = new System.Drawing.Size(123, 15);
            this.lblCurrentLogFile.TabIndex = 2;
            this.lblCurrentLogFile.Text = "Current Log: None";
            
            // 
            // btnFindLatest
            // 
            this.btnFindLatest.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFindLatest.Location = new System.Drawing.Point(20, 15);
            this.btnFindLatest.Name = "btnFindLatest";
            this.btnFindLatest.Size = new System.Drawing.Size(200, 45);
            this.btnFindLatest.TabIndex = 0;
            this.btnFindLatest.Text = "최신 접속 갱신";
            this.btnFindLatest.UseVisualStyleBackColor = true;
            this.btnFindLatest.Click += new System.EventHandler(this.btnFindLatest_Click);
            
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelCityName);
            this.groupBox1.Controls.Add(this.labelRegionName);
            this.groupBox1.Controls.Add(this.labelCountryName);
            this.groupBox1.Controls.Add(this.labelIpAddress);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(20, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 130);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "서버 상세 정보";
            
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.Location = new System.Drawing.Point(20, 25);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(81, 19);
            this.labelIpAddress.TabIndex = 0;
            this.labelIpAddress.Text = "IP Address : ";
            
            // 
            // labelCountryName
            // 
            this.labelCountryName.AutoSize = true;
            this.labelCountryName.Location = new System.Drawing.Point(20, 50);
            this.labelCountryName.Name = "labelCountryName";
            this.labelCountryName.Size = new System.Drawing.Size(44, 19);
            this.labelCountryName.TabIndex = 1;
            this.labelCountryName.Text = "국가 : ";
            
            // 
            // labelRegionName
            // 
            this.labelRegionName.AutoSize = true;
            this.labelRegionName.Location = new System.Drawing.Point(20, 75);
            this.labelRegionName.Name = "labelRegionName";
            this.labelRegionName.Size = new System.Drawing.Size(44, 19);
            this.labelRegionName.TabIndex = 2;
            this.labelRegionName.Text = "지역 : ";
            
            // 
            // labelCityName
            // 
            this.labelCityName.AutoSize = true;
            this.labelCityName.Location = new System.Drawing.Point(20, 100);
            this.labelCityName.Name = "labelCityName";
            this.labelCityName.Size = new System.Drawing.Size(44, 19);
            this.labelCityName.TabIndex = 3;
            this.labelCityName.Text = "도시 : ";
            
            // 
            // lblHistory
            // 
            this.lblHistory.AutoSize = true;
            this.lblHistory.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblHistory.Location = new System.Drawing.Point(16, 10);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(200, 20);
            this.lblHistory.TabIndex = 0;
            this.lblHistory.Text = "서버 접속 이력 (전체 로그)";
            
            // 
            // dataGridViewHistory
            // 
            this.dataGridViewHistory.AllowUserToAddRows = false;
            this.dataGridViewHistory.AllowUserToDeleteRows = false;
            this.dataGridViewHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCheck,
            this.colDate,
            this.colIp,
            this.colCity,
            this.colFolder});
            this.dataGridViewHistory.Location = new System.Drawing.Point(20, 40);
            this.dataGridViewHistory.Name = "dataGridViewHistory";
            this.dataGridViewHistory.ReadOnly = true;
            this.dataGridViewHistory.RowTemplate.Height = 25;
            this.dataGridViewHistory.Size = new System.Drawing.Size(760, 380);
            this.dataGridViewHistory.TabIndex = 1;
            this.dataGridViewHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewHistory_CellContentClick);
            
            // 
            // colCheck
            // 
            this.colCheck.HeaderText = "위치 확인";
            this.colCheck.Name = "colCheck";
            this.colCheck.ReadOnly = true;
            this.colCheck.Text = "확인하기";
            this.colCheck.UseColumnTextForButtonValue = true;
            this.colCheck.Width = 80;
            
            // 
            // colDate
            // 
            this.colDate.HeaderText = "생성 일자";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            this.colDate.Width = 150;
            
            // 
            // colIp
            // 
            this.colIp.HeaderText = "매칭 IP";
            this.colIp.Name = "colIp";
            this.colIp.ReadOnly = true;
            this.colIp.Width = 120;
            
            // 
            // colCity
            // 
            this.colCity.HeaderText = "도시 (미확인)";
            this.colCity.Name = "colCity";
            this.colCity.ReadOnly = true;
            this.colCity.Width = 100;
            
            // 
            // colFolder
            // 
            this.colFolder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFolder.HeaderText = "로그 폴더명";
            this.colFolder.Name = "colFolder";
            this.colFolder.ReadOnly = true;
            
            // 
            // ServerLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ServerLocation";
            this.Size = new System.Drawing.Size(800, 700);
            this.Load += new System.EventHandler(this.ServerLocation_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHistory)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lblCurrentLogFile;
        private System.Windows.Forms.Button btnFindLatest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelCityName;
        private System.Windows.Forms.Label labelRegionName;
        private System.Windows.Forms.Label labelCountryName;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Label lblHistory;
        private System.Windows.Forms.DataGridView dataGridViewHistory;
        private System.Windows.Forms.DataGridViewButtonColumn colCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFolder;
    }
}
