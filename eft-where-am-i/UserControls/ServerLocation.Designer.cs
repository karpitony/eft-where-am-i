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
            this.btnFindServer = new System.Windows.Forms.Button();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.labelCountryName = new System.Windows.Forms.Label();
            this.labelRegionName = new System.Windows.Forms.Label();
            this.labelCityName = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // btnFindServer
            // 
            this.btnFindServer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFindServer.Location = new System.Drawing.Point(20, 20);
            this.btnFindServer.Name = "btnFindServer";
            this.btnFindServer.Size = new System.Drawing.Size(200, 50);
            this.btnFindServer.TabIndex = 0;
            this.btnFindServer.Text = "Find Server Location";
            this.btnFindServer.UseVisualStyleBackColor = true;
            this.btnFindServer.Click += new System.EventHandler(this.btnFindServer_Click);
            
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelCityName);
            this.groupBox1.Controls.Add(this.labelRegionName);
            this.groupBox1.Controls.Add(this.labelCountryName);
            this.groupBox1.Controls.Add(this.labelIpAddress);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(20, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 200);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server Information";
            
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.Location = new System.Drawing.Point(20, 40);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(81, 19);
            this.labelIpAddress.TabIndex = 0;
            this.labelIpAddress.Text = "IP Address : ";
            
            // 
            // labelCountryName
            // 
            this.labelCountryName.AutoSize = true;
            this.labelCountryName.Location = new System.Drawing.Point(20, 75);
            this.labelCountryName.Name = "labelCountryName";
            this.labelCountryName.Size = new System.Drawing.Size(68, 19);
            this.labelCountryName.TabIndex = 1;
            this.labelCountryName.Text = "Country : ";
            
            // 
            // labelRegionName
            // 
            this.labelRegionName.AutoSize = true;
            this.labelRegionName.Location = new System.Drawing.Point(20, 110);
            this.labelRegionName.Name = "labelRegionName";
            this.labelRegionName.Size = new System.Drawing.Size(63, 19);
            this.labelRegionName.TabIndex = 2;
            this.labelRegionName.Text = "Region : ";
            
            // 
            // labelCityName
            // 
            this.labelCityName.AutoSize = true;
            this.labelCityName.Location = new System.Drawing.Point(20, 145);
            this.labelCityName.Name = "labelCityName";
            this.labelCityName.Size = new System.Drawing.Size(44, 19);
            this.labelCityName.TabIndex = 3;
            this.labelCityName.Text = "City : ";
            
            // 
            // ServerLocation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnFindServer);
            this.Name = "ServerLocation";
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = System.Drawing.Color.White;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnFindServer;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Label labelCountryName;
        private System.Windows.Forms.Label labelRegionName;
        private System.Windows.Forms.Label labelCityName;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
